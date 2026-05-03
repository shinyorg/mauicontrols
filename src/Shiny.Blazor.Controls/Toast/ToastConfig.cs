namespace Shiny.Blazor.Controls.Toast;

public class ToastConfig
{
    public string Text { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);
    public ToastPosition Position { get; set; } = ToastPosition.Bottom;
    public ToastDisplayMode DisplayMode { get; set; } = ToastDisplayMode.Pill;
    public bool DismissOnTap { get; set; } = true;
    public ToastQueueMode QueueMode { get; set; } = ToastQueueMode.Queue;
    public double Offset { get; set; } = 12;
    public ToastSpinnerPosition Spinner { get; set; } = ToastSpinnerPosition.None;
    public bool ShowProgressBar { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public string? BorderColor { get; set; }
    public double BorderThickness { get; set; }
    public double CornerRadius { get; set; } = 20;
    public string? IconHtml { get; set; }
    public Action? TapCallback { get; set; }
}
