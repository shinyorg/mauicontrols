namespace Shiny.Blazor.Controls.Toast;

public interface IToastService
{
    Task<IDisposable> ShowAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> InfoAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> SuccessAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> WarningAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> DangerAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> CriticalAsync(string text, Action<ToastConfig>? configure = null);
    event Action? OnChanged;
    IReadOnlyList<ToastEntry> ActiveToasts { get; }
}

public sealed class ToastEntry
{
    internal ToastEntry(ToastConfig config, string id)
    {
        Config = config;
        Id = id;
    }

    public ToastConfig Config { get; }
    public string Id { get; }
    internal TaskCompletionSource DismissedTcs { get; } = new();
    internal bool IsDismissing { get; set; }
}
