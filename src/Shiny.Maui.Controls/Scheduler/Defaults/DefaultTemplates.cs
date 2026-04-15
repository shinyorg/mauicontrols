using System.Globalization;

namespace Shiny.Maui.Controls.Scheduler;

public static class DefaultTemplates
{
    public static DataTemplate CreateEventItemTemplate() => new(() =>
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(4)),
                new ColumnDefinition(GridLength.Star)
            },
            Padding = new Thickness(2),
            ColumnSpacing = 4
        };

        var colorBar = new BoxView { CornerRadius = 2, Color = Colors.CornflowerBlue };
        colorBar.SetBinding(BoxView.ColorProperty, static (SchedulerEvent e) => e.Color);

        var titleLabel = new Label
        {
            FontSize = 11,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        titleLabel.SetBinding(Label.TextProperty, static (SchedulerEvent e) => e.Title);

        grid.Add(colorBar, 0);
        grid.Add(titleLabel, 1);

        return grid;
    });

    public static DataTemplate CreateOverflowTemplate() => new(() =>
    {
        var label = new Label
        {
            FontSize = 10,
            TextColor = Colors.Gray,
            Padding = new Thickness(6, 0)
        };
        label.SetBinding(Label.TextProperty, static (CalendarOverflowContext c) => c.EventCount, stringFormat: "+{0} more");
        return label;
    });

    public static DataTemplate CreateLoaderTemplate() => new(() =>
    {
        var stack = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Spacing = 8,
            Children =
            {
                new ActivityIndicator { IsRunning = true, Color = Colors.CornflowerBlue },
                new Label
                {
                    Text = "Loading...",
                    FontSize = 12,
                    HorizontalTextAlignment = TextAlignment.Center,
                    TextColor = Colors.Gray
                }
            }
        };
        return stack;
    });

    public static DataTemplate CreateCalendarListDayHeaderTemplate() => new(() =>
    {
        var accentBar = new BoxView
        {
            WidthRequest = 4,
            Color = Colors.DodgerBlue
        };
        accentBar.SetBinding(VisualElement.IsVisibleProperty, static (CalendarListDayGroup g) => g.IsToday);

        var dot = new BoxView
        {
            WidthRequest = 8,
            HeightRequest = 8,
            CornerRadius = 4,
            Color = Colors.DodgerBlue,
            VerticalOptions = LayoutOptions.Center
        };
        dot.SetBinding(VisualElement.IsVisibleProperty, static (CalendarListDayGroup g) => g.IsToday);

        var dateLabel = new Label
        {
            FontAttributes = FontAttributes.Bold,
            FontSize = 14,
            VerticalOptions = LayoutOptions.Center
        };
        dateLabel.SetBinding(Label.TextProperty, static (CalendarListDayGroup g) => g.DateDisplay);

        var countLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Gray,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };
        countLabel.SetBinding(Label.TextProperty, static (CalendarListDayGroup g) => g.EventCountDisplay);

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Padding = new Thickness(0, 0, 12, 0),
            ColumnSpacing = 8,
            BackgroundColor = Color.FromRgba(240, 240, 240, 255),
            MinimumHeightRequest = 36
        };

        grid.Add(accentBar, 0);
        grid.Add(dot, 1);
        grid.Add(dateLabel, 2);
        grid.Add(countLabel, 3);

        return grid;
    });

    public static DataTemplate CreateCalendarListEventItemTemplate() => new(() =>
    {
        var colorBar = new BoxView
        {
            WidthRequest = 4,
            CornerRadius = 2,
            Color = Colors.CornflowerBlue
        };
        colorBar.SetBinding(BoxView.ColorProperty, static (SchedulerEvent e) => e.Color);

        var titleLabel = new Label
        {
            FontSize = 14,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        titleLabel.SetBinding(Label.TextProperty, static (SchedulerEvent e) => e.Title);

        var descLabel = new Label
        {
            FontSize = 11,
            TextColor = Colors.Gray,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        descLabel.SetBinding(Label.TextProperty, static (SchedulerEvent e) => e.Description);

        var startLabel = new Label
        {
            FontSize = 11,
            TextColor = Colors.Gray
        };
        startLabel.SetBinding(Label.TextProperty, static (SchedulerEvent e) => e.Start, stringFormat: "{0:h:mm tt}");

        var endLabel = new Label
        {
            FontSize = 11,
            TextColor = Colors.Gray
        };
        endLabel.SetBinding(Label.TextProperty, static (SchedulerEvent e) => e.End, stringFormat: "{0:h:mm tt}");

        var allDayLabel = new Label
        {
            FontSize = 11,
            TextColor = Colors.Gray,
            Text = "All Day"
        };
        allDayLabel.SetBinding(VisualElement.IsVisibleProperty, static (SchedulerEvent e) => e.IsAllDay);

        var timeRange = new HorizontalStackLayout
        {
            Spacing = 0,
            Children =
            {
                startLabel,
                new Label { Text = " – ", FontSize = 11, TextColor = Colors.Gray },
                endLabel
            }
        };
        timeRange.SetBinding(VisualElement.IsVisibleProperty, static (SchedulerEvent e) => e.IsAllDay,
            converter: new InverseBoolConverter());

        var timeStack = new VerticalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Center,
            Children = { timeRange, allDayLabel }
        };

        var textStack = new VerticalStackLayout
        {
            Spacing = 2,
            Children = { titleLabel, descLabel }
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(4)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Padding = new Thickness(12, 8),
            ColumnSpacing = 8
        };

        grid.Add(colorBar, 0);
        grid.Add(textStack, 1);
        grid.Add(timeStack, 2);

        var border = new Border
        {
            Content = grid,
            Stroke = Color.FromRgba(230, 230, 230, 255),
            StrokeThickness = 1,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Margin = new Thickness(12, 2),
            BackgroundColor = Colors.White
        };

        return border;
    });

    public static DataTemplate CreateAppleCalendarDayPickerTemplate() => new(() =>
    {
        var dayName = new Label
        {
            FontSize = 10,
            HorizontalTextAlignment = TextAlignment.Center
        };
        dayName.SetBinding(Label.TextProperty, static (DatePickerItemContext c) => c.DayName);

        var dayNumber = new Label
        {
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center
        };
        dayNumber.SetBinding(Label.TextProperty, static (DatePickerItemContext c) => c.DayNumber);

        var stack = new VerticalStackLayout
        {
            Spacing = 2,
            HorizontalOptions = LayoutOptions.Center,
            Children = { dayName, dayNumber }
        };

        var circle = new Border
        {
            Content = stack,
            Padding = new Thickness(4, 6),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 22 },
            WidthRequest = 48,
            HeightRequest = 56,
            BackgroundColor = Colors.Transparent
        };

        // Use property-changed handler to update colors based on context
        circle.BindingContextChanged += (s, _) =>
        {
            if (s is not Border b || b.BindingContext is not DatePickerItemContext ctx) return;

            var selected = ctx.IsSelected;
            var today = ctx.IsToday;

            b.BackgroundColor = selected ? Colors.DodgerBlue : Colors.Transparent;
            dayName.TextColor = today && !selected ? Colors.DodgerBlue : selected ? Colors.White : Colors.Gray;
            dayNumber.TextColor = today && !selected ? Colors.DodgerBlue : selected ? Colors.White : Colors.Black;
        };

        return circle;
    });

    sealed class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool b ? !b : value!;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is bool b ? !b : value!;
    }
}