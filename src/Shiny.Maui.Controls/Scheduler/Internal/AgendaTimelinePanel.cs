using Microsoft.Maui.Layouts;

namespace Shiny.Maui.Controls.Scheduler.Internal;

class AgendaTimelinePanel : ContentView
{
    readonly AbsoluteLayout eventsLayer;
    readonly Grid timelineGrid;
    double timeSlotHeight = 60;
    Color timezoneColor = Colors.Gray;
    Color defaultEventColor = Colors.CornflowerBlue;
    DataTemplate? eventTemplate;
    bool use24HourTime = true;
    Color separatorColor = Color.FromRgba(220, 220, 220, 120);
    IReadOnlyList<TimeZoneInfo>? additionalTimezones;
    bool showTimeLabels = true;

    public Action<SchedulerEvent>? EventTapped { get; set; }
    public Action<DateTimeOffset>? TimeSlotTapped { get; set; }

    public AgendaTimelinePanel()
    {
        timelineGrid = new Grid();
        eventsLayer = new AbsoluteLayout();
        Content = timelineGrid;
    }

    public double TimeSlotHeight
    {
        get => timeSlotHeight;
        set { timeSlotHeight = value; }
    }

    public Color TimezoneColor
    {
        get => timezoneColor;
        set { timezoneColor = value; }
    }

    public Color DefaultEventColor
    {
        get => defaultEventColor;
        set { defaultEventColor = value; }
    }

    public DataTemplate? EventTemplate
    {
        get => eventTemplate;
        set { eventTemplate = value; }
    }

    public bool Use24HourTime
    {
        get => use24HourTime;
        set { use24HourTime = value; }
    }

    public Color SeparatorColor
    {
        get => separatorColor;
        set { separatorColor = value; }
    }

    public IReadOnlyList<TimeZoneInfo>? AdditionalTimezones
    {
        get => additionalTimezones;
        set { additionalTimezones = value; }
    }

    public bool ShowTimeLabels
    {
        get => showTimeLabels;
        set { showTimeLabels = value; }
    }

    public void Build(DateOnly date, IReadOnlyList<SchedulerEvent> timedEvents, CurrentTimeIndicator? timeIndicator, bool showTimeMarker)
    {
        timelineGrid.Children.Clear();
        timelineGrid.RowDefinitions.Clear();
        timelineGrid.ColumnDefinitions.Clear();

        var totalHeight = 24 * timeSlotHeight;
        var extraTzs = showTimeLabels ? (additionalTimezones ?? []) : [];
        var localTz = TimeZoneInfo.Local;

        // Column layout: [local time?] [extra tz?] ... [events]
        var eventsColumnIndex = 0;
        if (showTimeLabels)
        {
            timelineGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(56)));
            foreach (var _ in extraTzs)
                timelineGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(56)));
            eventsColumnIndex = 1 + extraTzs.Count;
        }
        timelineGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        for (var hour = 0; hour < 24; hour++)
            timelineGrid.RowDefinitions.Add(new RowDefinition(new GridLength(timeSlotHeight)));

        var timeFormat = use24HourTime ? "HH:mm" : "h tt";
        var localDate = date.ToDateTime(TimeOnly.MinValue);

        for (var hour = 0; hour < 24; hour++)
        {
            if (showTimeLabels)
            {
                // local timezone label
                var lbl = new Label
                {
                    Text = new TimeOnly(hour, 0).ToString(timeFormat),
                    FontSize = 11,
                    TextColor = timezoneColor,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.End,
                    Padding = new Thickness(0, 0, 8, 0),
                    Margin = new Thickness(0, -8, 0, 0),
                    VerticalOptions = LayoutOptions.Start,
                    HeightRequest = 16
                };
                timelineGrid.Add(lbl, 0, hour);

                // additional timezone labels
                for (var t = 0; t < extraTzs.Count; t++)
                {
                    var localDt = new DateTime(localDate.Year, localDate.Month, localDate.Day, hour, 0, 0, DateTimeKind.Local);
                    var converted = TimeZoneInfo.ConvertTime(localDt, localTz, extraTzs[t]);

                    var tzLbl = new Label
                    {
                        Text = converted.ToString(timeFormat),
                        FontSize = 10,
                        TextColor = Colors.SlateGray,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.End,
                        Padding = new Thickness(0, 0, 8, 0),
                        Margin = new Thickness(0, -8, 0, 0),
                        VerticalOptions = LayoutOptions.Start,
                        HeightRequest = 16
                    };
                    timelineGrid.Add(tzLbl, 1 + t, hour);
                }
            }

            var separator = new BoxView
            {
                Color = separatorColor,
                HeightRequest = 0.5,
                VerticalOptions = LayoutOptions.Start
            };
            timelineGrid.Add(separator, eventsColumnIndex, hour);
        }

        // events overlay in the events column
        eventsLayer.Children.Clear();
        eventsLayer.HeightRequest = totalHeight;

        var overlaps = DetectOverlaps(timedEvents);

        foreach (var (evt, column, totalColumns) in overlaps)
        {
            var startMinutes = evt.Start.LocalDateTime.TimeOfDay.TotalMinutes;
            var endMinutes = evt.End.LocalDateTime.TimeOfDay.TotalMinutes;
            if (DateOnly.FromDateTime(evt.Start.LocalDateTime) < date)
                startMinutes = 0;
            if (DateOnly.FromDateTime(evt.End.LocalDateTime) > date)
                endMinutes = 24 * 60;
            var duration = Math.Max(endMinutes - startMinutes, 15);

            var y = startMinutes * timeSlotHeight / 60.0;
            var h = duration * timeSlotHeight / 60.0;

            View eventView;
            if (eventTemplate != null)
            {
                eventView = (View)eventTemplate.CreateContent();
                eventView.BindingContext = evt;
            }
            else
            {
                eventView = CreateDefaultEventView(evt);
            }

            var tap = new TapGestureRecognizer();
            var captured = evt;
            tap.Tapped += (_, _) => EventTapped?.Invoke(captured);
            eventView.GestureRecognizers.Add(tap);

            AbsoluteLayout.SetLayoutBounds(eventView, new Rect(
                (double)column / totalColumns, y, 1.0 / totalColumns, h));
            AbsoluteLayout.SetLayoutFlags(eventView,
                AbsoluteLayoutFlags.XProportional | AbsoluteLayoutFlags.WidthProportional);

            eventsLayer.Children.Add(eventView);
        }

        // time marker
        if (showTimeMarker && timeIndicator != null && date == DateOnly.FromDateTime(DateTime.Today))
        {
            var now = DateTime.Now.TimeOfDay.TotalMinutes;
            var markerY = now * timeSlotHeight / 60.0;

            timeIndicator.UpdateTime(use24HourTime);

            AbsoluteLayout.SetLayoutBounds(timeIndicator, new Rect(0, markerY, 1, 16));
            AbsoluteLayout.SetLayoutFlags(timeIndicator, AbsoluteLayoutFlags.WidthProportional);

            if (!eventsLayer.Children.Contains(timeIndicator))
                eventsLayer.Children.Add(timeIndicator);
        }

        // tappable background
        var bgTap = new TapGestureRecognizer();
        bgTap.Tapped += (_, e) =>
        {
            if (TimeSlotTapped == null) return;
            var pos = e.GetPosition(eventsLayer);
            if (pos.HasValue)
            {
                var minutes = pos.Value.Y / timeSlotHeight * 60.0;
                var rounded = Math.Floor(minutes / 30.0) * 30.0;
                var ts = TimeSpan.FromMinutes(rounded);
                var dt = date.ToDateTime(new TimeOnly(ts.Hours, ts.Minutes));
                TimeSlotTapped(new DateTimeOffset(dt));
            }
        };
        eventsLayer.GestureRecognizers.Add(bgTap);

        timelineGrid.Add(eventsLayer, eventsColumnIndex);
        Grid.SetRowSpan(eventsLayer, 24);
    }

    View CreateDefaultEventView(SchedulerEvent evt)
    {
        var color = evt.Color ?? defaultEventColor;
        var border = new Border
        {
            BackgroundColor = color,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 4 },
            Stroke = Colors.Transparent,
            Padding = new Thickness(6, 4),
            Margin = new Thickness(1)
        };

        var stack = new VerticalStackLayout
        {
            Children =
            {
                new Label
                {
                    Text = evt.Title,
                    TextColor = Colors.White,
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    LineBreakMode = LineBreakMode.TailTruncation
                },
                new Label
                {
                    Text = use24HourTime
                        ? $"{evt.Start.LocalDateTime:HH:mm} - {evt.End.LocalDateTime:HH:mm}"
                        : $"{evt.Start.LocalDateTime:h:mm tt} - {evt.End.LocalDateTime:h:mm tt}",
                    TextColor = Color.FromRgba(255, 255, 255, 200),
                    FontSize = 10
                }
            }
        };

        border.Content = stack;
        return border;
    }

    static List<(SchedulerEvent Event, int Column, int TotalColumns)> DetectOverlaps(IReadOnlyList<SchedulerEvent> events)
    {
        if (events.Count == 0) return [];

        var sorted = events.OrderBy(e => e.Start).ThenBy(e => e.End).ToList();
        var result = new List<(SchedulerEvent Event, int Column, int TotalColumns)>();
        var groups = new List<List<SchedulerEvent>>();

        foreach (var evt in sorted)
        {
            var placed = false;
            foreach (var group in groups)
            {
                if (group.Any(g => Overlaps(g, evt)))
                {
                    group.Add(evt);
                    placed = true;
                    break;
                }
            }
            if (!placed)
                groups.Add([evt]);
        }

        foreach (var group in groups)
        {
            var columns = new List<List<SchedulerEvent>>();
            foreach (var evt in group.OrderBy(e => e.Start))
            {
                var placed = false;
                for (var c = 0; c < columns.Count; c++)
                {
                    if (!columns[c].Any(e => Overlaps(e, evt)))
                    {
                        columns[c].Add(evt);
                        result.Add((evt, c, 0));
                        placed = true;
                        break;
                    }
                }
                if (!placed)
                {
                    columns.Add([evt]);
                    result.Add((evt, columns.Count - 1, 0));
                }
            }

            var totalCols = columns.Count;
            for (var i = result.Count - group.Count; i < result.Count; i++)
                result[i] = (result[i].Event, result[i].Column, totalCols);
        }

        return result;
    }

    static bool Overlaps(SchedulerEvent a, SchedulerEvent b) =>
        a.Start < b.End && b.Start < a.End;
}