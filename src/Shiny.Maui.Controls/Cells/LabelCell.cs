using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.Controls.Cells;

public class LabelCell : CellBase
{
    Label valueLabel = default!;

    public static readonly BindableProperty ValueTextProperty = BindableProperty.Create(
        nameof(ValueText), typeof(string), typeof(LabelCell), string.Empty,
        propertyChanged: (b, o, n) => ((LabelCell)b).valueLabel.Text = (string)n);

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(LabelCell), null,
        propertyChanged: (b, o, n) => ((LabelCell)b).UpdateValueTextColor());

    public static readonly BindableProperty ValueTextFontSizeProperty = BindableProperty.Create(
        nameof(ValueTextFontSize), typeof(double), typeof(LabelCell), -1d,
        propertyChanged: (b, o, n) => ((LabelCell)b).UpdateValueTextFontSize());

    public static readonly BindableProperty ValueTextFontFamilyProperty = BindableProperty.Create(
        nameof(ValueTextFontFamily), typeof(string), typeof(LabelCell), null,
        propertyChanged: (b, o, n) => ((LabelCell)b).UpdateValueTextFontFamily());

    public static readonly BindableProperty ValueTextFontAttributesProperty = BindableProperty.Create(
        nameof(ValueTextFontAttributes), typeof(FontAttributes?), typeof(LabelCell), null,
        propertyChanged: (b, o, n) => ((LabelCell)b).UpdateValueTextFontAttributes());

    public string ValueText
    {
        get => (string)GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    public Color? ValueTextColor
    {
        get => (Color?)GetValue(ValueTextColorProperty);
        set => SetValue(ValueTextColorProperty, value);
    }

    public double ValueTextFontSize
    {
        get => (double)GetValue(ValueTextFontSizeProperty);
        set => SetValue(ValueTextFontSizeProperty, value);
    }

    public string? ValueTextFontFamily
    {
        get => (string?)GetValue(ValueTextFontFamilyProperty);
        set => SetValue(ValueTextFontFamilyProperty, value);
    }

    public FontAttributes? ValueTextFontAttributes
    {
        get => (FontAttributes?)GetValue(ValueTextFontAttributesProperty);
        set => SetValue(ValueTextFontAttributesProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        valueLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };
        return valueLabel;
    }

    protected Label ValueLabel => valueLabel;

    void UpdateValueTextColor()
    {
        var color = ValueTextColor ?? ParentTableView?.CellValueTextColor;
        if (color != null)
            valueLabel.TextColor = color;
        else
            valueLabel.ClearValue(Label.TextColorProperty);
    }

    void UpdateValueTextFontSize()
        => valueLabel.FontSize = ResolveDouble(ValueTextFontSize, ParentTableView?.CellValueTextFontSize ?? -1, 16);

    void UpdateValueTextFontFamily()
        => valueLabel.FontFamily = ResolveFontFamily(ValueTextFontFamily, ParentTableView?.CellValueTextFontFamily);

    void UpdateValueTextFontAttributes()
        => valueLabel.FontAttributes = ResolveFontAttributes(ValueTextFontAttributes, ParentTableView?.CellValueTextFontAttributes);
}