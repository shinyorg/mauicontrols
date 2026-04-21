using System.Windows.Input;

namespace Shiny.Maui.Controls.ColorPicker;

public partial class ColorPicker
{
    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
        nameof(SelectedColor),
        typeof(Color),
        typeof(ColorPicker),
        Colors.Red,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((ColorPicker)b).OnSelectedColorChanged((Color)n));

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public static readonly BindableProperty ShowOpacityProperty = BindableProperty.Create(
        nameof(ShowOpacity),
        typeof(bool),
        typeof(ColorPicker),
        true,
        propertyChanged: (b, _, n) => ((ColorPicker)b).OnShowOpacityChanged((bool)n));

    public bool ShowOpacity
    {
        get => (bool)GetValue(ShowOpacityProperty);
        set => SetValue(ShowOpacityProperty, value);
    }

    public static readonly BindableProperty ShowHexInputProperty = BindableProperty.Create(
        nameof(ShowHexInput),
        typeof(bool),
        typeof(ColorPicker),
        true,
        propertyChanged: (b, _, n) =>
        {
            var cp = (ColorPicker)b;
            cp.hexEntry.IsVisible = (bool)n;
        });

    public bool ShowHexInput
    {
        get => (bool)GetValue(ShowHexInputProperty);
        set => SetValue(ShowHexInputProperty, value);
    }

    public static readonly BindableProperty ShowPreviewProperty = BindableProperty.Create(
        nameof(ShowPreview),
        typeof(bool),
        typeof(ColorPicker),
        true,
        propertyChanged: (b, _, n) =>
        {
            var cp = (ColorPicker)b;
            cp.previewSwatch.IsVisible = (bool)n;
        });

    public bool ShowPreview
    {
        get => (bool)GetValue(ShowPreviewProperty);
        set => SetValue(ShowPreviewProperty, value);
    }

    public static readonly BindableProperty ColorChangedCommandProperty = BindableProperty.Create(
        nameof(ColorChangedCommand),
        typeof(ICommand),
        typeof(ColorPicker));

    public ICommand? ColorChangedCommand
    {
        get => (ICommand?)GetValue(ColorChangedCommandProperty);
        set => SetValue(ColorChangedCommandProperty, value);
    }

    public event EventHandler<Color>? ColorChanged;

    void OnSelectedColorChanged(Color color)
    {
        if (isUpdating) return;
        UpdateFromColor(color);

        ColorChanged?.Invoke(this, color);
        if (ColorChangedCommand?.CanExecute(color) == true)
            ColorChangedCommand.Execute(color);
    }

    void OnShowOpacityChanged(bool show)
    {
        // The opacity row is at index 2 in the rootGrid
        if (rootGrid.Children.Count > 2 && rootGrid.Children[2] is View opacityRow)
            opacityRow.IsVisible = show;

        if (!show)
            opacitySlider.Value = 1;
    }
}
