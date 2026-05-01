using Shiny.Maui.Controls.FloatingPanel;

namespace Shiny.Maui.Controls.Pickers;

public class ShinyDurationPicker : ContentView
{
    readonly Label valueLabel;
    readonly Border tapArea;
    FloatingPanel.FloatingPanel? panel;

    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration), typeof(TimeSpan?), typeof(ShinyDurationPicker), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((ShinyDurationPicker)b).UpdateDisplayText());

    public static readonly BindableProperty MinDurationProperty = BindableProperty.Create(
        nameof(MinDuration), typeof(TimeSpan), typeof(ShinyDurationPicker), TimeSpan.Zero);

    public static readonly BindableProperty MaxDurationProperty = BindableProperty.Create(
        nameof(MaxDuration), typeof(TimeSpan), typeof(ShinyDurationPicker), TimeSpan.FromHours(24));

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(ShinyDurationPicker), @"h\:mm",
        propertyChanged: (b, o, n) => ((ShinyDurationPicker)b).UpdateDisplayText());

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(ShinyDurationPicker), "Select duration",
        propertyChanged: (b, o, n) => ((ShinyDurationPicker)b).UpdateDisplayText());

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor), typeof(Color), typeof(ShinyDurationPicker), Colors.Gray);

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(ShinyDurationPicker), null,
        propertyChanged: (b, o, n) => ((ShinyDurationPicker)b).UpdateDisplayText());

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(ShinyDurationPicker), 16d,
        propertyChanged: (b, o, n) => ((ShinyDurationPicker)b).valueLabel.FontSize = (double)n);

    public static readonly BindableProperty MinuteIntervalProperty = BindableProperty.Create(
        nameof(MinuteInterval), typeof(int), typeof(ShinyDurationPicker), 5);

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

    public int MinuteInterval
    {
        get => (int)GetValue(MinuteIntervalProperty);
        set => SetValue(MinuteIntervalProperty, value);
    }

    public event EventHandler<TimeSpan>? DurationSelected;

    public ShinyDurationPicker()
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

    void UpdateDisplayText()
    {
        if (Duration.HasValue)
        {
            valueLabel.Text = Duration.Value.ToString(Format);
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

        var content = BuildDurationPickerContent();

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

        panel.PanelContent = content;
        panel.IsOpen = true;
    }

    View BuildDurationPickerContent()
    {
        var currentDuration = Duration ?? TimeSpan.Zero;
        var maxHours = (int)MaxDuration.TotalHours;
        var interval = Math.Max(1, MinuteInterval);

        var hourPicker = new Picker
        {
            Title = "Hours",
            HorizontalOptions = LayoutOptions.Fill
        };

        var minutePicker = new Picker
        {
            Title = "Minutes",
            HorizontalOptions = LayoutOptions.Fill
        };

        for (var h = 0; h <= maxHours; h++)
            hourPicker.Items.Add(h.ToString());

        for (var m = 0; m < 60; m += interval)
            minutePicker.Items.Add(m.ToString("D2"));

        var currentHours = (int)currentDuration.TotalHours;
        if (currentHours <= maxHours)
            hourPicker.SelectedIndex = currentHours;

        var minuteIndex = currentDuration.Minutes / interval;
        if (minuteIndex < minutePicker.Items.Count)
            minutePicker.SelectedIndex = minuteIndex;

        var pickerGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 8,
            HorizontalOptions = LayoutOptions.Fill
        };
        pickerGrid.Add(hourPicker, 0);
        pickerGrid.Add(new Label
        {
            Text = "hr",
            VerticalOptions = LayoutOptions.Center,
            TextColor = Colors.Gray
        }, 1);
        pickerGrid.Add(minutePicker, 2);
        pickerGrid.Add(new Label
        {
            Text = "min",
            VerticalOptions = LayoutOptions.Center,
            TextColor = Colors.Gray
        }, 3);

        var doneButton = new Button
        {
            Text = "Done",
            HorizontalOptions = LayoutOptions.Fill
        };

        doneButton.Clicked += (_, _) =>
        {
            var hours = hourPicker.SelectedIndex >= 0 ? hourPicker.SelectedIndex : 0;
            var minutes = minutePicker.SelectedIndex >= 0 ? minutePicker.SelectedIndex * interval : 0;
            var duration = new TimeSpan(hours, minutes, 0);

            if (duration < MinDuration) duration = MinDuration;
            if (duration > MaxDuration) duration = MaxDuration;

            Duration = duration;
            DurationSelected?.Invoke(this, duration);
            panel!.IsOpen = false;
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            BackgroundColor = Colors.Gray,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Fill
        };
        cancelButton.Clicked += (_, _) => panel!.IsOpen = false;

        var buttonGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10
        };
        buttonGrid.Add(cancelButton, 0);
        buttonGrid.Add(doneButton, 1);

        return new VerticalStackLayout
        {
            Padding = new Thickness(20),
            Spacing = 16,
            Children =
            {
                new Label
                {
                    Text = "Select Duration",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center
                },
                pickerGrid,
                buttonGrid
            }
        };
    }
}
