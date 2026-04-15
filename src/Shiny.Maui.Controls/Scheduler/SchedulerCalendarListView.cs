using System.Collections.ObjectModel;

namespace Shiny.Maui.Controls.Scheduler;

public class SchedulerCalendarListView : ContentView
{
    readonly CollectionView collectionView;
    readonly ContentView loaderOverlay;
    readonly ObservableCollection<CalendarListDayGroup> groups = [];

    DateOnly rangeStart;
    DateOnly rangeEnd;
    bool isLoadingMore;
    CancellationTokenSource? loadCts;
    PinchGestureRecognizer? pinchGesture;
    double currentScale = 1;
    double startScale = 1;


    public static readonly BindableProperty ProviderProperty = BindableProperty.Create(
        nameof(Provider), typeof(ISchedulerEventProvider), typeof(SchedulerCalendarListView),
        propertyChanged: (b, _, _) => ((SchedulerCalendarListView)b).OnProviderChanged());

    public static readonly BindableProperty SelectedDateProperty = BindableProperty.Create(
        nameof(SelectedDate), typeof(DateOnly), typeof(SchedulerCalendarListView),
        defaultValue: DateOnly.FromDateTime(DateTime.Today),
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, _, _) => ((SchedulerCalendarListView)b).LoadInitial());

    public static readonly BindableProperty EventItemTemplateProperty = BindableProperty.Create(
        nameof(EventItemTemplate), typeof(DataTemplate), typeof(SchedulerCalendarListView));

    public static readonly BindableProperty DayHeaderTemplateProperty = BindableProperty.Create(
        nameof(DayHeaderTemplate), typeof(DataTemplate), typeof(SchedulerCalendarListView));

    public static readonly BindableProperty LoaderTemplateProperty = BindableProperty.Create(
        nameof(LoaderTemplate), typeof(DataTemplate), typeof(SchedulerCalendarListView));

    public static readonly BindableProperty DaysPerPageProperty = BindableProperty.Create(
        nameof(DaysPerPage), typeof(int), typeof(SchedulerCalendarListView), 30);

    public static readonly BindableProperty DefaultEventColorProperty = BindableProperty.Create(
        nameof(DefaultEventColor), typeof(Color), typeof(SchedulerCalendarListView), Colors.CornflowerBlue);

    public static readonly BindableProperty DayHeaderBackgroundColorProperty = BindableProperty.Create(
        nameof(DayHeaderBackgroundColor), typeof(Color), typeof(SchedulerCalendarListView), Colors.Transparent);

    public static readonly BindableProperty DayHeaderTextColorProperty = BindableProperty.Create(
        nameof(DayHeaderTextColor), typeof(Color), typeof(SchedulerCalendarListView), Colors.Black);

    public static readonly BindableProperty MinDateProperty = BindableProperty.Create(
        nameof(MinDate), typeof(DateOnly?), typeof(SchedulerCalendarListView));

    public static readonly BindableProperty MaxDateProperty = BindableProperty.Create(
        nameof(MaxDate), typeof(DateOnly?), typeof(SchedulerCalendarListView));

    public static readonly BindableProperty AllowPanProperty = BindableProperty.Create(
        nameof(AllowPan), typeof(bool), typeof(SchedulerCalendarListView), true,
        propertyChanged: (b, _, n) => ((SchedulerCalendarListView)b).collectionView.VerticalScrollBarVisibility =
            (bool)n ? ScrollBarVisibility.Default : ScrollBarVisibility.Never);

    public static readonly BindableProperty AllowZoomProperty = BindableProperty.Create(
        nameof(AllowZoom), typeof(bool), typeof(SchedulerCalendarListView), false,
        propertyChanged: (b, _, _) => ((SchedulerCalendarListView)b).UpdateZoomGesture());

    public ISchedulerEventProvider? Provider
    {
        get => (ISchedulerEventProvider?)GetValue(ProviderProperty);
        set => SetValue(ProviderProperty, value);
    }

    public DateOnly SelectedDate
    {
        get => (DateOnly)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public DataTemplate? EventItemTemplate
    {
        get => (DataTemplate?)GetValue(EventItemTemplateProperty);
        set => SetValue(EventItemTemplateProperty, value);
    }

    public DataTemplate? DayHeaderTemplate
    {
        get => (DataTemplate?)GetValue(DayHeaderTemplateProperty);
        set => SetValue(DayHeaderTemplateProperty, value);
    }

    public DataTemplate? LoaderTemplate
    {
        get => (DataTemplate?)GetValue(LoaderTemplateProperty);
        set => SetValue(LoaderTemplateProperty, value);
    }

    public int DaysPerPage
    {
        get => (int)GetValue(DaysPerPageProperty);
        set => SetValue(DaysPerPageProperty, value);
    }

    public Color DefaultEventColor
    {
        get => (Color)GetValue(DefaultEventColorProperty);
        set => SetValue(DefaultEventColorProperty, value);
    }

    public Color DayHeaderBackgroundColor
    {
        get => (Color)GetValue(DayHeaderBackgroundColorProperty);
        set => SetValue(DayHeaderBackgroundColorProperty, value);
    }

    public Color DayHeaderTextColor
    {
        get => (Color)GetValue(DayHeaderTextColorProperty);
        set => SetValue(DayHeaderTextColorProperty, value);
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

    public bool AllowPan
    {
        get => (bool)GetValue(AllowPanProperty);
        set => SetValue(AllowPanProperty, value);
    }

    public bool AllowZoom
    {
        get => (bool)GetValue(AllowZoomProperty);
        set => SetValue(AllowZoomProperty, value);
    }


    public SchedulerCalendarListView()
    {
        collectionView = new CollectionView
        {
            IsGrouped = true,
            ItemsSource = groups,
            RemainingItemsThreshold = 5,
            SelectionMode = SelectionMode.Single,
            GroupHeaderTemplate = new DataTemplate(() =>
            {
                var cv = new ContentView();
                cv.SetBinding(ContentView.BindingContextProperty, ".");
                cv.ControlTemplate = new ControlTemplate(() =>
                {
                    var presenter = new ContentPresenter();
                    return presenter;
                });
                return cv;
            })
        };

        collectionView.RemainingItemsThresholdReached += OnRemainingItemsThresholdReached;
        collectionView.Scrolled += OnScrolled;
        collectionView.SelectionChanged += OnSelectionChanged;

        loaderOverlay = new ContentView
        {
            BackgroundColor = Color.FromRgba(255, 255, 255, 200),
            IsVisible = false
        };

        var rootGrid = new Grid();
        rootGrid.Add(collectionView);
        rootGrid.Add(loaderOverlay);

        HorizontalOptions = LayoutOptions.Fill;
        VerticalOptions = LayoutOptions.Fill;
        Content = rootGrid;
        ApplyTemplates();
    }

    void ApplyTemplates()
    {
        collectionView.GroupHeaderTemplate = DayHeaderTemplate
            ?? DefaultTemplates.CreateCalendarListDayHeaderTemplate();

        collectionView.ItemTemplate = EventItemTemplate
            ?? DefaultTemplates.CreateCalendarListEventItemTemplate();
    }

    void UpdateZoomGesture()
    {
        if (pinchGesture != null)
            collectionView.GestureRecognizers.Remove(pinchGesture);

        if (AllowZoom)
        {
            pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            collectionView.GestureRecognizers.Add(pinchGesture);
        }
        else
        {
            collectionView.Scale = 1;
            currentScale = 1;
        }
    }

    void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                startScale = currentScale;
                break;
            case GestureStatus.Running:
                currentScale = Math.Clamp(startScale * e.Scale, 0.5, 3.0);
                collectionView.Scale = currentScale;
                break;
        }
    }

    void OnProviderChanged() => LoadInitial();

    async void LoadInitial()
    {
        if (Provider == null) return;

        loadCts?.Cancel();
        loadCts = new CancellationTokenSource();
        var token = loadCts.Token;

        groups.Clear();
        var halfPage = DaysPerPage / 2;
        rangeStart = SelectedDate.AddDays(-halfPage);
        rangeEnd = SelectedDate.AddDays(halfPage);

        if (MinDate.HasValue && rangeStart < MinDate.Value)
            rangeStart = MinDate.Value;
        if (MaxDate.HasValue && rangeEnd > MaxDate.Value)
            rangeEnd = MaxDate.Value;

        ShowLoader(true);

        try
        {
            var loadedGroups = await LoadRange(rangeStart, rangeEnd, token);
            if (token.IsCancellationRequested) return;

            foreach (var g in loadedGroups)
                groups.Add(g);

            ScrollToSelectedDate();
        }
        catch (TaskCanceledException) { }
        finally
        {
            if (!token.IsCancellationRequested)
                ShowLoader(false);
        }
    }

    async void OnRemainingItemsThresholdReached(object? sender, EventArgs e)
    {
        if (isLoadingMore || Provider == null) return;
        if (MaxDate.HasValue && rangeEnd >= MaxDate.Value) return;
        isLoadingMore = true;

        try
        {
            var newEnd = rangeEnd.AddDays(DaysPerPage);
            if (MaxDate.HasValue && newEnd > MaxDate.Value)
                newEnd = MaxDate.Value;
            var loadedGroups = await LoadRange(rangeEnd, newEnd, CancellationToken.None);
            rangeEnd = newEnd;

            foreach (var g in loadedGroups)
                groups.Add(g);
        }
        catch (TaskCanceledException) { }
        finally
        {
            isLoadingMore = false;
        }
    }

    async void OnScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        if (isLoadingMore || Provider == null) return;
        if (e.FirstVisibleItemIndex > 3) return;
        if (MinDate.HasValue && rangeStart <= MinDate.Value) return;

        isLoadingMore = true;

        try
        {
            var newStart = rangeStart.AddDays(-DaysPerPage);
            if (MinDate.HasValue && newStart < MinDate.Value)
                newStart = MinDate.Value;
            var loadedGroups = await LoadRange(newStart, rangeStart, CancellationToken.None);
            rangeStart = newStart;

            for (var i = loadedGroups.Count - 1; i >= 0; i--)
                groups.Insert(0, loadedGroups[i]);
        }
        catch (TaskCanceledException) { }
        finally
        {
            isLoadingMore = false;
        }
    }

    void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is SchedulerEvent evt)
        {
            Provider?.OnEventSelected(evt);
            collectionView.SelectedItem = null;
        }
    }

    async Task<List<CalendarListDayGroup>> LoadRange(DateOnly start, DateOnly end, CancellationToken token)
    {
        var startDto = new DateTimeOffset(start.ToDateTime(TimeOnly.MinValue));
        var endDto = new DateTimeOffset(end.ToDateTime(TimeOnly.MinValue));

        var events = await Provider!.GetEvents(startDto, endDto);
        if (token.IsCancellationRequested) return [];

        var result = new List<CalendarListDayGroup>();
        for (var date = start; date < end; date = date.AddDays(1))
        {
            var dayEvents = events
                .Where(e =>
                {
                    var eventStart = DateOnly.FromDateTime(e.Start.LocalDateTime);
                    var eventEnd = e.End.LocalDateTime.TimeOfDay == TimeSpan.Zero
                        ? DateOnly.FromDateTime(e.End.LocalDateTime.AddDays(-1))
                        : DateOnly.FromDateTime(e.End.LocalDateTime);
                    return eventStart <= date && eventEnd >= date;
                })
                .OrderBy(e => !e.IsAllDay)
                .ThenBy(e => e.Start)
                .ToList();

            if (dayEvents.Count > 0)
                result.Add(new CalendarListDayGroup(date, dayEvents));
        }

        return result;
    }

    async void ScrollToSelectedDate()
    {
        await Task.Delay(100);
        var targetGroup = groups.FirstOrDefault(g => g.Date == SelectedDate);
        if (targetGroup != null)
            collectionView.ScrollTo(targetGroup, position: ScrollToPosition.Start, animate: false);
    }

    void ShowLoader(bool show)
    {
        if (show && loaderOverlay.Content == null)
        {
            var template = LoaderTemplate ?? DefaultTemplates.CreateLoaderTemplate();
            loaderOverlay.Content = (View)template.CreateContent();
        }
        loaderOverlay.IsVisible = show;
    }
}