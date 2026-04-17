using System.Collections.ObjectModel;

namespace Shiny.Maui.Controls;

[ContentProperty(nameof(SheetContent))]
public class BottomSheetView : ContentView
{
    const double DefaultAnimationDuration = 250;
    const double DragHandleHeight = 30;
    const double BackdropMaxOpacity = 0.5;

    readonly Grid rootGrid;
    readonly BoxView backdrop;
    readonly Border sheetContainer;
    readonly Grid sheetInnerGrid;
    readonly BoxView dragHandle;
    readonly ScrollView scrollView;
    readonly ContentView contentHost;
    readonly PanGestureRecognizer panGesture;

    double sheetStartTranslationY;
    double availableHeight;
    double previousAvailableHeight;
    bool isAnimating;
    int currentDetentIndex;
    int detentIndexBeforeKeyboard = -1;
    bool isKeyboardVisible;
    DetentValue? fitContentDetent;

    public BottomSheetView()
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

        // Drag handle container
        var dragHandleContainer = new Grid
        {
            HeightRequest = DragHandleHeight,
            BackgroundColor = Colors.Transparent
        };
        dragHandleContainer.Children.Add(dragHandle);

        // Pan gesture on the whole sheet
        panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;

        // Inner grid for layout
        sheetInnerGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star)
            }
        };
        sheetInnerGrid.Children.Add(dragHandleContainer);
        Grid.SetRow(dragHandleContainer, 0);
        sheetInnerGrid.Children.Add(scrollView);
        Grid.SetRow(scrollView, 1);

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

        // Root grid stacks backdrop + sheet
        rootGrid = new Grid();
        rootGrid.Children.Add(backdrop);
        rootGrid.Children.Add(sheetContainer);
        rootGrid.SizeChanged += OnRootGridSizeChanged;

        Content = rootGrid;

        // Default detents
        Detents = new ObservableCollection<DetentValue>
        {
            DetentValue.Quarter,
            DetentValue.Half,
            DetentValue.Full
        };
    }


    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
        nameof(IsOpen),
        typeof(bool),
        typeof(BottomSheetView),
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
        typeof(BottomSheetView),
        null,
        propertyChanged: OnSheetContentChanged);

    public View? SheetContent
    {
        get => (View?)GetValue(SheetContentProperty);
        set => SetValue(SheetContentProperty, value);
    }

    public static readonly BindableProperty DetentsProperty = BindableProperty.Create(
        nameof(Detents),
        typeof(ObservableCollection<DetentValue>),
        typeof(BottomSheetView),
        null);

    public ObservableCollection<DetentValue> Detents
    {
        get => (ObservableCollection<DetentValue>)GetValue(DetentsProperty);
        set => SetValue(DetentsProperty, value);
    }

    public static readonly BindableProperty SheetBackgroundColorProperty = BindableProperty.Create(
        nameof(SheetBackgroundColor),
        typeof(Color),
        typeof(BottomSheetView),
        Colors.White,
        propertyChanged: (b, _, n) => ((BottomSheetView)b).sheetContainer.BackgroundColor = (Color)n);

    public Color SheetBackgroundColor
    {
        get => (Color)GetValue(SheetBackgroundColorProperty);
        set => SetValue(SheetBackgroundColorProperty, value);
    }

    public static readonly BindableProperty HandleColorProperty = BindableProperty.Create(
        nameof(HandleColor),
        typeof(Color),
        typeof(BottomSheetView),
        Colors.Grey,
        propertyChanged: (b, _, n) => ((BottomSheetView)b).dragHandle.Color = (Color)n);

    public Color HandleColor
    {
        get => (Color)GetValue(HandleColorProperty);
        set => SetValue(HandleColorProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(SheetCornerRadius),
        typeof(double),
        typeof(BottomSheetView),
        16.0,
        propertyChanged: OnCornerRadiusChanged);

    public double SheetCornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty HasBackdropProperty = BindableProperty.Create(
        nameof(HasBackdrop),
        typeof(bool),
        typeof(BottomSheetView),
        true);

    public bool HasBackdrop
    {
        get => (bool)GetValue(HasBackdropProperty);
        set => SetValue(HasBackdropProperty, value);
    }

    public static readonly BindableProperty CloseOnBackdropTapProperty = BindableProperty.Create(
        nameof(CloseOnBackdropTap),
        typeof(bool),
        typeof(BottomSheetView),
        true);

    public bool CloseOnBackdropTap
    {
        get => (bool)GetValue(CloseOnBackdropTapProperty);
        set => SetValue(CloseOnBackdropTapProperty, value);
    }

    public static readonly BindableProperty AnimationDurationProperty = BindableProperty.Create(
        nameof(AnimationDuration),
        typeof(double),
        typeof(BottomSheetView),
        DefaultAnimationDuration);

    public double AnimationDuration
    {
        get => (double)GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    public static readonly BindableProperty ExpandOnInputFocusProperty = BindableProperty.Create(
        nameof(ExpandOnInputFocus),
        typeof(bool),
        typeof(BottomSheetView),
        true);

    public bool ExpandOnInputFocus
    {
        get => (bool)GetValue(ExpandOnInputFocusProperty);
        set => SetValue(ExpandOnInputFocusProperty, value);
    }

    public static readonly BindableProperty IsLockedProperty = BindableProperty.Create(
        nameof(IsLocked),
        typeof(bool),
        typeof(BottomSheetView),
        false,
        propertyChanged: (b, _, n) => ((BottomSheetView)b).OnIsLockedChanged((bool)n));

    public bool IsLocked
    {
        get => (bool)GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }

    public static readonly BindableProperty FitContentProperty = BindableProperty.Create(
        nameof(FitContent),
        typeof(bool),
        typeof(BottomSheetView),
        false);

    public bool FitContent
    {
        get => (bool)GetValue(FitContentProperty);
        set => SetValue(FitContentProperty, value);
    }

    void OnIsLockedChanged(bool locked)
    {
        dragHandle.IsVisible = !locked;
        if (locked)
            sheetContainer.GestureRecognizers.Remove(panGesture);
        else if (!sheetContainer.GestureRecognizers.Contains(panGesture))
            sheetContainer.GestureRecognizers.Add(panGesture);
    }



    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler<DetentValue>? DetentChanged;



    static void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sheet = (BottomSheetView)bindable;
        if ((bool)newValue)
            _ = sheet.OpenAsync();
        else
            _ = sheet.CloseAsync();
    }

    static void OnSheetContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sheet = (BottomSheetView)bindable;

        // Unhook old content's input views
        if (oldValue is View oldView)
            sheet.UnhookInputViews(oldView);

        sheet.contentHost.Content = newValue as View;

        // Hook new content's input views for keyboard handling
        if (newValue is View newView)
            sheet.HookInputViews(newView);
    }

    static void OnCornerRadiusChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var sheet = (BottomSheetView)bindable;
        var radius = (double)newValue;
        var corners = new CornerRadius(radius, radius, 0, 0);
        sheet.sheetContainer.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = corners
        };
    }



    async Task OpenAsync()
    {
        if (isAnimating) return;
        isAnimating = true;

        IsVisible = true;

        // Wait for layout pass so rootGrid has a valid height
        if (rootGrid.Height <= 0)
            await WaitForLayoutAsync();

        availableHeight = rootGrid.Height > 0 ? rootGrid.Height : Height;
        if (availableHeight <= 0)
            availableHeight = Window?.Page is Page page ? page.Height : 800;
        previousAvailableHeight = availableHeight;

        // Start off-screen
        sheetContainer.TranslationY = availableHeight;
        backdrop.Opacity = 0;
        backdrop.InputTransparent = !HasBackdrop;

        // Compute detent from content size when FitContent is enabled
        if (FitContent && contentHost.Content is View fitView)
        {
            var measured = fitView.Measure(rootGrid.Width, double.PositiveInfinity);
            var contentHeight = measured.Height + DragHandleHeight + 20; // 20px padding buffer
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

        detentIndexBeforeKeyboard = -1;
        isKeyboardVisible = false;

        var animTasks = new List<Task>
        {
            sheetContainer.TranslateToAsync(0, availableHeight, (uint)AnimationDuration, Easing.CubicIn)
        };

        if (HasBackdrop)
            animTasks.Add(backdrop.FadeToAsync(0, (uint)AnimationDuration));

        await Task.WhenAll(animTasks);

        IsVisible = false;
        isAnimating = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }



    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (isAnimating || IsLocked) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                sheetStartTranslationY = sheetContainer.TranslationY;
                scrollView.IsEnabled = false;
                break;

            case GestureStatus.Running:
                var newY = sheetStartTranslationY + e.TotalY;
                var highestDetentY = GetTranslationYForDetent(GetSortedDetents().Last());
                var lowestY = availableHeight;
                newY = Math.Clamp(newY, highestDetentY - 20, lowestY);
                sheetContainer.TranslationY = newY;

                if (HasBackdrop)
                {
                    var progress = 1.0 - (newY / availableHeight);
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

        var swipeDown = totalPanY > 50;
        var swipeUp = totalPanY < -50;

        // If swiped down past the lowest detent threshold, close
        var lowestDetentY = GetTranslationYForDetent(sortedDetents.First());
        if (swipeDown && currentY > lowestDetentY + (availableHeight * 0.1))
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

            if (swipeUp && detentY < currentY)
                distance *= 0.5;
            else if (swipeDown && detentY > currentY)
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
            var progress = 1.0 - (targetY / availableHeight);
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

        // Significant height decrease = keyboard appeared (Android AdjustResize)
        if (newHeight < previousAvailableHeight - 50)
        {
            isKeyboardVisible = true;
            availableHeight = newHeight;
            _ = RepositionForKeyboardAsync();
        }
        // Height restored = keyboard dismissed
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

        // Move sheet to the highest detent within the new (smaller) available height
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

        // Restore to pre-keyboard detent
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

        // Remember current detent so we can restore later
        if (detentIndexBeforeKeyboard < 0)
            detentIndexBeforeKeyboard = currentDetentIndex;

        // Expand to highest detent and enable scrolling so the ScrollView
        // can keep the focused input visible
        var sortedDetents = GetSortedDetents();
        var highestDetent = sortedDetents.Last();
        _ = AnimateToDetentAsync(highestDetent);
    }

    void OnInputUnfocused(object? sender, FocusEventArgs e)
    {
        if (!IsOpen || !ExpandOnInputFocus || isKeyboardVisible) return;

        // Restore previous detent if keyboard isn't still showing
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

    double GetTranslationYForDetent(DetentValue detent)
    {
        return availableHeight * (1.0 - detent.Ratio);
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

        // Safety timeout so we never hang
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
            var progress = 1.0 - (targetY / availableHeight);
            var targetOpacity = Math.Clamp(progress * BackdropMaxOpacity, 0, BackdropMaxOpacity);
            animTasks.Add(backdrop.FadeToAsync(targetOpacity, (uint)AnimationDuration));
        }

        await Task.WhenAll(animTasks);
        UpdateScrollEnabled();
        isAnimating = false;
        DetentChanged?.Invoke(this, detent);
    }

}