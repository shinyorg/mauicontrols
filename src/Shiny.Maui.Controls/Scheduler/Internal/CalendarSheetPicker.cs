namespace Shiny.Maui.Controls.Scheduler.Internal;

class CalendarSheetPicker : ContentView
{
    const double RowHeight = 34;
    const double RowSpacing = 2;
    const double HeaderHeight = 36;
    const double DayHeaderHeight = 24;
    const double HandleHeight = 20;
    const double PanThreshold = 40;

    readonly Grid rootGrid;
    readonly Grid headerGrid;
    readonly Label monthLabel;
    readonly Button prevButton;
    readonly Button nextButton;
    readonly Grid dayHeaderGrid;
    readonly Grid calendarGrid;
    readonly BoxView handleBar;
    readonly BoxView[] todayIndicators = new BoxView[42];
    readonly Label[] dayLabels = new Label[42];
    readonly Border[] cellBorders = new Border[42];
    readonly DateOnly[] cellDates = new DateOnly[42];

    DateOnly selectedDate = DateOnly.FromDateTime(DateTime.Today);
    DateOnly displayMonth = DateOnly.FromDateTime(DateTime.Today);
    DayOfWeek firstDayOfWeek = DayOfWeek.Sunday;
    bool isExpanded;
    bool buildPending;
    int selectedRow;
    int rowsNeeded = 5;

    public Action<DateOnly>? DateSelected { get; set; }

    public DateOnly SelectedDate
    {
        get => selectedDate;
        set
        {
            if (selectedDate == value) return;
            selectedDate = value;

            // Auto-navigate display month to match selected date
            var newMonth = new DateOnly(value.Year, value.Month, 1);
            if (displayMonth != newMonth)
                displayMonth = newMonth;

            QueueBuild();
        }
    }

    public DayOfWeek FirstDayOfWeek
    {
        get => firstDayOfWeek;
        set
        {
            firstDayOfWeek = value;
            QueueBuild();
        }
    }

    public bool IsExpanded
    {
        get => isExpanded;
        set
        {
            if (isExpanded == value) return;
            isExpanded = value;
            AnimateExpansion();
        }
    }

    public CalendarSheetPicker()
    {
        IsClippedToBounds = true;

        // Month/year header with nav arrows
        monthLabel = new Label
        {
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        prevButton = new Button
        {
            Text = "<",
            FontSize = 16,
            TextColor = Colors.DodgerBlue,
            BackgroundColor = Colors.Transparent,
            WidthRequest = 40,
            HeightRequest = HeaderHeight,
            BorderWidth = 0,
            Padding = 0
        };
        prevButton.Clicked += (_, _) => NavigateMonth(-1);

        nextButton = new Button
        {
            Text = ">",
            FontSize = 16,
            TextColor = Colors.DodgerBlue,
            BackgroundColor = Colors.Transparent,
            WidthRequest = 40,
            HeightRequest = HeaderHeight,
            BorderWidth = 0,
            Padding = 0
        };
        nextButton.Clicked += (_, _) => NavigateMonth(1);

        headerGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(40)),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(40))
            },
            HeightRequest = HeaderHeight,
            Padding = new Thickness(4, 0)
        };
        headerGrid.Add(prevButton, 0);
        headerGrid.Add(monthLabel, 1);
        headerGrid.Add(nextButton, 2);

        // Day-of-week headers
        dayHeaderGrid = new Grid { HeightRequest = DayHeaderHeight, Padding = new Thickness(4, 0) };
        for (var i = 0; i < 7; i++)
            dayHeaderGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        // Calendar grid - 6 rows x 7 columns
        calendarGrid = new Grid
        {
            Padding = new Thickness(4, 0),
            RowSpacing = RowSpacing,
            ColumnSpacing = 0
        };
        for (var i = 0; i < 7; i++)
            calendarGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        for (var i = 0; i < 6; i++)
            calendarGrid.RowDefinitions.Add(new RowDefinition(new GridLength(RowHeight)));

        for (var i = 0; i < 42; i++)
        {
            var dayLabel = new Label
            {
                FontSize = 13,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            dayLabels[i] = dayLabel;

            var todayDot = new BoxView
            {
                WidthRequest = 4,
                HeightRequest = 4,
                CornerRadius = 2,
                Color = Colors.DodgerBlue,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                IsVisible = false,
                Margin = new Thickness(0, 0, 0, 2)
            };
            todayIndicators[i] = todayDot;

            var cellContent = new Grid
            {
                Children = { dayLabel, todayDot }
            };

            var border = new Border
            {
                Content = cellContent,
                StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
                BackgroundColor = Colors.Transparent,
                WidthRequest = 32,
                HeightRequest = 32,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            cellBorders[i] = border;

            var idx = i;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => OnCellTapped(idx);
            border.GestureRecognizers.Add(tap);

            calendarGrid.Add(border, i % 7, i / 7);
        }

        // Pull handle bar
        handleBar = new BoxView
        {
            WidthRequest = 36,
            HeightRequest = 4,
            CornerRadius = 2,
            Color = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        var handleContainer = new ContentView
        {
            Content = handleBar,
            HeightRequest = HandleHeight,
            HorizontalOptions = LayoutOptions.Fill
        };

        // Pan gesture on the handle area for pull-to-expand
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        handleContainer.GestureRecognizers.Add(pan);

        // Tap the handle to toggle
        var handleTap = new TapGestureRecognizer();
        handleTap.Tapped += (_, _) => IsExpanded = !IsExpanded;
        handleContainer.GestureRecognizers.Add(handleTap);

        rootGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),   // 0: month header
                new RowDefinition(GridLength.Auto),   // 1: day-of-week headers
                new RowDefinition(GridLength.Auto),   // 2: calendar grid
                new RowDefinition(GridLength.Auto)    // 3: handle bar
            },
            RowSpacing = 0
        };
        rootGrid.Add(headerGrid, 0, 0);
        rootGrid.Add(dayHeaderGrid, 0, 1);
        rootGrid.Add(calendarGrid, 0, 2);
        rootGrid.Add(handleContainer, 0, 3);

        Content = rootGrid;
        Build();
        // Start collapsed - show only selected week
        ApplyCollapsedLayout(false);
    }

    double panStartY;

    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                panStartY = 0;
                break;

            case GestureStatus.Running:
                panStartY = e.TotalY;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (Math.Abs(panStartY) > PanThreshold)
                    IsExpanded = panStartY > 0;
                break;
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
            if (!isExpanded)
                ApplyCollapsedLayout(false);
        });
    }

    void Build()
    {
        var dm = displayMonth;
        monthLabel.Text = new DateTime(dm.Year, dm.Month, 1).ToString("MMMM yyyy");

        BuildDayHeaders();
        BuildDayCells();
    }

    void BuildDayHeaders()
    {
        dayHeaderGrid.Children.Clear();
        var names = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
        var first = (int)firstDayOfWeek;

        for (var i = 0; i < 7; i++)
        {
            var idx = (first + i) % 7;
            dayHeaderGrid.Add(new Label
            {
                Text = names[idx].ToUpperInvariant()[..2],
                FontSize = 11,
                TextColor = Colors.Gray,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            }, i);
        }
    }

    void BuildDayCells()
    {
        var dm = displayMonth;
        var firstOfMonth = new DateOnly(dm.Year, dm.Month, 1);
        var firstDayOffset = ((int)firstOfMonth.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        var startDate = firstOfMonth.AddDays(-firstDayOffset);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var totalDays = firstDayOffset + DateTime.DaysInMonth(dm.Year, dm.Month);
        rowsNeeded = (totalDays + 6) / 7;

        selectedRow = -1;

        for (var i = 0; i < 42; i++)
        {
            var date = startDate.AddDays(i);
            cellDates[i] = date;
            var isCurrentMonth = date.Month == dm.Month && date.Year == dm.Year;
            var isSelected = date == selectedDate;
            var isToday = date == today;
            var row = i / 7;

            if (isSelected)
                selectedRow = row;

            cellBorders[i].IsVisible = row < rowsNeeded;
            if (row >= rowsNeeded)
            {
                cellBorders[i].Opacity = 0;
                continue;
            }

            dayLabels[i].Text = date.Day.ToString();

            if (isSelected)
            {
                cellBorders[i].BackgroundColor = Colors.DodgerBlue;
                dayLabels[i].TextColor = Colors.White;
                dayLabels[i].FontAttributes = FontAttributes.Bold;
                todayIndicators[i].IsVisible = false;
            }
            else if (isToday)
            {
                cellBorders[i].BackgroundColor = Colors.Transparent;
                dayLabels[i].TextColor = Colors.DodgerBlue;
                dayLabels[i].FontAttributes = FontAttributes.Bold;
                todayIndicators[i].IsVisible = true;
                todayIndicators[i].Color = Colors.DodgerBlue;
            }
            else
            {
                cellBorders[i].BackgroundColor = Colors.Transparent;
                dayLabels[i].TextColor = isCurrentMonth ? Colors.Black : Colors.LightGray;
                dayLabels[i].FontAttributes = FontAttributes.None;
                todayIndicators[i].IsVisible = false;
            }

            cellBorders[i].Opacity = 1;
        }

        if (selectedRow < 0)
            selectedRow = 0;
    }

    void ApplyCollapsedLayout(bool animate)
    {
        // Show only the row containing the selected date
        if (animate)
        {
            for (var r = 0; r < 6; r++)
            {
                var show = r == selectedRow;
                for (var c = 0; c < 7; c++)
                {
                    var idx = r * 7 + c;
                    if (r < rowsNeeded)
                        cellBorders[idx].FadeTo(show ? 1 : 0, 200, Easing.CubicInOut);
                }
                calendarGrid.RowDefinitions[r].Height = show ? new GridLength(RowHeight) : new GridLength(0);
            }
        }
        else
        {
            for (var r = 0; r < 6; r++)
            {
                var show = r == selectedRow;
                for (var c = 0; c < 7; c++)
                {
                    var idx = r * 7 + c;
                    if (r < rowsNeeded)
                        cellBorders[idx].Opacity = show ? 1 : 0;
                }
                calendarGrid.RowDefinitions[r].Height = show ? new GridLength(RowHeight) : new GridLength(0);
            }
        }

        // Hide nav arrows when collapsed
        prevButton.IsVisible = false;
        nextButton.IsVisible = false;
    }

    void ApplyExpandedLayout(bool animate)
    {
        prevButton.IsVisible = true;
        nextButton.IsVisible = true;

        for (var r = 0; r < 6; r++)
        {
            var visible = r < rowsNeeded;
            calendarGrid.RowDefinitions[r].Height = visible ? new GridLength(RowHeight) : new GridLength(0);

            for (var c = 0; c < 7; c++)
            {
                var idx = r * 7 + c;
                if (visible)
                {
                    if (animate)
                        cellBorders[idx].FadeTo(1, 200, Easing.CubicInOut);
                    else
                        cellBorders[idx].Opacity = 1;
                }
                else
                {
                    cellBorders[idx].Opacity = 0;
                }
            }
        }
    }

    void AnimateExpansion()
    {
        if (isExpanded)
            ApplyExpandedLayout(true);
        else
            ApplyCollapsedLayout(true);
    }

    void OnCellTapped(int index)
    {
        var date = cellDates[index];
        selectedDate = date;

        if (date.Month != displayMonth.Month || date.Year != displayMonth.Year)
            displayMonth = new DateOnly(date.Year, date.Month, 1);

        Build();

        if (!isExpanded)
            ApplyCollapsedLayout(false);

        DateSelected?.Invoke(date);
    }

    void NavigateMonth(int direction)
    {
        displayMonth = displayMonth.AddMonths(direction);
        Build();

        if (!isExpanded)
            ApplyCollapsedLayout(false);
    }
}
