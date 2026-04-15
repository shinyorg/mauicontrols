using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.Controls.Cells;

public class TimePickerCell : CellBase
{
    Label valueLabel = default!;
    TimePicker hiddenPicker = default!;

    public static readonly BindableProperty TimeProperty = BindableProperty.Create(
        nameof(Time), typeof(TimeSpan), typeof(TimePickerCell), TimeSpan.Zero,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((TimePickerCell)b).OnTimeChanged());

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(TimePickerCell), "t",
        propertyChanged: (b, o, n) => ((TimePickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(TimePickerCell), null,
        propertyChanged: (b, o, n) => ((TimePickerCell)b).UpdateValueColor());

    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public string Format
    {
        get => (string)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    public Color? ValueTextColor
    {
        get => (Color?)GetValue(ValueTextColorProperty);
        set => SetValue(ValueTextColorProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        valueLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };

        hiddenPicker = new TimePicker
        {
        };
        hiddenPicker.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TimePicker.Time))
                Time = hiddenPicker.Time ?? TimeSpan.Zero;
        };
        hiddenPicker.Unfocused += (s, e) => ClearSelectionHighlight();

#if ANDROID
        // Android: overlay the transparent picker across the entire cell so tapping
        // anywhere opens the native time dialog (Focus() is unreliable on Android).
        hiddenPicker.Opacity = 0.01;
        Grid.SetColumn(hiddenPicker, 0);
        Grid.SetColumnSpan(hiddenPicker, 3);
        Grid.SetRow(hiddenPicker, 0);
        Grid.SetRowSpan(hiddenPicker, 2);
        RootGrid.Children.Add(hiddenPicker);

        UpdateDisplayText();
        return valueLabel;
#else
        // iOS/Mac: hidden zero-size picker in local Grid; Focus() opens native picker
        hiddenPicker.Opacity = 0;
        hiddenPicker.WidthRequest = 0;
        hiddenPicker.HeightRequest = 0;

        var layout = new Grid();
        layout.Children.Add(hiddenPicker);
        layout.Children.Add(valueLabel);

        UpdateDisplayText();
        return layout;
#endif
    }

#if ANDROID
    protected override void OnCellTapped(object? sender, TappedEventArgs e)
    {
        // Android: native picker overlay handles all touch interaction
    }
#endif

    protected override bool ShouldKeepSelection() => true;

    protected override void OnTapped()
    {
#if !ANDROID
        hiddenPicker.Focus();
#endif
    }

    void OnTimeChanged()
    {
        if (hiddenPicker != null)
            hiddenPicker.Time = Time;
        UpdateDisplayText();
    }

    void UpdateDisplayText()
    {
        if (valueLabel == null) return;
        var dt = DateTime.Today.Add(Time);
        valueLabel.Text = dt.ToString(Format);
    }

    void UpdateValueColor()
    {
        var color = ValueTextColor ?? ParentTableView?.CellValueTextColor;
        if (color != null)
            valueLabel.TextColor = color;
        else
            valueLabel.ClearValue(Label.TextColorProperty);
    }
}