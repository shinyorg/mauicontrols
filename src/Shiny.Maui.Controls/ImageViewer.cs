namespace Shiny.Maui.Controls;

public class ImageViewer : ContentView
{
    const double MinScale = 1.0;
    const double DefaultMaxZoom = 5.0;
    const uint AnimationDuration = 250;

    readonly Grid rootGrid;
    readonly BoxView backdrop;
    readonly Image image;
    readonly Button closeButton;
    readonly TapGestureRecognizer doubleTapGesture;
    readonly PinchGestureRecognizer pinchGesture;
    readonly PanGestureRecognizer panGesture;

    double currentScale = 1;
    double startScale = 1;
    double xOffset;
    double yOffset;
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
            Opacity = 0,
            InputTransparent = false
        };

        image = new Image
        {
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            InputTransparent = false
        };

        doubleTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        doubleTapGesture.Tapped += OnDoubleTapped;

        pinchGesture = new PinchGestureRecognizer();
        pinchGesture.PinchUpdated += OnPinchUpdated;

        panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;

        // Pinch and double-tap directly on the image.
        // Pan is added only after zoom completes.
        image.GestureRecognizers.Add(pinchGesture);
        image.GestureRecognizers.Add(doubleTapGesture);

        // Swallow touches on backdrop so nothing falls
        // through to the page behind the overlay
        backdrop.GestureRecognizers.Add(new TapGestureRecognizer());

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
            InputTransparent = false,
            CascadeInputTransparent = false,
            Children = { backdrop, image, closeButton }
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

    public static readonly BindableProperty MaxZoomProperty = BindableProperty.Create(
        nameof(MaxZoom),
        typeof(double),
        typeof(ImageViewer),
        DefaultMaxZoom);

    public double MaxZoom
    {
        get => (double)GetValue(MaxZoomProperty);
        set => SetValue(MaxZoomProperty, value);
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
        xOffset = 0;
        yOffset = 0;
        image.Scale = 1;
        image.TranslationX = 0;
        image.TranslationY = 0;
        image.GestureRecognizers.Remove(panGesture);
    }

    void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                isPinching = true;
                startScale = currentScale;
                break;

            case GestureStatus.Running:
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Clamp(currentScale, MinScale, MaxZoom);

                // ScaleOrigin is 0-1 relative to the view that owns the gesture (image).
                // With center anchor, offset the translation so the pinch origin stays fixed.
                var pinchX = (e.ScaleOrigin.X - 0.5) * image.Width;
                var pinchY = (e.ScaleOrigin.Y - 0.5) * image.Height;
                var scaleDelta = currentScale - startScale;

                var targetX = xOffset - pinchX * scaleDelta;
                var targetY = yOffset - pinchY * scaleDelta;

                image.TranslationX = ClampX(targetX);
                image.TranslationY = ClampY(targetY);
                image.Scale = currentScale;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                isPinching = false;
                xOffset = image.TranslationX;
                yOffset = image.TranslationY;

                if (currentScale <= MinScale)
                    _ = AnimateResetAsync();
                else if (!image.GestureRecognizers.Contains(panGesture))
                    image.GestureRecognizers.Add(panGesture);
                break;
        }
    }

    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (isPinching || currentScale <= MinScale) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                startX = xOffset;
                startY = yOffset;
                break;

            case GestureStatus.Running:
                image.TranslationX = ClampX(startX + e.TotalX);
                image.TranslationY = ClampY(startY + e.TotalY);
                break;

            case GestureStatus.Completed:
                xOffset = image.TranslationX;
                yOffset = image.TranslationY;
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

    async Task AnimateZoomInAsync(TappedEventArgs e)
    {
        isAnimating = true;

        var targetScale = Math.Min(2.5, MaxZoom);
        var point = e.GetPosition(image);

        double tx = 0, ty = 0;
        if (point.HasValue)
        {
            // With center anchor: translate so tap point ends up at viewport center.
            // screenPos = viewCenter + (tapPos - viewCenter) * scale + translation
            // We want screenPos = viewCenter, so:
            // translation = -(tapPos - viewCenter) * scale
            tx = -(point.Value.X - image.Width / 2) * (targetScale - 1);
            ty = -(point.Value.Y - image.Height / 2) * (targetScale - 1);
        }

        currentScale = targetScale;
        tx = ClampX(tx);
        ty = ClampY(ty);
        xOffset = tx;
        yOffset = ty;

        await Task.WhenAll(
            image.ScaleTo(targetScale, AnimationDuration, Easing.CubicOut),
            image.TranslateTo(tx, ty, AnimationDuration, Easing.CubicOut)
        );

        if (!image.GestureRecognizers.Contains(panGesture))
            image.GestureRecognizers.Add(panGesture);

        isAnimating = false;
    }

    async Task AnimateResetAsync()
    {
        isAnimating = true;
        image.GestureRecognizers.Remove(panGesture);

        await Task.WhenAll(
            image.ScaleTo(1, AnimationDuration, Easing.CubicOut),
            image.TranslateTo(0, 0, AnimationDuration, Easing.CubicOut)
        );

        currentScale = 1;
        xOffset = 0;
        yOffset = 0;
        isAnimating = false;
    }

    double ClampX(double x)
    {
        if (currentScale <= MinScale) return 0;
        var maxX = image.Width * (currentScale - 1) / 2;
        return Math.Clamp(x, -maxX, maxX);
    }

    double ClampY(double y)
    {
        if (currentScale <= MinScale) return 0;
        var maxY = image.Height * (currentScale - 1) / 2;
        return Math.Clamp(y, -maxY, maxY);
    }
}
