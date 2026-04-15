namespace Shiny.Maui.Controls.Scheduler.Internal;

class DateCarouselPicker : ContentView
{
    readonly HorizontalStackLayout stack;
    readonly ScrollView scroll;
    readonly List<View> items = [];
    DateOnly selectedDate;
    int daysToShow = 1;
    DataTemplate? itemTemplate;
    bool buildPending;

    public Action<DateOnly>? DateSelected { get; set; }

    public DateCarouselPicker()
    {
        stack = new HorizontalStackLayout { Spacing = 0, Padding = new Thickness(4) };
        scroll = new ScrollView
        {
            Orientation = ScrollOrientation.Horizontal,
            Content = stack,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never
        };
        Content = scroll;
    }

    public DateOnly SelectedDate
    {
        get => selectedDate;
        set
        {
            selectedDate = value;
            QueueBuild();
        }
    }

    public int DaysToShow
    {
        get => daysToShow;
        set
        {
            daysToShow = value;
            QueueBuild();
        }
    }

    public DataTemplate? ItemTemplate
    {
        get => itemTemplate;
        set
        {
            itemTemplate = value;
            QueueBuild();
        }
    }

    void QueueBuild()
    {
        if (buildPending) return;
        buildPending = true;
        Dispatcher.Dispatch(() =>
        {
            buildPending = false;
            Build();
        });
    }

    void Build()
    {
        stack.Children.Clear();
        items.Clear();

        var today = DateOnly.FromDateTime(DateTime.Today);
        var start = selectedDate.AddDays(-14);

        for (var i = 0; i < 29; i++)
        {
            var date = start.AddDays(i);
            var isSelected = date == selectedDate;
            var isToday = date == today;

            var template = itemTemplate ?? DefaultTemplates.CreateAppleCalendarDayPickerTemplate();
            var item = (View)template.CreateContent();
            item.BindingContext = new DatePickerItemContext
            {
                Date = date,
                DayNumber = date.Day.ToString(),
                DayName = date.ToString("ddd").ToUpperInvariant(),
                MonthName = date.ToString("MMM").ToUpperInvariant(),
                IsSelected = isSelected,
                IsToday = isToday
            };

            var captured = date;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) =>
            {
                selectedDate = captured;
                DateSelected?.Invoke(captured);
                UpdateSelection();
            };
            item.GestureRecognizers.Add(tap);

            items.Add(item);
            stack.Children.Add(item);
        }

        ScrollToSelected();
    }

    void UpdateSelection()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var start = selectedDate.AddDays(-14);
        for (var i = 0; i < items.Count; i++)
        {
            var date = start.AddDays(i);
            var isSelected = date == selectedDate;
            var isToday = date == today;

            if (items[i].BindingContext is DatePickerItemContext ctx)
            {
                ctx.IsSelected = isSelected;
                items[i].BindingContext = null;
                items[i].BindingContext = ctx;
            }
        }
    }

    async void ScrollToSelected()
    {
        await Task.Delay(50);
        var selectedIdx = 14;
        if (selectedIdx < items.Count)
            await scroll.ScrollToAsync(items[selectedIdx], ScrollToPosition.Center, false);
    }
}