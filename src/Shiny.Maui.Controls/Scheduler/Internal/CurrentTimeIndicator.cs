namespace Shiny.Maui.Controls.Scheduler.Internal;

class CurrentTimeIndicator : ContentView
{
    readonly BoxView line;
    readonly Label timeLabel;
    readonly BoxView dot;

    public CurrentTimeIndicator()
    {
        dot = new BoxView
        {
            Color = Colors.Red,
            CornerRadius = 4,
            WidthRequest = 8,
            HeightRequest = 8,
            VerticalOptions = LayoutOptions.Center
        };

        timeLabel = new Label
        {
            FontSize = 9,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.Red,
            VerticalOptions = LayoutOptions.Center,
            VerticalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(2, 0, 0, 0)
        };

        line = new BoxView
        {
            Color = Colors.Red,
            HeightRequest = 2,
            VerticalOptions = LayoutOptions.Center
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star)
            },
            VerticalOptions = LayoutOptions.Start
        };

        grid.Add(dot, 0);
        grid.Add(timeLabel, 1);
        grid.Add(line, 2);

        Content = grid;
    }

    public Color MarkerColor
    {
        set
        {
            line.Color = value;
            dot.Color = value;
            timeLabel.TextColor = value;
        }
    }

    public void UpdateTime(bool use24HourTime)
    {
        var now = DateTime.Now;
        timeLabel.Text = now.ToString(use24HourTime ? "HH:mm" : "h:mm tt");
    }
}