using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.Controls.Cells;

public class DurationPickerCell : CellBase
{
    Label valueLabel = default!;

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration), typeof(TimeSpan?), typeof(DurationPickerCell), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((DurationPickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty MinDurationProperty = BindableProperty.Create(
        nameof(MinDuration), typeof(TimeSpan), typeof(DurationPickerCell), TimeSpan.Zero);

    public static readonly BindableProperty MaxDurationProperty = BindableProperty.Create(
        nameof(MaxDuration), typeof(TimeSpan), typeof(DurationPickerCell), TimeSpan.FromHours(24));

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(DurationPickerCell), @"h\:mm",
        propertyChanged: (b, o, n) => ((DurationPickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty PickerTitleProperty = BindableProperty.Create(
        nameof(PickerTitle), typeof(string), typeof(DurationPickerCell), "Enter duration");

    public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create(
        nameof(SelectedCommand), typeof(ICommand), typeof(DurationPickerCell), null);

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(DurationPickerCell), null,
        propertyChanged: (b, o, n) => ((DurationPickerCell)b).UpdateValueColor());

    public TimeSpan? Duration
    {
        get => (TimeSpan?)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public TimeSpan MinDuration
    {
        get => (TimeSpan)GetValue(MinDurationProperty);
        set => SetValue(MinDurationProperty, value);
    }

    public TimeSpan MaxDuration
    {
        get => (TimeSpan)GetValue(MaxDurationProperty);
        set => SetValue(MaxDurationProperty, value);
    }

    public string Format
    {
        get => (string)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    public string PickerTitle
    {
        get => (string)GetValue(PickerTitleProperty);
        set => SetValue(PickerTitleProperty, value);
    }

    public ICommand? SelectedCommand
    {
        get => (ICommand?)GetValue(SelectedCommandProperty);
        set => SetValue(SelectedCommandProperty, value);
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

    void UpdateDisplayText()
    {
        if (valueLabel == null) return;
        valueLabel.Text = Duration.HasValue ? Duration.Value.ToString(Format) : string.Empty;
    }

    void UpdateValueColor()
    {
        var color = ValueTextColor ?? ParentTableView?.CellValueTextColor;
        if (color != null)
            valueLabel.TextColor = color;
        else
            valueLabel.ClearValue(Label.TextColorProperty);
    }

    protected override bool ShouldKeepSelection() => true;

    protected override async void OnTapped()
    {
        var page = GetParentPage();
        if (page == null) return;

        var initial = Duration.HasValue
            ? $"{(int)Duration.Value.TotalHours}:{Duration.Value.Minutes:D2}"
            : string.Empty;

        var result = await page.DisplayPromptAsync(
            PickerTitle,
            "Format: H:MM (e.g. 1:30)",
            keyboard: Keyboard.Default,
            initialValue: initial);

        ClearSelectionHighlight();

        if (result == null)
            return;

        if (!TryParseDuration(result, out var parsed))
            return;

        parsed = Clamp(parsed, MinDuration, MaxDuration);
        Duration = parsed;

        if (SelectedCommand?.CanExecute(Duration) == true)
            SelectedCommand.Execute(Duration);
    }

    static bool TryParseDuration(string input, out TimeSpan result)
    {
        result = TimeSpan.Zero;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim();

        // Try standard TimeSpan parsing first (handles "1:30", "0:45", "1:30:00", etc.)
        if (TimeSpan.TryParse(input, out result))
            return result >= TimeSpan.Zero;

        // Try plain number as minutes
        if (int.TryParse(input, out var minutes) && minutes >= 0)
        {
            result = TimeSpan.FromMinutes(minutes);
            return true;
        }

        return false;
    }

    static TimeSpan Clamp(TimeSpan value, TimeSpan min, TimeSpan max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}