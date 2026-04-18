using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny.Maui.Controls;

namespace Sample.Features.Sheet;

public partial class SheetViewModel : ObservableObject
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

    // -- Top Sheet --

    [ObservableProperty]
    bool isTopSheetOpen;

    [RelayCommand]
    void OpenTopSheet() => IsTopSheetOpen = true;

    [RelayCommand]
    void CloseTopSheet() => IsTopSheetOpen = false;

    // -- Locked: Signature --

    [ObservableProperty]
    bool isSignatureOpen;

    [ObservableProperty]
    ImageSource? signatureImage;

    public bool HasSignature => SignatureImage != null;

    [RelayCommand]
    void OpenSignature() => IsSignatureOpen = true;

    [RelayCommand]
    void CancelSignature() => IsSignatureOpen = false;

    [RelayCommand]
    async Task DoneSignature(DrawingView drawingView)
    {
        var stream = await drawingView.GetImageStream(300, 150);
        if (stream != null)
        {
            SignatureImage = ImageSource.FromStream(() => stream);
            OnPropertyChanged(nameof(HasSignature));
        }
        drawingView.Lines.Clear();
        IsSignatureOpen = false;
    }

    // -- Locked: Selector --

    public ObservableCollection<DetentValue> SelectorDetents { get; } = new()
    {
        DetentValue.Half
    };

    [ObservableProperty]
    bool isSelectorOpen;

    [ObservableProperty]
    string? selectedCountry;

    public bool HasSelection => SelectedCountry != null;

    public ObservableCollection<string> Countries { get; } = new(new[]
    {
        "Argentina", "Australia", "Austria", "Belgium", "Brazil",
        "Canada", "Chile", "China", "Colombia", "Czech Republic",
        "Denmark", "Egypt", "Finland", "France", "Germany",
        "Greece", "Hungary", "India", "Indonesia", "Ireland",
        "Israel", "Italy", "Japan", "Kenya", "Malaysia",
        "Mexico", "Netherlands", "New Zealand", "Nigeria", "Norway",
        "Peru", "Philippines", "Poland", "Portugal", "Romania",
        "Russia", "Saudi Arabia", "Singapore", "South Africa", "South Korea",
        "Spain", "Sweden", "Switzerland", "Thailand", "Turkey",
        "Ukraine", "United Arab Emirates", "United Kingdom", "United States", "Vietnam"
    });

    [RelayCommand]
    void OpenSelector() => IsSelectorOpen = true;

    [RelayCommand]
    void CancelSelector() => IsSelectorOpen = false;

    [RelayCommand]
    void SelectCountry(object? item)
    {
        if (item is string selected)
        {
            SelectedCountry = selected;
            OnPropertyChanged(nameof(HasSelection));
            IsSelectorOpen = false;
        }
    }
}
