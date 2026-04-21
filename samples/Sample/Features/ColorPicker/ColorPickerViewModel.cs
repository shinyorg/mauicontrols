using CommunityToolkit.Mvvm.ComponentModel;

namespace Sample.Features.ColorPicker;

public partial class ColorPickerViewModel : ObservableObject
{
    [ObservableProperty]
    Color selectedColor = Colors.CornflowerBlue;

    [ObservableProperty]
    bool showOpacity = true;

    [ObservableProperty]
    string colorDisplay = "#6495ED";

    partial void OnSelectedColorChanged(Color value)
    {
        var r = (int)(value.Red * 255);
        var g = (int)(value.Green * 255);
        var b = (int)(value.Blue * 255);
        var a = (int)(value.Alpha * 255);

        ColorDisplay = a < 255
            ? $"#{a:X2}{r:X2}{g:X2}{b:X2}"
            : $"#{r:X2}{g:X2}{b:X2}";
    }
}
