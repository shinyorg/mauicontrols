using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sample.Features.ImageViewer;

public partial class ImageViewerViewModel : ObservableObject
{
    [ObservableProperty]
    ImageSource? selectedImage;

    [ObservableProperty]
    bool isViewerOpen;

    [RelayCommand]
    void OpenViewer(string imageSource)
    {
        SelectedImage = imageSource;
        IsViewerOpen = true;
    }
}
