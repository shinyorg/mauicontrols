using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sample.Features.BottomSheet;

public partial class MinimizedSheetViewModel : ObservableObject
{
    [ObservableProperty]
    bool isSheetOpen;

    [ObservableProperty]
    string statusMessage = "Sheet is closed — minimized bar visible at bottom";

    [RelayCommand]
    void OpenSheet() => IsSheetOpen = true;

    [RelayCommand]
    void CloseSheet() => IsSheetOpen = false;

    partial void OnIsSheetOpenChanged(bool value)
    {
        StatusMessage = value
            ? "Sheet is open"
            : "Sheet is closed — minimized bar visible at bottom";
    }
}
