using Shiny;

namespace Sample;

public partial class AppShell : ShinyShell
{
    public AppShell()
    {
        InitializeComponent();
    }

    async void OnFooterTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            await Launcher.OpenAsync(new Uri("https://shinylib.net"));
        }
        catch
        {
            // Platform may not support launching URLs
        }
    }
}