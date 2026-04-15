using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.Controls.TableView;
using TvTableSection = Shiny.Maui.Controls.Sections.TableSection;

namespace Shiny.Maui.Controls.Cells;

public class RadioCell : CellBase
{
    RadioButton radioButton = default!;

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value), typeof(object), typeof(RadioCell), null);

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor), typeof(Color), typeof(RadioCell), null,
        propertyChanged: (b, o, n) => ((RadioCell)b).UpdateAccentColor());

    // Attached property for section-level or tableview-level selected value
    public static readonly BindableProperty SelectedValueProperty = BindableProperty.CreateAttached(
        "SelectedValue", typeof(object), typeof(RadioCell), null,
        BindingMode.TwoWay,
        propertyChanged: OnSelectedValueChanged);

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Color? AccentColor
    {
        get => (Color?)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public static object? GetSelectedValue(BindableObject obj) => obj.GetValue(SelectedValueProperty);
    public static void SetSelectedValue(BindableObject obj, object? value) => obj.SetValue(SelectedValueProperty, value);

    protected override View? CreateAccessoryView()
    {
        radioButton = new RadioButton
        {
            VerticalOptions = LayoutOptions.Center
        };
        radioButton.CheckedChanged += OnRadioCheckedChanged;
        return radioButton;
    }

    void OnRadioCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return;

        // Write to Section scope
        if (ParentSection != null)
            SetSelectedValue(ParentSection, Value);

        // Also write to TableView scope (global radio groups)
        if (ParentTableView != null)
            SetSelectedValue(ParentTableView, Value);
    }

    static void OnSelectedValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TvTableSection section)
        {
            foreach (var cell in section.GetVisibleCells())
            {
                if (cell is RadioCell radioCell)
                    radioCell.radioButton.IsChecked = Equals(radioCell.Value, newValue);
            }
        }
        else if (bindable is TvTableView tableView)
        {
            foreach (var sec in tableView.GetAllSections())
            {
                foreach (var cell in sec.GetVisibleCells())
                {
                    if (cell is RadioCell radioCell)
                        radioCell.radioButton.IsChecked = Equals(radioCell.Value, newValue);
                }
            }
        }
    }

    void UpdateAccentColor()
    {
        var color = AccentColor ?? ParentTableView?.CellAccentColor;
        if (color != null)
            radioButton.BorderColor = color;
    }

    protected override void OnTapped()
    {
        radioButton.IsChecked = true;
    }
}