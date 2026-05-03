using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny.Maui.Controls.Toast;

namespace Sample.Features.Toast;

public partial class ToastViewModel(IToaster toaster) : ObservableObject
{
    IDisposable? manualToast;

    [ObservableProperty]
    string statusMessage = "Tap a button to show a toast";

    [RelayCommand]
    async Task BasicToast()
    {
        await toaster.ShowAsync("Item saved successfully!");
        StatusMessage = "Basic toast shown";
    }

    [RelayCommand]
    async Task TopToast()
    {
        await toaster.ShowAsync("New message received", cfg =>
        {
            cfg.Position = ToastPosition.Top;
        });
        StatusMessage = "Top toast shown";
    }

    [RelayCommand]
    async Task FillToast()
    {
        await toaster.ShowAsync("No internet connection", cfg =>
        {
            cfg.DisplayMode = ToastDisplayMode.FillHorizontal;
            cfg.BackgroundColor = Colors.OrangeRed;
            cfg.Duration = TimeSpan.FromSeconds(4);
        });
        StatusMessage = "Fill horizontal toast shown";
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
        StatusMessage = "Spinner toast shown (tap Dismiss to close)";
    }

    [RelayCommand]
    void DismissManual()
    {
        manualToast?.Dispose();
        manualToast = null;
        StatusMessage = "Manual toast dismissed";
    }

    [RelayCommand]
    async Task StackedToasts()
    {
        for (var i = 1; i <= 3; i++)
        {
            await toaster.ShowAsync($"Notification {i}", cfg =>
            {
                cfg.QueueMode = ToastQueueMode.Stack;
                cfg.Duration = TimeSpan.FromSeconds(3 + i);
            });
        }
        StatusMessage = "3 stacked toasts shown";
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
    async Task IconToast()
    {
        await toaster.ShowAsync("Download complete", cfg =>
        {
            cfg.Icon = ImageSource.FromFile("dotnet_bot.png");
            cfg.Duration = TimeSpan.FromSeconds(4);
        });
        StatusMessage = "Icon toast shown";
    }

    [RelayCommand]
    async Task ProgressToast()
    {
        await toaster.ShowAsync("Processing request...", cfg =>
        {
            cfg.ShowProgressBar = true;
            cfg.Duration = TimeSpan.FromSeconds(5);
            cfg.BackgroundColor = Color.FromArgb("#0F766E");
        });
        StatusMessage = "Progress bar toast shown";
    }

    [RelayCommand]
    async Task TapCommandToast()
    {
        await toaster.ShowAsync("Tap me for details!", cfg =>
        {
            cfg.Duration = TimeSpan.FromSeconds(5);
            cfg.TapCommand = new Command(() => StatusMessage = "Toast was tapped!");
        });
        StatusMessage = "Tap command toast shown";
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
