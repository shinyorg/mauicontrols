namespace Shiny.Maui.Controls.Toast;

public interface IToaster
{
    Task<IDisposable> ShowAsync(string text);
    Task<IDisposable> ShowAsync(string text, Action<ToastConfig> configure);
    Task<IDisposable> InfoAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> SuccessAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> WarningAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> DangerAsync(string text, Action<ToastConfig>? configure = null);
    Task<IDisposable> CriticalAsync(string text, Action<ToastConfig>? configure = null);
}
