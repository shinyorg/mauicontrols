using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.Home;

public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty]
    string markdown = string.Empty;

    public HomeViewModel()
    {
        _ = LoadReadmeAsync();
    }

    async Task LoadReadmeAsync()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("README.md");
            using var reader = new StreamReader(stream);
            Markdown = await reader.ReadToEndAsync();
        }
        catch
        {
            Markdown = "# Shiny.Maui.Controls\n\nFailed to load README.";
        }
    }
}
