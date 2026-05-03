using System.Windows.Input;

namespace Shiny.Maui.Controls.Toast;

public class ToastConfig
{
    public string Text { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(3);
    public ToastPosition Position { get; set; } = ToastPosition.Bottom;
    public ToastDisplayMode DisplayMode { get; set; } = ToastDisplayMode.Pill;
    public bool DismissOnTap { get; set; } = true;
    public ToastQueueMode QueueMode { get; set; } = ToastQueueMode.Queue;
    public Thickness Offset { get; set; } = new(12);
    public ToastSpinnerPosition Spinner { get; set; } = ToastSpinnerPosition.None;
    public bool UseFeedback { get; set; } = true;
    public bool ShowProgressBar { get; set; }
    public Color? BackgroundColor { get; set; }
    public Color? TextColor { get; set; }
    public Color? BorderColor { get; set; }
    public double BorderThickness { get; set; }
    public double CornerRadius { get; set; } = 20;
    public ImageSource? Icon { get; set; }
    public ICommand? TapCommand { get; set; }
    public bool AnnounceToScreenReader { get; set; } = true;
}
