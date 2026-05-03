namespace Shiny.Blazor.Controls.Toast;

public sealed class ToastService : IToastService
{
    const int MaxStackCount = 5;
    readonly List<ToastEntry> activeToasts = new();
    readonly Queue<(ToastConfig Config, TaskCompletionSource<IDisposable> Tcs)> queue = new();
    bool isProcessingQueue;

    static readonly Dictionary<ToastType, (string Bg, string Text, string Border)> DefaultColors = new()
    {
        [ToastType.Info] = ("#1E40AF", "#FFFFFF", "#3B82F6"),
        [ToastType.Success] = ("#166534", "#FFFFFF", "#22C55E"),
        [ToastType.Warning] = ("#854D0E", "#FFFFFF", "#F59E0B"),
        [ToastType.Danger] = ("#9A3412", "#FFFFFF", "#F97316"),
        [ToastType.Critical] = ("#991B1B", "#FFFFFF", "#EF4444"),
    };

    public event Action? OnChanged;
    public IReadOnlyList<ToastEntry> ActiveToasts => activeToasts.AsReadOnly();

    public Task<IDisposable> InfoAsync(string text, Action<ToastConfig>? configure = null)
        => ShowTypedAsync(ToastType.Info, text, configure);

    public Task<IDisposable> SuccessAsync(string text, Action<ToastConfig>? configure = null)
        => ShowTypedAsync(ToastType.Success, text, configure);

    public Task<IDisposable> WarningAsync(string text, Action<ToastConfig>? configure = null)
        => ShowTypedAsync(ToastType.Warning, text, configure);

    public Task<IDisposable> DangerAsync(string text, Action<ToastConfig>? configure = null)
        => ShowTypedAsync(ToastType.Danger, text, configure);

    public Task<IDisposable> CriticalAsync(string text, Action<ToastConfig>? configure = null)
        => ShowTypedAsync(ToastType.Critical, text, configure);

    Task<IDisposable> ShowTypedAsync(ToastType type, string text, Action<ToastConfig>? configure)
    {
        return ShowAsync(text, cfg =>
        {
            var (bg, textColor, border) = DefaultColors[type];
            cfg.BackgroundColor ??= bg;
            cfg.TextColor ??= textColor;
            cfg.BorderColor ??= border;
            if (cfg.BorderThickness == 0)
                cfg.BorderThickness = 1;

            configure?.Invoke(cfg);
        });
    }

    public Task<IDisposable> ShowAsync(string text, Action<ToastConfig>? configure = null)
    {
        var config = new ToastConfig { Text = text };
        configure?.Invoke(config);

        var tcs = new TaskCompletionSource<IDisposable>();

        if (config.QueueMode == ToastQueueMode.Stack)
        {
            _ = ShowStackedAsync(config, tcs);
        }
        else
        {
            queue.Enqueue((config, tcs));
            _ = ProcessQueueAsync();
        }

        return tcs.Task;
    }

    async Task ProcessQueueAsync()
    {
        if (isProcessingQueue)
            return;

        isProcessingQueue = true;
        try
        {
            while (queue.Count > 0)
            {
                var (config, tcs) = queue.Dequeue();
                var entry = new ToastEntry(config, Guid.NewGuid().ToString("N"));
                activeToasts.Add(entry);
                OnChanged?.Invoke();

                var handle = new ToastHandle(() => DismissEntry(entry));
                tcs.SetResult(handle);

                if (config.Duration > TimeSpan.Zero)
                {
                    _ = AutoDismissAsync(entry, config.Duration);
                }

                await entry.DismissedTcs.Task;
                activeToasts.Remove(entry);
                OnChanged?.Invoke();
            }
        }
        finally
        {
            isProcessingQueue = false;
        }
    }

    async Task ShowStackedAsync(ToastConfig config, TaskCompletionSource<IDisposable> tcs)
    {
        while (activeToasts.Count >= MaxStackCount)
        {
            DismissEntry(activeToasts[0]);
            await Task.Delay(50);
        }

        var entry = new ToastEntry(config, Guid.NewGuid().ToString("N"));
        activeToasts.Add(entry);
        OnChanged?.Invoke();

        var handle = new ToastHandle(() => DismissEntry(entry));
        tcs.SetResult(handle);

        if (config.Duration > TimeSpan.Zero)
        {
            _ = AutoDismissAsync(entry, config.Duration);
        }

        await entry.DismissedTcs.Task;
        activeToasts.Remove(entry);
        OnChanged?.Invoke();
    }

    async Task AutoDismissAsync(ToastEntry entry, TimeSpan duration)
    {
        await Task.Delay(duration);
        DismissEntry(entry);
    }

    void DismissEntry(ToastEntry entry)
    {
        if (entry.IsDismissing) return;
        entry.IsDismissing = true;
        OnChanged?.Invoke();

        // Delay removal to allow CSS animation
        _ = Task.Delay(250).ContinueWith(_ => entry.DismissedTcs.TrySetResult());
    }

    public void HandleTap(ToastEntry entry)
    {
        entry.Config.TapCallback?.Invoke();
        if (entry.Config.DismissOnTap)
            DismissEntry(entry);
    }

    sealed class ToastHandle : IDisposable
    {
        Action? dismissAction;
        public ToastHandle(Action action) => dismissAction = action;
        public void Dispose()
        {
            var action = Interlocked.Exchange(ref dismissAction, null);
            action?.Invoke();
        }
    }
}
