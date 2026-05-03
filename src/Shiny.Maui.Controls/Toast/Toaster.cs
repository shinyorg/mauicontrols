using Shiny.Maui.Controls.Infrastructure;

namespace Shiny.Maui.Controls.Toast;

public sealed class Toaster : IToaster
{
    public Task<IDisposable> ShowAsync(string text)
        => ShowCoreAsync(text, null);

    public Task<IDisposable> ShowAsync(string text, Action<ToastConfig> configure)
        => ShowCoreAsync(text, configure);

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

    static Task<IDisposable> ShowTypedAsync(ToastType type, string text, Action<ToastConfig>? configure)
    {
        return ShowCoreAsync(text, cfg =>
        {
            ToastStyles.TryApplyStyle(type, cfg);
            ToastStyles.ApplyDefaults(type, cfg);
            configure?.Invoke(cfg);
        });
    }

    static Task<IDisposable> ShowCoreAsync(string text, Action<ToastConfig>? configure)
    {
        var config = new ToastConfig { Text = text };
        configure?.Invoke(config);

        if (config.UseFeedback)
            FeedbackHelper.Execute(typeof(Toaster), "Show", text);

        return ToastManager.ShowAsync(config);
    }
}
