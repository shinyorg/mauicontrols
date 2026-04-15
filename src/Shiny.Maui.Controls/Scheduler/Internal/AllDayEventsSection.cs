namespace Shiny.Maui.Controls.Scheduler.Internal;

class AllDayEventsSection : ContentView
{
    readonly HorizontalStackLayout stack;

    public AllDayEventsSection()
    {
        stack = new HorizontalStackLayout { Spacing = 4, Padding = new Thickness(60, 4, 4, 4) };
        var scroll = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            Content = stack,
            HeightRequest = 32
        };
        Content = scroll;
        IsVisible = false;
    }

    public void SetEvents(IReadOnlyList<SchedulerEvent> events, DataTemplate? template, Action<SchedulerEvent>? onTapped)
    {
        stack.Children.Clear();
        IsVisible = events.Count > 0;

        foreach (var evt in events)
        {
            View view;
            if (template != null)
            {
                view = (View)template.CreateContent();
                view.BindingContext = evt;
            }
            else
            {
                view = new Border
                {
                    BackgroundColor = evt.Color ?? Colors.CornflowerBlue,
                    StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
                    Stroke = Colors.Transparent,
                    Padding = new Thickness(8, 2),
                    Content = new Label
                    {
                        Text = evt.Title,
                        TextColor = Colors.White,
                        FontSize = 11
                    }
                };
            }

            if (onTapped != null)
            {
                var tap = new TapGestureRecognizer();
                var captured = evt;
                tap.Tapped += (_, _) => onTapped(captured);
                view.GestureRecognizers.Add(tap);
            }
            stack.Children.Add(view);
        }
    }
}