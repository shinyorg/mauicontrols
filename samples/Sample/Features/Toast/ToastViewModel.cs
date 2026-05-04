using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny.Maui.Controls.Toast;

namespace Sample.Features.Toast;

public partial class ToastViewModel(IToaster toaster) : ObservableObject
{
    IDisposable? manualToast;

    [ObservableProperty]
    string statusMessage = "Configure and tap Show Toast";

    [ObservableProperty]
    string toastText = "Hello from Shiny!";

    // --- Configuration properties ---

    public List<ToastPosition> Positions { get; } = Enum.GetValues<ToastPosition>().ToList();
    public List<ToastDisplayMode> DisplayModes { get; } = Enum.GetValues<ToastDisplayMode>().ToList();
    public List<ToastQueueMode> QueueModes { get; } = Enum.GetValues<ToastQueueMode>().ToList();
    public List<ToastSpinnerPosition> SpinnerPositions { get; } = Enum.GetValues<ToastSpinnerPosition>().ToList();
    public List<ToastTextOverflow> TextOverflows { get; } = Enum.GetValues<ToastTextOverflow>().ToList();

    [ObservableProperty]
    ToastPosition selectedPosition = ToastPosition.Bottom;

    [ObservableProperty]
    ToastDisplayMode selectedDisplayMode = ToastDisplayMode.Pill;

    [ObservableProperty]
    ToastQueueMode selectedQueueMode = ToastQueueMode.Queue;

    [ObservableProperty]
    ToastSpinnerPosition selectedSpinner = ToastSpinnerPosition.None;

    [ObservableProperty]
    double durationSeconds = 3;

    [ObservableProperty]
    double cornerRadius = 20;

    [ObservableProperty]
    bool dismissOnTap = true;

    [ObservableProperty]
    bool showProgressBar;

    [ObservableProperty]
    bool useFeedback = true;

    [ObservableProperty]
    bool announceToScreenReader = true;

    [ObservableProperty]
    ToastTextOverflow selectedTextOverflow = ToastTextOverflow.Ellipsis;

    [ObservableProperty]
    double marqueeSpeed = 40;

    // --- Commands ---

    [RelayCommand]
    async Task ShowConfiguredToast()
    {
        await toaster.ShowAsync(ToastText, cfg =>
        {
            cfg.Position = SelectedPosition;
            cfg.DisplayMode = SelectedDisplayMode;
            cfg.QueueMode = SelectedQueueMode;
            cfg.Spinner = SelectedSpinner;
            cfg.Duration = TimeSpan.FromSeconds(DurationSeconds);
            cfg.CornerRadius = CornerRadius;
            cfg.DismissOnTap = DismissOnTap;
            cfg.ShowProgressBar = ShowProgressBar;
            cfg.UseFeedback = UseFeedback;
            cfg.AnnounceToScreenReader = AnnounceToScreenReader;
            cfg.TextOverflow = SelectedTextOverflow;
            cfg.MarqueeSpeedPixelsPerSecond = MarqueeSpeed;
        });
        StatusMessage = "Configured toast shown";
    }

    [RelayCommand]
    async Task SpinnerToast()
    {
        manualToast?.Dispose();
        manualToast = await toaster.ShowAsync("Uploading file...", cfg =>
        {
            cfg.Spinner = ToastSpinnerPosition.Left;
            cfg.Duration = TimeSpan.Zero;
            cfg.DismissOnTap = false;
        });
        StatusMessage = "Persistent toast shown (tap Dismiss to close)";
    }

    [RelayCommand]
    void DismissManual()
    {
        manualToast?.Dispose();
        manualToast = null;
        StatusMessage = "Manual toast dismissed";
    }

    [RelayCommand]
    async Task StyledToast()
    {
        await toaster.ShowAsync("Custom styled toast", cfg =>
        {
            cfg.BackgroundColor = Color.FromArgb("#312E81");
            cfg.TextColor = Color.FromArgb("#E0E7FF");
            cfg.BorderColor = Color.FromArgb("#6D28D9");
            cfg.BorderThickness = 1.5;
            cfg.CornerRadius = 12;
        });
        StatusMessage = "Styled toast shown";
    }

    [RelayCommand]
    async Task InfoToast()
    {
        await toaster.InfoAsync("New update available");
        StatusMessage = "Info toast shown";
    }

    [RelayCommand]
    async Task SuccessToast()
    {
        await toaster.SuccessAsync("File uploaded successfully");
        StatusMessage = "Success toast shown";
    }

    [RelayCommand]
    async Task WarningToast()
    {
        await toaster.WarningAsync("Storage almost full");
        StatusMessage = "Warning toast shown";
    }

    [RelayCommand]
    async Task DangerToast()
    {
        await toaster.DangerAsync("Failed to save changes");
        StatusMessage = "Danger toast shown";
    }

    [RelayCommand]
    async Task CriticalToast()
    {
        await toaster.CriticalAsync("System error - contact support");
        StatusMessage = "Critical toast shown";
    }
}
