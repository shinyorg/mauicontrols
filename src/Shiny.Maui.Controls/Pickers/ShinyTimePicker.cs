using Shiny.Maui.Controls.FloatingPanel;

namespace Shiny.Maui.Controls.Pickers;

public class ShinyTimePicker : ContentView
{
    readonly Label valueLabel;
    readonly Border tapArea;
    FloatingPanel.FloatingPanel? panel;

    public static readonly BindableProperty TimeProperty = BindableProperty.Create(
        nameof(Time), typeof(TimeSpan?), typeof(ShinyTimePicker), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((ShinyTimePicker)b).UpdateDisplayText());

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(ShinyTimePicker), "t",
        propertyChanged: (b, o, n) => ((ShinyTimePicker)b).UpdateDisplayText());

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(ShinyTimePicker), "Select time",
        propertyChanged: (b, o, n) => ((ShinyTimePicker)b).UpdateDisplayText());

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor), typeof(Color), typeof(ShinyTimePicker), Colors.Gray);

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(ShinyTimePicker), null,
        propertyChanged: (b, o, n) => ((ShinyTimePicker)b).UpdateDisplayText());

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize), typeof(double), typeof(ShinyTimePicker), 16d,
        propertyChanged: (b, o, n) => ((ShinyTimePicker)b).valueLabel.FontSize = (double)n);

    public static readonly BindableProperty MinuteIntervalProperty = BindableProperty.Create(
        nameof(MinuteInterval), typeof(int), typeof(ShinyTimePicker), 1);

    public static readonly BindableProperty Use24HourProperty = BindableProperty.Create(
        nameof(Use24Hour), typeof(bool), typeof(ShinyTimePicker), false);

    public TimeSpan? Time
    {
        get => (TimeSpan?)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
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

    public bool Use24Hour
    {
        get => (bool)GetValue(Use24HourProperty);
        set => SetValue(Use24HourProperty, value);
    }

    public event EventHandler<TimeSpan>? TimeSelected;

    public ShinyTimePicker()
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
        if (Time.HasValue)
        {
            valueLabel.Text = DateTime.Today.Add(Time.Value).ToString(Format);
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

        var content = BuildTimePickerContent();

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

    View BuildTimePickerContent()
    {
        var currentTime = Time ?? new TimeSpan(12, 0, 0);
        var selectedHour = currentTime.Hours;
        var selectedMinute = currentTime.Minutes;

        var hourPicker = new Picker
        {
            Title = "Hour",
            HorizontalOptions = LayoutOptions.Fill
        };

        var minutePicker = new Picker
        {
            Title = "Minute",
            HorizontalOptions = LayoutOptions.Fill
        };

        var ampmPicker = new Picker
        {
            Title = "AM/PM",
            HorizontalOptions = LayoutOptions.Fill
        };

        if (Use24Hour)
        {
            for (var h = 0; h < 24; h++)
                hourPicker.Items.Add(h.ToString("D2"));
            hourPicker.SelectedIndex = selectedHour;
        }
        else
        {
            for (var h = 1; h <= 12; h++)
                hourPicker.Items.Add(h.ToString());

            ampmPicker.Items.Add("AM");
            ampmPicker.Items.Add("PM");

            var displayHour = selectedHour % 12;
            if (displayHour == 0) displayHour = 12;
            hourPicker.SelectedIndex = displayHour - 1;
            ampmPicker.SelectedIndex = selectedHour >= 12 ? 1 : 0;
        }

        var interval = Math.Max(1, MinuteInterval);
        for (var m = 0; m < 60; m += interval)
            minutePicker.Items.Add(m.ToString("D2"));

        var minuteIndex = selectedMinute / interval;
        if (minuteIndex < minutePicker.Items.Count)
            minutePicker.SelectedIndex = minuteIndex;

        var pickerGrid = new Grid
        {
            ColumnSpacing = 8,
            HorizontalOptions = LayoutOptions.Fill
        };

        if (Use24Hour)
        {
            pickerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            pickerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            pickerGrid.Add(hourPicker, 0);
            pickerGrid.Add(minutePicker, 1);
        }
        else
        {
            pickerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            pickerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            pickerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            pickerGrid.Add(hourPicker, 0);
            pickerGrid.Add(minutePicker, 1);
            pickerGrid.Add(ampmPicker, 2);
        }

        var doneButton = new Button
        {
            Text = "Done",
            HorizontalOptions = LayoutOptions.Fill
        };

        doneButton.Clicked += (_, _) =>
        {
            var hour = hourPicker.SelectedIndex;
            if (!Use24Hour)
            {
                hour = hourPicker.SelectedIndex + 1;
                if (hour == 12) hour = 0;
                if (ampmPicker.SelectedIndex == 1) hour += 12;
            }

            var minute = minutePicker.SelectedIndex * interval;
            var time = new TimeSpan(hour, minute, 0);
            Time = time;
            TimeSelected?.Invoke(this, time);
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
                    Text = "Select Time",
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
