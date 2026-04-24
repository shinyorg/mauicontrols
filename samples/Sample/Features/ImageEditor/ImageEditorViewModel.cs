using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;
using Shiny.Maui.Controls.ImageEditor;

namespace Sample.Features.ImageEditor;

public partial class ImageEditorViewModel(IDialogs dialogs) : ObservableObject
{
    [ObservableProperty]
    ImageSource? imageSource = "dotnet_bot.png";

    [ObservableProperty]
    bool canUndo;

    [ObservableProperty]
    bool canRedo;

    [ObservableProperty]
    ImageEditorToolMode currentToolMode;

    [ObservableProperty]
    Color drawColor = Colors.White;

    [ObservableProperty]
    double drawStrokeWidth = 3;

    public IList<string> AvailableFonts { get; } = new List<string>
    {
        "OpenSansRegular",
        "OpenSansSemibold"
    };

    [ObservableProperty]
    string? textFontFamily = "OpenSansRegular";

    [RelayCommand]
    async Task PickImage()
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Select an image to edit"
        });
        if (result == null)
            return;

        ImageSource = ImageSource.FromFile(result.FullPath);
    }

    [RelayCommand]
    async Task Save(EditedImage editedImage)
    {
        await using var stream = await editedImage.ToStreamAsync(ImageExportFormat.Png);

        var fileName = $"edited_{DateTime.Now:yyyyMMdd_HHmmss}.png";
        var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await using (var fileStream = File.Create(filePath))
            await stream.CopyToAsync(fileStream);

        await dialogs.Alert("Saved", $"Image saved to:\n{filePath}");
    }
}
