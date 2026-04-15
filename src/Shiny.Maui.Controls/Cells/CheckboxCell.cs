using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.Controls.TableView;

namespace Shiny.Maui.Controls.Cells;

public class CheckboxCell : CellBase
{
    CheckBox checkBox = default!;

    public static readonly BindableProperty CheckedProperty = BindableProperty.Create(
        nameof(Checked), typeof(bool), typeof(CheckboxCell), false,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((CheckboxCell)b).checkBox.IsChecked = (bool)n);

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor), typeof(Color), typeof(CheckboxCell), null,
        propertyChanged: (b, o, n) => ((CheckboxCell)b).UpdateCheckBoxColor());

    public bool Checked
    {
        get => (bool)GetValue(CheckedProperty);
        set => SetValue(CheckedProperty, value);
    }

    public Color? AccentColor
    {
        get => (Color?)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        checkBox = new CheckBox
        {
            VerticalOptions = LayoutOptions.Center
        };
        checkBox.CheckedChanged += (s, e) => Checked = e.Value;
        return checkBox;
    }

    void UpdateCheckBoxColor()
    {
        var color = AccentColor ?? ParentTableView?.CellAccentColor;
        if (color != null)
            checkBox.Color = color;
    }

    protected override void OnTapped()
    {
        checkBox.IsChecked = !checkBox.IsChecked;
    }
}