using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny.Maui.Controls.ImageEditor;

namespace Sample.Features.ImageEditor;

public partial class ImageEditorViewModel : ObservableObject
{
    [ObservableProperty]
    byte[]? imageData;

    [ObservableProperty]
    bool canUndo;

    [ObservableProperty]
    bool canRedo;

    [ObservableProperty]
    ImageEditorToolMode currentToolMode;

    [ObservableProperty]
    Color drawColor = Colors.Red;

    [ObservableProperty]
    double drawStrokeWidth = 3;

    [RelayCommand]
    async Task LoadSampleImage()
    {
        // Load the embedded dotnet_bot image as bytes
        using var stream = await FileSystem.OpenAppPackageFileAsync("dotnet_bot.png");
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ImageData = ms.ToArray();
    }

    [RelayCommand]
    async Task PickImage()
    {
        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
        {
            Title = "Select an image to edit"
        });
        if (result == null)
            return;

        using var stream = await result.OpenReadAsync();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ImageData = ms.ToArray();
    }

    [RelayCommand]
    void SetDrawColor(string colorHex)
    {
        DrawColor = Color.FromArgb(colorHex);
    }
}
