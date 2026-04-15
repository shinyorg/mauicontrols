using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sample;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    bool isSheetOpen;

    [ObservableProperty]
    string entryText = string.Empty;

    [ObservableProperty]
    string editorText = string.Empty;

    [ObservableProperty]
    string statusMessage = "Sheet is closed";

    [RelayCommand]
    void OpenSheet() => IsSheetOpen = true;

    [RelayCommand]
    void CloseSheet() => IsSheetOpen = false;

    [RelayCommand]
    void ToggleSheet() => IsSheetOpen = !IsSheetOpen;

    partial void OnIsSheetOpenChanged(bool value)
    {
        StatusMessage = value ? "Sheet is open" : "Sheet is closed";
    }
}