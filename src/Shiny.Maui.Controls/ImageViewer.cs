namespace Shiny.Maui.Controls;

public class ImageViewer : ContentView
{
    const double MinScale = 1.0;
    const double MaxScale = 5.0;
    const uint AnimationDuration = 250;

    readonly Grid rootGrid;
    readonly BoxView backdrop;
    readonly Image image;
    readonly ContentView gestureLayer;
    readonly Button closeButton;
    readonly TapGestureRecognizer doubleTapGesture;
    readonly PinchGestureRecognizer pinchGesture;
    readonly PanGestureRecognizer panGesture;

    double currentScale = 1;
    double startScale = 1;
    double panX;
    double panY;
    double panXStart;
    double panYStart;
    double startX;
    double startY;
    bool isAnimating;
    bool isPinching;

    public ImageViewer()
    {
        IsVisible = false;
        InputTransparent = false;

        backdrop = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0
        };

        image = new Image
        {
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        // Full-screen transparent layer that captures all gestures
        gestureLayer = new ContentView
        {
            BackgroundColor = Colors.Transparent,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        doubleTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        doubleTapGesture.Tapped += OnDoubleTapped;

        pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += OnPinchUpdated;

        panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;

        gestureLayer.GestureRecognizers.Add(doubleTapGesture);
        gestureLayer.GestureRecognizers.Add(pinchGesture);

        closeButton = new Button
        {
            Text = "\u2715",
            FontSize = 20,
            TextColor = Colors.White,
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5),
            CornerRadius = 20,
            WidthRequest = 40,
            HeightRequest = 40,
            Padding = 0,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 50, 16, 0)
        };
        closeButton.Clicked += (_, _) => IsOpen = false;

        rootGrid = new Grid
        {
            Children = { backdrop, image, gestureLayer, closeButton }
        };

        Content = rootGrid;
    }

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(ImageSource),
        typeof(ImageViewer),
        null,
        propertyChanged: (b, _, n) => ((ImageViewer)b).image.Source = (ImageSource?)n);

    public ImageSource? Source
    {
        get => (ImageSource?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
        nameof(IsOpen),
        typeof(bool),
        typeof(ImageViewer),
        false,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) =>
        {
            var viewer = (ImageViewer)b;
            if ((bool)n)
                _ = viewer.OpenAsync();
            else
                _ = viewer.CloseAsync();
        });

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    async Task OpenAsync()
    {
        if (isAnimating) return;
        isAnimating = true;

        ResetTransform();
        IsVisible = true;
        backdrop.Opacity = 0;
        image.Opacity = 0;
        closeButton.Opacity = 0;

        await Task.WhenAll(
            backdrop.FadeTo(1, AnimationDuration),
            image.FadeTo(1, AnimationDuration),
            closeButton.FadeTo(1, AnimationDuration)
        );

        isAnimating = false;
    }

    async Task CloseAsync()
    {
        if (isAnimating) return;
        isAnimating = true;

        await Task.WhenAll(
            backdrop.FadeTo(0, AnimationDuration),
            image.FadeTo(0, AnimationDuration),
            closeButton.FadeTo(0, AnimationDuration)
        );

        IsVisible = false;
        ResetTransform();
        isAnimating = false;
    }

    void ResetTransform()
    {
        currentScale = 1;
        panX = 0;
        panY = 0;
        image.Scale = 1;
        image.TranslationX = 0;
        image.TranslationY = 0;
        gestureLayer.GestureRecognizers.Remove(panGesture);
    }

    void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                isPinching = true;
                gestureLayer.GestureRecognizers.Remove(panGesture);
                gestureLayer.GestureRecognizers.Remove(doubleTapGesture);
                startScale = currentScale;
                panXStart = panX;
                panYStart = panY;
                break;

            case GestureStatus.Running:
                var newScale = Math.Clamp(startScale * e.Scale, MinScale, MaxScale);
                var scaleDelta = newScale - startScale;

                // Use gestureLayer dimensions (full screen) for origin calculation
                panX = panXStart - (e.ScaleOrigin.X - 0.5) * gestureLayer.Width * scaleDelta;
                panY = panYStart - (e.ScaleOrigin.Y - 0.5) * gestureLayer.Height * scaleDelta;

                currentScale = newScale;
                ClampPan();

                image.Scale = currentScale;
                image.TranslationX = panX;
                image.TranslationY = panY;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                isPinching = false;
                if (!gestureLayer.GestureRecognizers.Contains(doubleTapGesture))
                    gestureLayer.GestureRecognizers.Add(doubleTapGesture);
                if (currentScale <= MinScale)
                    _ = AnimateResetAsync();
                else if (!gestureLayer.GestureRecognizers.Contains(panGesture))
                    gestureLayer.GestureRecognizers.Add(panGesture);
                break;
        }
    }

    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (isPinching || currentScale <= MinScale) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                startX = panX;
                startY = panY;
                break;

            case GestureStatus.Running:
                panX = startX + e.TotalX;
                panY = startY + e.TotalY;
                ClampPan();

                image.TranslationX = panX;
                image.TranslationY = panY;
                break;
        }
    }

    void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (isAnimating) return;

        if (currentScale > MinScale)
            _ = AnimateResetAsync();
        else
            _ = AnimateZoomInAsync(e);
    }

    async Task AnimateResetAsync()
    {
        isAnimating = true;
        gestureLayer.GestureRecognizers.Remove(panGesture);

        await Task.WhenAll(
            image.ScaleTo(1, AnimationDuration, Easing.CubicOut),
            image.TranslateTo(0, 0, AnimationDuration, Easing.CubicOut)
        );

        currentScale = 1;
        panX = 0;
        panY = 0;
        isAnimating = false;
    }

    async Task AnimateZoomInAsync(TappedEventArgs e)
    {
        isAnimating = true;

        var targetScale = 2.5;
        var point = e.GetPosition(gestureLayer);

        if (point.HasValue)
        {
            panX = -(point.Value.X - gestureLayer.Width / 2) * (targetScale - 1);
            panY = -(point.Value.Y - gestureLayer.Height / 2) * (targetScale - 1);
        }

        currentScale = targetScale;
        ClampPan();

        await Task.WhenAll(
            image.ScaleTo(targetScale, AnimationDuration, Easing.CubicOut),
            image.TranslateTo(panX, panY, AnimationDuration, Easing.CubicOut)
        );

        if (!gestureLayer.GestureRecognizers.Contains(panGesture))
            gestureLayer.GestureRecognizers.Add(panGesture);

        isAnimating = false;
    }

    void ClampPan()
    {
        if (currentScale <= MinScale)
        {
            panX = 0;
            panY = 0;
            return;
        }

        var maxX = (image.Width * (currentScale - 1)) / 2;
        var maxY = (image.Height * (currentScale - 1)) / 2;
        panX = Math.Clamp(panX, -maxX, maxX);
        panY = Math.Clamp(panY, -maxY, maxY);
    }
}
