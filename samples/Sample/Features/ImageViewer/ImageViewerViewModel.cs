using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sample.Features.ImageViewer;

public partial class ImageViewerViewModel : ObservableObject
{
    [ObservableProperty]
    bool isViewerOpen;

    [RelayCommand]
    void CloseViewer() => IsViewerOpen = false;
}
