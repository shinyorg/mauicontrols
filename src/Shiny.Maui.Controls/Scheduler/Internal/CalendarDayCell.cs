namespace Shiny.Maui.Controls.Scheduler.Internal;

class CalendarDayCell : ContentView
{
    readonly Label dateLabel;
    readonly VerticalStackLayout eventsStack;
    readonly Grid root;

    DateOnly date;
    IReadOnlyList<SchedulerEvent> events = [];
    bool isSelected;
    bool isCurrentMonth;
    bool isToday;
    int maxEvents = 3;
    bool showCountOnly;
    DataTemplate? eventTemplate;
    DataTemplate? overflowTemplate;
    Color cellColor = Colors.White;
    Color selectedColor = Colors.LightBlue;
    Color currentDayColor = Colors.DodgerBlue;

    public CalendarDayCell()
    {
        dateLabel = new Label
        {
            FontSize = 12,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            HeightRequest = 24,
            WidthRequest = 24
        };

        eventsStack = new VerticalStackLayout
        {
            Spacing = 1
        };

        root = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(new GridLength(28)),
                new RowDefinition(GridLength.Star)
            },
            Padding = new Thickness(1),
            IsClippedToBounds = true
        };

        root.Add(dateLabel, 0, 0);
        root.Add(eventsStack, 0, 1);

        IsClippedToBounds = true;
        Content = root;
    }

    public DateOnly Date
    {
        get => date;
        set { date = value; Refresh(); }
    }

    public IReadOnlyList<SchedulerEvent> Events
    {
        get => events;
        set { events = value; RefreshEvents(); }
    }

    public bool IsSelected
    {
        get => isSelected;
        set { isSelected = value; RefreshAppearance(); }
    }

    public bool IsCurrentMonth
    {
        get => isCurrentMonth;
        set { isCurrentMonth = value; RefreshAppearance(); }
    }

    public bool IsToday
    {
        get => isToday;
        set { isToday = value; RefreshAppearance(); }
    }

    public int MaxEvents
    {
        get => maxEvents;
        set { maxEvents = value; RefreshEvents(); }
    }

    public bool ShowCountOnly
    {
        get => showCountOnly;
        set { showCountOnly = value; RefreshEvents(); }
    }

    public DataTemplate? EventTemplate
    {
        get => eventTemplate;
        set { eventTemplate = value; RefreshEvents(); }
    }

    public DataTemplate? OverflowTemplate
    {
        get => overflowTemplate;
        set { overflowTemplate = value; RefreshEvents(); }
    }

    public Color CellColor
    {
        get => cellColor;
        set { cellColor = value; RefreshAppearance(); }
    }

    public Color SelectedColor
    {
        get => selectedColor;
        set { selectedColor = value; RefreshAppearance(); }
    }

    public Color CurrentDayColor
    {
        get => currentDayColor;
        set { currentDayColor = value; RefreshAppearance(); }
    }

    public Action<SchedulerEvent>? EventTapped { get; set; }
    public Action<DateOnly>? DayTapped { get; set; }

    void Refresh()
    {
        dateLabel.Text = date.Day.ToString();
        RefreshAppearance();
    }

    void RefreshAppearance()
    {
        dateLabel.Opacity = isCurrentMonth ? 1.0 : 0.4;

        if (isToday)
        {
            dateLabel.TextColor = Colors.White;
            dateLabel.BackgroundColor = currentDayColor;
        }
        else
        {
            dateLabel.TextColor = isCurrentMonth ? Colors.Black : Colors.Gray;
            dateLabel.BackgroundColor = Colors.Transparent;
        }

        BackgroundColor = isSelected ? selectedColor : cellColor;
    }

    void RefreshEvents()
    {
        eventsStack.Children.Clear();

        if (events.Count == 0)
            return;

        if (showCountOnly)
        {
            eventsStack.Children.Add(new Label
            {
                Text = events.Count.ToString(),
                FontSize = 10,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.Gray
            });
            return;
        }

        var allDay = events.Where(e => e.IsAllDay).ToList();
        var timed = events.Where(e => !e.IsAllDay).OrderBy(e => e.Start).ToList();
        var sorted = allDay.Concat(timed).ToList();

        var toShow = sorted.Take(maxEvents).ToList();
        var overflow = sorted.Count - maxEvents;

        foreach (var evt in toShow)
        {
            View view;
            if (eventTemplate != null)
            {
                view = (View)eventTemplate.CreateContent();
                view.BindingContext = evt;
            }
            else
            {
                view = CreateDefaultEventView(evt);
            }

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => EventTapped?.Invoke(evt);
            view.GestureRecognizers.Add(tap);
            eventsStack.Children.Add(view);
        }

        if (overflow > 0)
        {
            var ctx = new CalendarOverflowContext { EventCount = sorted.Count - maxEvents, Date = date };
            View overflowView;
            if (overflowTemplate != null)
            {
                overflowView = (View)overflowTemplate.CreateContent();
                overflowView.BindingContext = ctx;
            }
            else
            {
                overflowView = new Label
                {
                    Text = $"+{ctx.EventCount} more",
                    FontSize = 10,
                    TextColor = Colors.Gray,
                    Padding = new Thickness(2, 0)
                };
            }
            eventsStack.Children.Add(overflowView);
        }
    }

    static View CreateDefaultEventView(SchedulerEvent evt)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(3)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 2,
            Padding = new Thickness(1)
        };

        grid.Add(new BoxView
        {
            Color = evt.Color ?? Colors.CornflowerBlue,
            CornerRadius = 1,
            WidthRequest = 3
        }, 0);

        grid.Add(new Label
        {
            Text = evt.Title,
            FontSize = 10,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        }, 1);

        return grid;
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => DayTapped?.Invoke(date);
        GestureRecognizers.Clear();
        GestureRecognizers.Add(tap);
    }
}