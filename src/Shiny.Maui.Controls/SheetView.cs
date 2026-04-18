using System.Collections.ObjectModel;

namespace Shiny.Maui.Controls;

[ContentProperty(nameof(SheetContent))]
public class SheetView : ContentView
{
    const double DefaultAnimationDuration = 250;
    const double DragHandleHeight = 30;
    const double BackdropMaxOpacity = 0.5;

    readonly Grid outerGrid;
    readonly Grid rootGrid;
    readonly BoxView backdrop;
    readonly Border sheetContainer;
    readonly Grid sheetInnerGrid;
    readonly Grid dragHandleContainer;
    readonly BoxView dragHandle;
    readonly ScrollView scrollView;
    readonly ContentView contentHost;
    readonly ContentView headerHost;
    readonly ContentView minimizedHost;
    readonly PanGestureRecognizer panGesture;

    double sheetStartTranslationY;
    double availableHeight;
    double previousAvailableHeight;
    bool isAnimating;
    bool isMinimized;
    int currentDetentIndex;
    int detentIndexBeforeKeyboard = -1;
    bool isKeyboardVisible;
    DetentValue? fitContentDetent;

    bool IsBottom => Direction == SheetDirection.Bottom;

    double GetOffScreenY() => IsBottom ? availableHeight : -availableHeight;

    double GetTranslationYForDetent(DetentValue detent)
        => (IsBottom ? 1.0 : -1.0) * availableHeight * (1.0 - detent.Ratio);

    double GetBackdropProgress(double translationY)
        => 1.0 - (Math.Abs(translationY) / availableHeight);

    double GetMinimizedY()
    {
        var headerHeight = headerHost.Height > 0 ? headerHost.Height : 0;
        if (headerHeight <= 0) return GetOffScreenY();
        return IsBottom
            ? availableHeight - headerHeight
            : -(availableHeight - headerHeight);
    }

    public SheetView()
    {
        IsVisible = false;

        // Backdrop
        backdrop = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0,
            InputTransparent = false
        };
        var backdropTap = new TapGestureRecognizer();
        backdropTap.Tapped += OnBackdropTapped;
        backdrop.GestureRecognizers.Add(backdropTap);

        // Drag handle
        dragHandle = new BoxView
        {
            HeightRequest = 4,
            WidthRequest = 40,
            CornerRadius = 2,
            Color = Colors.Grey,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 10, 0, 10)
        };

        // Content host inside scroll view
        contentHost = new ContentView();
        scrollView = new ScrollView
        {
            Content = contentHost,
            IsEnabled = false
        };

        // Header host (lives inside the sheet when open)
        headerHost = new ContentView();
        headerHost.SizeChanged += OnHeaderHostSizeChanged;

        // Minimized host (lives outside rootGrid — never blocked by InputTransparent)
        minimizedHost = new ContentView
        {
            IsVisible = false
        };
        var minimizedTap = new TapGestureRecognizer();
        minimizedTap.Tapped += (_, _) => { if (!IsOpen && isMinimized) IsOpen = true; };
        minimizedHost.GestureRecognizers.Add(minimizedTap);

        // Drag handle container
        dragHandleContainer = new Grid
        {
            HeightRequest = DragHandleHeight,
            BackgroundColor = Colors.Transparent
        };
        dragHandleContainer.Children.Add(dragHandle);

        // Pan gesture on the whole sheet
        panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;

        // Inner grid: header, handle, content (bottom) or content, handle, header (top)
        sheetInnerGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            }
        };
        sheetInnerGrid.Children.Add(headerHost);
        Grid.SetRow(headerHost, 0);
        sheetInnerGrid.Children.Add(dragHandleContainer);
        Grid.SetRow(dragHandleContainer, 1);
        sheetInnerGrid.Children.Add(scrollView);
        Grid.SetRow(scrollView, 2);

        // Sheet container with rounded corners
        sheetContainer = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(16, 16, 0, 0)
            },
            Stroke = Colors.Transparent,
            StrokeThickness = 0,
            BackgroundColor = Colors.White,
            Content = sheetInnerGrid
        };

        sheetContainer.GestureRecognizers.Add(panGesture);

        // Root grid stacks backdrop + sheet (full-page overlay when open)
        rootGrid = new Grid { IsVisible = false };
        rootGrid.Children.Add(backdrop);
        rootGrid.Children.Add(sheetContainer);
        rootGrid.SizeChanged += OnRootGridSizeChanged;

        // Outer grid: rootGrid (full sheet) + minimizedHost (small edge peek)
        outerGrid = new Grid();
        outerGrid.Children.Add(rootGrid);
        outerGrid.Children.Add(minimizedHost);

        Content = outerGrid;

        // Default detents
        Detents = new ObservableCollection<DetentValue>
        {
            DetentValue.Quarter,
            DetentValue.Half,
            DetentValue.Full
        };
    }


    public static readonly BindableProperty DirectionProperty = BindableProperty.Create(
        nameof(Direction),
        typeof(SheetDirection),
        typeof(SheetView),
        SheetDirection.Bottom,
        propertyChanged: OnDirectionChanged);

    public SheetDirection Direction
    {
        get => (SheetDirection)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
        nameof(IsOpen),
        typeof(bool),
        typeof(SheetView),
        false,
        BindingMode.TwoWay,
        propertyChanged: OnIsOpenChanged);

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public static readonly BindableProperty SheetContentProperty = BindableProperty.Create(
        nameof(SheetContent),
        typeof(View),
        typeof(SheetView),
        null,
        propertyChanged: OnSheetContentChanged);

    public View? SheetContent
    {
        get => (View?)GetValue(SheetContentProperty);
        set => SetValue(SheetContentProperty, value);
    }

    public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(
        nameof(HeaderTemplate),
        typeof(View),
        typeof(SheetView),
        null,
        propertyChanged: OnHeaderTemplateChanged);

    public View? HeaderTemplate
    {
        get => (View?)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public static readonly BindableProperty ShowHeaderWhenMinimizedProperty = BindableProperty.Create(
        nameof(ShowHeaderWhenMinimized),
        typeof(bool),
        typeof(SheetView),
        false,
        propertyChanged: OnShowHeaderWhenMinimizedChanged);

    public bool ShowHeaderWhenMinimized
    {
        get => (bool)GetValue(ShowHeaderWhenMinimizedProperty);
        set => SetValue(ShowHeaderWhenMinimizedProperty, value);
    }

    public static readonly BindableProperty DetentsProperty = BindableProperty.Create(
        nameof(Detents),
        typeof(ObservableCollection<DetentValue>),
        typeof(SheetView),
        null);

    public ObservableCollection<DetentValue> Detents
    {
        get => (ObservableCollection<DetentValue>)GetValue(DetentsProperty);
        set => SetValue(DetentsProperty, value);
    }

    public static readonly BindableProperty SheetBackgroundColorProperty = BindableProperty.Create(
        nameof(SheetBackgroundColor),
        typeof(Color),
        typeof(SheetView),
        Colors.White,
        propertyChanged: (b, _, n) => ((SheetView)b).sheetContainer.BackgroundColor = (Color)n);

    public Color SheetBackgroundColor
    {
        get => (Color)GetValue(SheetBackgroundColorProperty);
        set => SetValue(SheetBackgroundColorProperty, value);
    }

    public static readonly BindableProperty HandleColorProperty = BindableProperty.Create(
        nameof(HandleColor),
        typeof(Color),
        typeof(SheetView),
        Colors.Grey,
        propertyChanged: (b, _, n) => ((SheetView)b).dragHandle.Color = (Color)n);

    public Color HandleColor
    {
        get => (Color)GetValue(HandleColorProperty);
        set => SetValue(HandleColorProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(SheetCornerRadius),
        typeof(double),
        typeof(SheetView),
        16.0,
        propertyChanged: (b, _, n) => ((SheetView)b).UpdateCornerRadius((double)n));

    public double SheetCornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty HasBackdropProperty = BindableProperty.Create(
        nameof(HasBackdrop),
        typeof(bool),
        typeof(SheetView),
        true);

    public bool HasBackdrop
    {
        get => (bool)GetValue(HasBackdropProperty);
        set => SetValue(HasBackdropProperty, value);
    }

    public static readonly BindableProperty CloseOnBackdropTapProperty = BindableProperty.Create(
        nameof(CloseOnBackdropTap),
        typeof(bool),
        typeof(SheetView),
        true);

    public bool CloseOnBackdropTap
    {
        get => (bool)GetValue(CloseOnBackdropTapProperty);
        set => SetValue(CloseOnBackdropTapProperty, value);
    }

    public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(
        nameof(AnimationDuration),
        typeof(double),
        typeof(SheetView),
        DefaultAnimationDuration);

    public double AnimationDuration
    {
        get => (double)GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    public static readonly BindableProperty ExpandOnInputFocusProperty = BindableProperty.Create(
        nameof(ExpandOnInputFocus),
        typeof(bool),
        typeof(SheetView),
        true);

    public bool ExpandOnInputFocus
    {
        get => (bool)GetValue(ExpandOnInputFocusProperty);
        set => SetValue(ExpandOnInputFocusProperty, value);
    }

    public static readonly BindableProperty IsLockedProperty = BindableProperty.Create(
        nameof(IsLocked),
        typeof(bool),
        typeof(SheetView),
        false,
        propertyChanged: (b, _, n) => ((SheetView)b).OnIsLockedChanged((bool)n));

    public bool IsLocked
    {
        get => (bool)GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }

    public static readonly BindableProperty FitContentProperty = BindableProperty.Create(
        nameof(FitContent),
        typeof(bool),
        typeof(SheetView),
        false);

    public bool FitContent
    {
        get => (bool)GetValue(FitContentProperty);
        set => SetValue(FitContentProperty, value);
    }

    static void OnDirectionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sheet = (SheetView)bindable;
        sheet.UpdateLayoutForDirection();
    }

    static void OnHeaderTemplateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sheet = (SheetView)bindable;

        // Clear old parent references
        sheet.headerHost.Content = null;
        sheet.minimizedHost.Content = null;

        // UpdateMinimizedVisibility will place the view in the correct host
        sheet.UpdateMinimizedVisibility();

        // If not minimized, the header belongs in the sheet's headerHost
        if (!sheet.isMinimized && newValue is View view)
            sheet.headerHost.Content = view;
    }

    static void OnShowHeaderWhenMinimizedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((SheetView)bindable).UpdateMinimizedVisibility();
    }

    void UpdateMinimizedVisibility()
    {
        if (ShowHeaderWhenMinimized && HeaderTemplate != null && !IsOpen)
        {
            ShowMinimized();
        }
        else if (!IsOpen)
        {
            HideMinimized();
            IsVisible = false;
        }
    }

    void ShowMinimized()
    {
        isMinimized = true;
        rootGrid.IsVisible = false;

        // Move header content from inside the sheet to the standalone minimized host
        var headerView = HeaderTemplate;
        if (headerView != null)
        {
            headerHost.Content = null;
            minimizedHost.Content = headerView;
        }
        minimizedHost.IsVisible = true;

        // Shrink the SheetView to only the header area at the edge.
        // This way the SheetView doesn't cover the page and can't block touches.
        VerticalOptions = IsBottom ? LayoutOptions.End : LayoutOptions.Start;
        IsVisible = true;
    }

    void HideMinimized()
    {
        isMinimized = false;
        minimizedHost.IsVisible = false;

        // Restore full-page layout so the sheet can overlay the screen
        VerticalOptions = LayoutOptions.Fill;

        // Move header content back into the sheet
        var headerView = HeaderTemplate;
        if (headerView != null)
        {
            minimizedHost.Content = null;
            headerHost.Content = headerView;
        }
    }

    void OnHeaderHostSizeChanged(object? sender, EventArgs e)
    {
        // no-op: kept for future use
    }

    void OnIsLockedChanged(bool locked)
    {
        dragHandle.IsVisible = !locked;
        if (locked)
            sheetContainer.GestureRecognizers.Remove(panGesture);
        else if (!sheetContainer.GestureRecognizers.Contains(panGesture))
            sheetContainer.GestureRecognizers.Add(panGesture);
    }

    void UpdateLayoutForDirection()
    {
        if (IsBottom)
        {
            // Header at top (leading edge), handle, content
            sheetInnerGrid.RowDefinitions[0] = new RowDefinition(GridLength.Auto);
            sheetInnerGrid.RowDefinitions[1] = new RowDefinition(GridLength.Auto);
            sheetInnerGrid.RowDefinitions[2] = new RowDefinition(GridLength.Star);
            Grid.SetRow(headerHost, 0);
            Grid.SetRow(dragHandleContainer, 1);
            Grid.SetRow(scrollView, 2);
        }
        else
        {
            // Content, handle, header at bottom (leading edge for top sheet)
            sheetInnerGrid.RowDefinitions[0] = new RowDefinition(GridLength.Star);
            sheetInnerGrid.RowDefinitions[1] = new RowDefinition(GridLength.Auto);
            sheetInnerGrid.RowDefinitions[2] = new RowDefinition(GridLength.Auto);
            Grid.SetRow(scrollView, 0);
            Grid.SetRow(dragHandleContainer, 1);
            Grid.SetRow(headerHost, 2);
        }
        UpdateCornerRadius(SheetCornerRadius);
    }

    void UpdateCornerRadius(double radius)
    {
        var corners = IsBottom
            ? new CornerRadius(radius, radius, 0, 0)
            : new CornerRadius(0, 0, radius, radius);
        sheetContainer.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = corners
        };
    }


    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler<DetentValue>? DetentChanged;



    static void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sheet = (SheetView)bindable;
        if ((bool)newValue)
            _ = sheet.OpenAsync();
        else
            _ = sheet.CloseAsync();
    }

    static void OnSheetContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sheet = (SheetView)bindable;

        // Unhook old content's input views
        if (oldValue is View oldView)
            sheet.UnhookInputViews(oldView);

        sheet.contentHost.Content = newValue as View;

        // Hook new content's input views for keyboard handling
        if (newValue is View newView)
            sheet.HookInputViews(newView);
    }



    async Task OpenAsync()
    {
        if (isAnimating) return;
        isAnimating = true;

        // Move header back into the sheet if coming from minimized
        if (isMinimized)
            HideMinimized();

        IsVisible = true;
        rootGrid.IsVisible = true;

        // Wait for layout pass so rootGrid has a valid height
        if (rootGrid.Height <= 0)
            await WaitForLayoutAsync();

        availableHeight = rootGrid.Height > 0 ? rootGrid.Height : Height;
        if (availableHeight <= 0)
            availableHeight = Window?.Page is Page page ? page.Height : 800;
        previousAvailableHeight = availableHeight;

        // Start off-screen
        sheetContainer.TranslationY = GetOffScreenY();

        backdrop.Opacity = 0;
        backdrop.InputTransparent = !HasBackdrop;

        // Compute detent from content size when FitContent is enabled
        if (FitContent && contentHost.Content is View fitView)
        {
            var measured = fitView.Measure(rootGrid.Width, double.PositiveInfinity);
            var contentHeight = measured.Height + DragHandleHeight + 20; // 20px padding buffer
            if (headerHost.Content is View headerView)
                contentHeight += headerView.Measure(rootGrid.Width, double.PositiveInfinity).Height;
            var ratio = Math.Clamp(contentHeight / availableHeight, 0.1, 1.0);
            fitContentDetent = new DetentValue(ratio);
        }
        else
        {
            fitContentDetent = null;
        }

        // Animate to first detent
        currentDetentIndex = 0;
        var targetY = GetTranslationYForDetent(GetSortedDetents()[currentDetentIndex]);

        var animTasks = new List<Task>
        {
            sheetContainer.TranslateToAsync(0, targetY, (uint)AnimationDuration, Easing.CubicOut)
        };

        if (HasBackdrop)
            animTasks.Add(backdrop.FadeToAsync(BackdropMaxOpacity, (uint)AnimationDuration));

        await Task.WhenAll(animTasks);
        UpdateScrollEnabled();
        isAnimating = false;
        Opened?.Invoke(this, EventArgs.Empty);
    }

    async Task CloseAsync()
    {
        if (isAnimating) return;
        isAnimating = true;

        try
        {
            detentIndexBeforeKeyboard = -1;
            isKeyboardVisible = false;

            var shouldMinimize = ShowHeaderWhenMinimized && HeaderTemplate != null;
            var targetY = shouldMinimize ? GetMinimizedY() : GetOffScreenY();

            var animTasks = new List<Task>
            {
                sheetContainer.TranslateToAsync(0, targetY, (uint)AnimationDuration, Easing.CubicIn)
            };

            if (HasBackdrop)
                animTasks.Add(backdrop.FadeToAsync(0, (uint)AnimationDuration));

            await Task.WhenAll(animTasks);

            // Fully hide first to get a clean slate, then set up minimized if needed
            rootGrid.IsVisible = false;
            IsVisible = false;

            if (shouldMinimize)
            {
                ShowMinimized();
            }
            else
            {
                isMinimized = false;
            }
        }
        finally
        {
            isAnimating = false;
        }
        Closed?.Invoke(this, EventArgs.Empty);
    }



    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (isAnimating || IsLocked) return;

        if (!IsOpen) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                sheetStartTranslationY = sheetContainer.TranslationY;
                scrollView.IsEnabled = false;
                break;

            case GestureStatus.Running:
                var newY = sheetStartTranslationY + e.TotalY;
                var highestDetentY = GetTranslationYForDetent(GetSortedDetents().Last());
                var offScreen = GetOffScreenY();

                if (IsBottom)
                    newY = Math.Clamp(newY, highestDetentY - 20, offScreen);
                else
                    newY = Math.Clamp(newY, offScreen, highestDetentY + 20);

                sheetContainer.TranslationY = newY;

                if (HasBackdrop)
                {
                    var progress = GetBackdropProgress(newY);
                    backdrop.Opacity = Math.Clamp(progress * BackdropMaxOpacity, 0, BackdropMaxOpacity);
                }
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                _ = SnapToNearestDetentAsync(e.TotalY);
                break;
        }
    }

    async Task SnapToNearestDetentAsync(double totalPanY)
    {
        isAnimating = true;
        var currentY = sheetContainer.TranslationY;
        var sortedDetents = GetSortedDetents();

        // Dismiss = swiping away from the content; Expand = swiping toward content
        bool swipeDismiss, swipeExpand;
        if (IsBottom)
        {
            swipeDismiss = totalPanY > 50;
            swipeExpand = totalPanY < -50;
        }
        else
        {
            swipeDismiss = totalPanY < -50;
            swipeExpand = totalPanY > 50;
        }

        // If swiped past the lowest detent threshold, close
        var lowestDetentY = GetTranslationYForDetent(sortedDetents.First());
        bool pastDismissThreshold = IsBottom
            ? currentY > lowestDetentY + (availableHeight * 0.1)
            : currentY < lowestDetentY - (availableHeight * 0.1);

        if (swipeDismiss && pastDismissThreshold)
        {
            SetValue(IsOpenProperty, false);
            return;
        }

        // Find closest detent, biased by swipe direction
        var bestDetent = sortedDetents.First();
        var bestDistance = double.MaxValue;
        var bestIndex = 0;

        for (var i = 0; i < sortedDetents.Count; i++)
        {
            var detentY = GetTranslationYForDetent(sortedDetents[i]);
            var distance = Math.Abs(currentY - detentY);

            var detentMoreExpanded = Math.Abs(detentY) < Math.Abs(currentY);
            var detentMoreCollapsed = Math.Abs(detentY) > Math.Abs(currentY);

            if (swipeExpand && detentMoreExpanded)
                distance *= 0.5;
            else if (swipeDismiss && detentMoreCollapsed)
                distance *= 0.5;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestDetent = sortedDetents[i];
                bestIndex = i;
            }
        }

        currentDetentIndex = bestIndex;
        var targetY = GetTranslationYForDetent(bestDetent);

        var animTasks = new List<Task>
        {
            sheetContainer.TranslateToAsync(0, targetY, (uint)(AnimationDuration * 0.75), Easing.CubicOut)
        };

        if (HasBackdrop)
        {
            var progress = GetBackdropProgress(targetY);
            var targetOpacity = Math.Clamp(progress * BackdropMaxOpacity, 0, BackdropMaxOpacity);
            animTasks.Add(backdrop.FadeToAsync(targetOpacity, (uint)(AnimationDuration * 0.75)));
        }

        await Task.WhenAll(animTasks);
        UpdateScrollEnabled();
        isAnimating = false;
        DetentChanged?.Invoke(this, bestDetent);
    }



    void OnRootGridSizeChanged(object? sender, EventArgs e)
    {
        if (!IsOpen || availableHeight <= 0) return;

        var newHeight = rootGrid.Height;
        if (newHeight <= 0 || Math.Abs(newHeight - availableHeight) < 10) return;

        if (newHeight < previousAvailableHeight - 50)
        {
            isKeyboardVisible = true;
            availableHeight = newHeight;
            _ = RepositionForKeyboardAsync();
        }
        else if (newHeight > availableHeight + 50 && isKeyboardVisible)
        {
            isKeyboardVisible = false;
            availableHeight = newHeight;
            previousAvailableHeight = newHeight;
            _ = RestoreAfterKeyboardAsync();
        }
    }

    async Task RepositionForKeyboardAsync()
    {
        if (isAnimating) return;

        var sortedDetents = GetSortedDetents();
        var highestDetent = sortedDetents.Last();
        var targetY = GetTranslationYForDetent(highestDetent);

        isAnimating = true;
        await sheetContainer.TranslateToAsync(0, targetY, (uint)(AnimationDuration * 0.5), Easing.CubicOut);
        scrollView.IsEnabled = true;
        isAnimating = false;
    }

    async Task RestoreAfterKeyboardAsync()
    {
        if (isAnimating) return;

        var sortedDetents = GetSortedDetents();
        var targetIndex = detentIndexBeforeKeyboard >= 0 && detentIndexBeforeKeyboard < sortedDetents.Count
            ? detentIndexBeforeKeyboard
            : currentDetentIndex;
        detentIndexBeforeKeyboard = -1;

        var detent = sortedDetents[Math.Clamp(targetIndex, 0, sortedDetents.Count - 1)];
        var targetY = GetTranslationYForDetent(detent);

        isAnimating = true;
        await sheetContainer.TranslateToAsync(0, targetY, (uint)AnimationDuration, Easing.CubicOut);
        currentDetentIndex = targetIndex;
        UpdateScrollEnabled();
        isAnimating = false;
    }

    void HookInputViews(View view)
    {
        if (view is InputView input)
        {
            input.Focused += OnInputFocused;
            input.Unfocused += OnInputUnfocused;
        }

        if (view is Layout layout)
        {
            foreach (var child in layout.Children.OfType<View>())
                HookInputViews(child);
        }
        else if (view is ContentView cv && cv.Content is not null)
        {
            HookInputViews(cv.Content);
        }
        else if (view is ScrollView sv && sv.Content is View svContent)
        {
            HookInputViews(svContent);
        }
        else if (view is Border b && b.Content is View bContent)
        {
            HookInputViews(bContent);
        }
    }

    void UnhookInputViews(View view)
    {
        if (view is InputView input)
        {
            input.Focused -= OnInputFocused;
            input.Unfocused -= OnInputUnfocused;
        }

        if (view is Layout layout)
        {
            foreach (var child in layout.Children.OfType<View>())
                UnhookInputViews(child);
        }
        else if (view is ContentView cv && cv.Content is not null)
        {
            UnhookInputViews(cv.Content);
        }
        else if (view is ScrollView sv && sv.Content is View svContent)
        {
            UnhookInputViews(svContent);
        }
        else if (view is Border b && b.Content is View bContent)
        {
            UnhookInputViews(bContent);
        }
    }

    void OnInputFocused(object? sender, FocusEventArgs e)
    {
        if (!IsOpen || !ExpandOnInputFocus) return;

        if (detentIndexBeforeKeyboard < 0)
            detentIndexBeforeKeyboard = currentDetentIndex;

        var sortedDetents = GetSortedDetents();
        var highestDetent = sortedDetents.Last();
        _ = AnimateToDetentAsync(highestDetent);
    }

    void OnInputUnfocused(object? sender, FocusEventArgs e)
    {
        if (!IsOpen || !ExpandOnInputFocus || isKeyboardVisible) return;

        if (detentIndexBeforeKeyboard >= 0)
        {
            var sortedDetents = GetSortedDetents();
            var targetIndex = Math.Clamp(detentIndexBeforeKeyboard, 0, sortedDetents.Count - 1);
            detentIndexBeforeKeyboard = -1;
            _ = AnimateToDetentAsync(sortedDetents[targetIndex]);
        }
    }



    List<DetentValue> GetSortedDetents()
    {
        if (fitContentDetent.HasValue)
            return new List<DetentValue> { fitContentDetent.Value };

        var list = Detents?.OrderBy(d => d.Ratio).ToList()
            ?? new List<DetentValue> { DetentValue.Half };
        return list;
    }

    void UpdateScrollEnabled()
    {
        var sortedDetents = GetSortedDetents();
        var highestDetent = sortedDetents.Last();
        var highestY = GetTranslationYForDetent(highestDetent);
        scrollView.IsEnabled = Math.Abs(sheetContainer.TranslationY - highestY) < 5;
    }

    Task WaitForLayoutAsync()
    {
        var tcs = new TaskCompletionSource();
        void handler(object? s, EventArgs e)
        {
            if (rootGrid.Height > 0)
            {
                rootGrid.SizeChanged -= handler;
                tcs.TrySetResult();
            }
        }
        rootGrid.SizeChanged += handler;

        _ = Task.Delay(500).ContinueWith(_ =>
        {
            rootGrid.SizeChanged -= handler;
            tcs.TrySetResult();
        }, TaskScheduler.FromCurrentSynchronizationContext());

        return tcs.Task;
    }


    void OnBackdropTapped(object? sender, TappedEventArgs e)
    {
        if (IsLocked) return;
        if (CloseOnBackdropTap)
            IsOpen = false;
    }



    public async Task AnimateToDetentAsync(DetentValue detent)
    {
        if (isAnimating) return;
        isAnimating = true;

        var targetY = GetTranslationYForDetent(detent);
        var sortedDetents = GetSortedDetents();
        currentDetentIndex = sortedDetents.FindIndex(d => Math.Abs(d.Ratio - detent.Ratio) < 0.001);
        if (currentDetentIndex < 0) currentDetentIndex = 0;

        var animTasks = new List<Task>
        {
            sheetContainer.TranslateToAsync(0, targetY, (uint)AnimationDuration, Easing.CubicOut)
        };

        if (HasBackdrop)
        {
            var progress = GetBackdropProgress(targetY);
            var targetOpacity = Math.Clamp(progress * BackdropMaxOpacity, 0, BackdropMaxOpacity);
            animTasks.Add(backdrop.FadeToAsync(targetOpacity, (uint)AnimationDuration));
        }

        await Task.WhenAll(animTasks);
        UpdateScrollEnabled();
        isAnimating = false;
        DetentChanged?.Invoke(this, detent);
    }

}
