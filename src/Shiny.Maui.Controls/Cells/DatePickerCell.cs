using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Shiny.Maui.Controls.FloatingPanel;
using Shiny.Maui.Controls.Pickers;
using Shiny.Maui.Controls.Scheduler.Internal;

namespace Shiny.Maui.Controls.Cells;

public class DatePickerCell : CellBase
{
    Label valueLabel = default!;
    FloatingPanel.FloatingPanel? panel;

    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateTime?), typeof(DatePickerCell), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((DatePickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty InitialDateProperty = BindableProperty.Create(
        nameof(InitialDate), typeof(DateTime), typeof(DatePickerCell), new DateTime(2000, 1, 1));

    public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(
        nameof(MinimumDate), typeof(DateTime), typeof(DatePickerCell), new DateTime(1900, 1, 1));

    public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(
        nameof(MaximumDate), typeof(DateTime), typeof(DatePickerCell), new DateTime(2100, 12, 31));

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(DatePickerCell), "d",
        propertyChanged: (b, o, n) => ((DatePickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(DatePickerCell), null,
        propertyChanged: (b, o, n) => ((DatePickerCell)b).UpdateValueColor());

    public DateTime? Date
    {
        get => (DateTime?)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public DateTime InitialDate
    {
        get => (DateTime)GetValue(InitialDateProperty);
        set => SetValue(InitialDateProperty, value);
    }

    public DateTime MinimumDate
    {
        get => (DateTime)GetValue(MinimumDateProperty);
        set => SetValue(MinimumDateProperty, value);
    }

    public DateTime MaximumDate
    {
        get => (DateTime)GetValue(MaximumDateProperty);
        set => SetValue(MaximumDateProperty, value);
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
        UpdateDisplayText();
        return valueLabel;
    }

    protected override bool ShouldKeepSelection() => true;

    protected override void OnTapped()
    {
        var overlayHost = PickerHelper.FindOverlayHost(this);
        if (overlayHost == null) return;

        if (panel != null && panel.IsOpen)
            return;

        var initialDate = Date.HasValue
            ? DateOnly.FromDateTime(Date.Value)
            : DateOnly.FromDateTime(InitialDate);

        var calendar = new CalendarSheetPicker
        {
            IsExpanded = true,
            SelectedDate = initialDate
        };

        calendar.DateSelected = date =>
        {
            var minDate = DateOnly.FromDateTime(MinimumDate);
            var maxDate = DateOnly.FromDateTime(MaximumDate);

            if (date < minDate || date > maxDate) return;

            Date = date.ToDateTime(TimeOnly.MinValue);
            panel!.IsOpen = false;
            ClearSelectionHighlight();
        };

        if (panel == null)
        {
            panel = new FloatingPanel.FloatingPanel
            {
                FitContent = true,
                HasBackdrop = true,
                CloseOnBackdropTap = true,
                ShowHandle = false,
                IsLocked = true,
                PanelCornerRadius = 16
            };
            panel.Closed += (_, _) => ClearSelectionHighlight();
            overlayHost.Children.Add(panel);
        }

        panel.PanelContent = calendar;
        panel.IsOpen = true;
    }

    void UpdateDisplayText()
    {
        if (valueLabel == null) return;
        valueLabel.Text = Date?.ToString(Format) ?? string.Empty;
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
