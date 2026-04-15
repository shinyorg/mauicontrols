using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.Controls.TableView;

namespace Shiny.Maui.Controls.Cells;

public class SwitchCell : CellBase
{
    Switch switchControl = default!;

    public static readonly BindableProperty OnProperty = BindableProperty.Create(
        nameof(On), typeof(bool), typeof(SwitchCell), false,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((SwitchCell)b).switchControl.IsToggled = (bool)n);

    public static readonly BindableProperty OnColorProperty = BindableProperty.Create(
        nameof(OnColor), typeof(Color), typeof(SwitchCell), null,
        propertyChanged: (b, o, n) => ((SwitchCell)b).UpdateSwitchColor());

    public bool On
    {
        get => (bool)GetValue(OnProperty);
        set => SetValue(OnProperty, value);
    }

    public Color? OnColor
    {
        get => (Color?)GetValue(OnColorProperty);
        set => SetValue(OnColorProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        switchControl = new Switch
        {
            VerticalOptions = LayoutOptions.Center
        };
        switchControl.Toggled += (s, e) => On = e.Value;
        return switchControl;
    }

    void UpdateSwitchColor()
    {
        var color = OnColor ?? ParentTableView?.CellAccentColor;
        if (color != null)
            switchControl.OnColor = color;
    }

    protected override void OnTapped()
    {
        switchControl.IsToggled = !switchControl.IsToggled;
    }
}