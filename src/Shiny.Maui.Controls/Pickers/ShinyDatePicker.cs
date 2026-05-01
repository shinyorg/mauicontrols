using Shiny.Maui.Controls.FloatingPanel;
using Shiny.Maui.Controls.Scheduler.Internal;

namespace Shiny.Maui.Controls.Pickers;

public class ShinyDatePicker : ContentView
{
    readonly Label valueLabel;
    readonly Border tapArea;
    CalendarSheetPicker? calendar;
    FloatingPanel.FloatingPanel? panel;

    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateOnly?), typeof(ShinyDatePicker), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((ShinyDatePicker)b).OnDateChanged());

    public static readonly BindableProperty MinDateProperty = BindableProperty.Create(
        nameof(MinDate), typeof(DateOnly?), typeof(ShinyDatePicker));

    public static readonly BindableProperty MaxDateProperty = BindableProperty.Create(
        nameof(MaxDate), typeof(DateOnly?), typeof(ShinyDatePicker));

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(ShinyDatePicker), "d",
        propertyChanged: (b, o, n) => ((ShinyDatePicker)b).UpdateDisplayText());

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(ShinyDatePicker), "Select date",
        propertyChanged: (b, o, n) => ((ShinyDatePicker)b).UpdateDisplayText());

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor), typeof(Color), typeof(ShinyDatePicker), Colors.Gray);

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(ShinyDatePicker), null,
        propertyChanged: (b, o, n) => ((ShinyDatePicker)b).UpdateDisplayText());

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(ShinyDatePicker), 16d,
        propertyChanged: (b, o, n) => ((ShinyDatePicker)b).valueLabel.FontSize = (double)n);

    public static readonly BindableProperty FirstDayOfWeekProperty = BindableProperty.Create(
        nameof(FirstDayOfWeek), typeof(DayOfWeek), typeof(ShinyDatePicker), DayOfWeek.Sunday);

    public DateOnly? Date
    {
        get => (DateOnly?)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public DateOnly? MinDate
    {
        get => (DateOnly?)GetValue(MinDateProperty);
        set => SetValue(MinDateProperty, value);
    }

    public DateOnly? MaxDate
    {
        get => (DateOnly?)GetValue(MaxDateProperty);
        set => SetValue(MaxDateProperty, value);
    }

    public string Format
    {
        get => (string)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public Color PlaceholderColor
    {
        get => (Color)GetValue(PlaceholderColorProperty);
        set => SetValue(PlaceholderColorProperty, value);
    }

    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public DayOfWeek FirstDayOfWeek
    {
        get => (DayOfWeek)GetValue(FirstDayOfWeekProperty);
        set => SetValue(FirstDayOfWeekProperty, value);
    }

    public event EventHandler<DateOnly>? DateSelected;

    public ShinyDatePicker()
    {
        valueLabel = new Label
        {
            FontSize = 16,
            VerticalOptions = LayoutOptions.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        var chevron = new Label
        {
            Text = "▼",
            FontSize = 10,
            TextColor = Colors.Gray,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(4, 0, 0, 0)
        };

        var layout = new HorizontalStackLayout
        {
            Children = { valueLabel, chevron },
            VerticalOptions = LayoutOptions.Center
        };

        tapArea = new Border
        {
            Content = layout,
            Padding = new Thickness(12, 8),
            StrokeThickness = 1,
            Stroke = Colors.LightGray,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            BackgroundColor = Colors.Transparent
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        tapArea.GestureRecognizers.Add(tap);

        Content = tapArea;
        UpdateDisplayText();
    }

    void OnDateChanged()
    {
        UpdateDisplayText();
        if (calendar != null && Date.HasValue)
            calendar.SelectedDate = Date.Value;
    }

    void UpdateDisplayText()
    {
        if (Date.HasValue)
        {
            valueLabel.Text = Date.Value.ToDateTime(TimeOnly.MinValue).ToString(Format);
            valueLabel.TextColor = TextColor ?? (Color?)Label.TextColorProperty.DefaultValue ?? Colors.Black;
        }
        else
        {
            valueLabel.Text = Placeholder;
            valueLabel.TextColor = PlaceholderColor;
        }
    }

    void OnTapped(object? sender, TappedEventArgs e)
    {
        var overlayHost = PickerHelper.FindOverlayHost(this);
        if (overlayHost == null) return;

        if (panel != null && panel.IsOpen)
            return;

        calendar = new CalendarSheetPicker
        {
            IsExpanded = true,
            FirstDayOfWeek = FirstDayOfWeek
        };

        if (Date.HasValue)
            calendar.SelectedDate = Date.Value;

        calendar.DateSelected = OnCalendarDateSelected;

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
            overlayHost.Children.Add(panel);
        }

        panel.PanelContent = calendar;
        panel.IsOpen = true;
    }

    void OnCalendarDateSelected(DateOnly date)
    {
        if (MinDate.HasValue && date < MinDate.Value) return;
        if (MaxDate.HasValue && date > MaxDate.Value) return;

        Date = date;
        DateSelected?.Invoke(this, date);

        if (panel != null)
            panel.IsOpen = false;
    }

}
