using Shiny.Maui.Controls.FloatingPanel;
using Shiny.Maui.Controls.Infrastructure;

namespace Shiny.Maui.Controls;

public class ImageViewer : ContentView
{
    const double MinScale = 1.0;
    const double DefaultMaxZoom = 5.0;
    const uint AnimationDuration = 250;

    // Thumbnail — visible when IsOpen=false
    readonly Image thumbnailImage;

    // Overlay elements — injected into OverlayHost when IsOpen=true
    readonly Grid overlayGrid;
    readonly BoxView backdrop;
    readonly Image overlayImage;
    View closeView;
    View? headerView;
    View? footerView;
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

    // Track where the overlay is hosted
    Layout? overlayParent;

    public ImageViewer()
    {
        // When no source is set, the viewer should not intercept touches
        InputTransparent = true;

        // Thumbnail: standard Image with tap-to-open
        thumbnailImage = new Image
        {
            Aspect = Aspect.AspectFit,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        var tapToOpen = new TapGestureRecognizer();
        tapToOpen.Tapped += (_, _) =>
        {
            if (!IsOpen && Source != null && OpenViewerOnTap)
                IsOpen = true;
        };
        thumbnailImage.GestureRecognizers.Add(tapToOpen);

        Content = thumbnailImage;

        // Build overlay (not in the visual tree until opened)
        backdrop = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0,
            InputTransparent = false
        };
        // Swallow touches on backdrop
        backdrop.GestureRecognizers.Add(new TapGestureRecognizer());

        overlayImage = new Image
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

        overlayImage.GestureRecognizers.Add(pinchGesture);
        overlayImage.GestureRecognizers.Add(doubleTapGesture);

        closeView = CreateDefaultCloseButton();

        overlayGrid = new Grid
        {
            InputTransparent = false,
            CascadeInputTransparent = false,
            Children = { backdrop, overlayImage, closeView }
        };
    }

    #region Bindable Properties

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(ImageSource),
        typeof(ImageViewer),
        null,
        propertyChanged: (b, _, n) =>
        {
            var viewer = (ImageViewer)b;
            var source = (ImageSource?)n;
            viewer.thumbnailImage.Source = source;
            viewer.overlayImage.Source = source;
            // Only intercept touches when there's an image to show
            viewer.InputTransparent = source == null;
        });

    public ImageSource? Source
    {
        get => (ImageSource?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly BindableProperty AspectProperty = BindableProperty.Create(
        nameof(Aspect),
        typeof(Aspect),
        typeof(ImageViewer),
        Aspect.AspectFit,
        propertyChanged: (b, _, n) => ((ImageViewer)b).thumbnailImage.Aspect = (Aspect)n);

    public Aspect Aspect
    {
        get => (Aspect)GetValue(AspectProperty);
        set => SetValue(AspectProperty, value);
    }

    public static readonly BindableProperty OverlayAspectProperty = BindableProperty.Create(
        nameof(OverlayAspect),
        typeof(Aspect),
        typeof(ImageViewer),
        Aspect.AspectFit,
        propertyChanged: (b, _, n) => ((ImageViewer)b).overlayImage.Aspect = (Aspect)n);

    public Aspect OverlayAspect
    {
        get => (Aspect)GetValue(OverlayAspectProperty);
        set => SetValue(OverlayAspectProperty, value);
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

    public static readonly BindableProperty CloseButtonTemplateProperty = BindableProperty.Create(
        nameof(CloseButtonTemplate),
        typeof(DataTemplate),
        typeof(ImageViewer),
        null,
        propertyChanged: (b, _, _) => ((ImageViewer)b).ApplyCloseButtonTemplate());

    public DataTemplate? CloseButtonTemplate
    {
        get => (DataTemplate?)GetValue(CloseButtonTemplateProperty);
        set => SetValue(CloseButtonTemplateProperty, value);
    }

    public static readonly BindableProperty HeaderTemplateProperty = BindableProperty.Create(
        nameof(HeaderTemplate),
        typeof(DataTemplate),
        typeof(ImageViewer),
        null,
        propertyChanged: (b, _, _) => ((ImageViewer)b).ApplyHeaderTemplate());

    public DataTemplate? HeaderTemplate
    {
        get => (DataTemplate?)GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public static readonly BindableProperty FooterTemplateProperty = BindableProperty.Create(
        nameof(FooterTemplate),
        typeof(DataTemplate),
        typeof(ImageViewer),
        null,
        propertyChanged: (b, _, _) => ((ImageViewer)b).ApplyFooterTemplate());

    public DataTemplate? FooterTemplate
    {
        get => (DataTemplate?)GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    public static readonly BindableProperty UseFeedbackProperty = BindableProperty.Create(
        nameof(UseFeedback),
        typeof(bool),
        typeof(ImageViewer),
        true);

    public bool UseFeedback
    {
        get => (bool)GetValue(UseFeedbackProperty);
        set => SetValue(UseFeedbackProperty, value);
    }

    public static readonly BindableProperty OpenViewerOnTapProperty = BindableProperty.Create(
        nameof(OpenViewerOnTap),
        typeof(bool),
        typeof(ImageViewer),
        true);

    public bool OpenViewerOnTap
    {
        get => (bool)GetValue(OpenViewerOnTapProperty);
        set => SetValue(OpenViewerOnTapProperty, value);
    }

    #endregion

    #region Template Application

    void ApplyCloseButtonTemplate()
    {
        overlayGrid.Children.Remove(closeView);

        if (CloseButtonTemplate != null)
        {
            var content = CloseButtonTemplate.CreateContent();
            if (content is View view)
            {
                view.HorizontalOptions = LayoutOptions.End;
                view.VerticalOptions = LayoutOptions.Start;
                var tap = new TapGestureRecognizer();
                tap.Tapped += (_, _) => IsOpen = false;
                view.GestureRecognizers.Add(tap);
                closeView = view;
            }
        }
        else
        {
            closeView = CreateDefaultCloseButton();
        }

        overlayGrid.Children.Add(closeView);
    }

    void ApplyHeaderTemplate()
    {
        if (headerView != null)
            overlayGrid.Children.Remove(headerView);

        if (HeaderTemplate != null)
        {
            var content = HeaderTemplate.CreateContent();
            if (content is View view)
            {
                view.VerticalOptions = LayoutOptions.Start;
                headerView = view;
                overlayGrid.Children.Add(headerView);
            }
        }
        else
        {
            headerView = null;
        }
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (footerView != null)
            footerView.BindingContext = BindingContext;
        if (headerView != null)
            headerView.BindingContext = BindingContext;
        overlayGrid.BindingContext = BindingContext;
    }

    void ApplyFooterTemplate()
    {
        if (footerView != null)
            overlayGrid.Children.Remove(footerView);

        if (FooterTemplate != null)
        {
            var content = FooterTemplate.CreateContent();
            if (content is View view)
            {
                view.VerticalOptions = LayoutOptions.End;
                view.BindingContext = BindingContext;
                footerView = view;
                overlayGrid.Children.Add(footerView);
            }
        }
        else
        {
            footerView = null;
        }
    }

    Button CreateDefaultCloseButton()
    {
        var btn = new Button
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
        btn.Clicked += (_, _) => IsOpen = false;
        return btn;
    }

    #endregion

    #region Open / Close

    Layout? FindOverlayParent()
    {
        // Walk up the tree looking for an OverlayHost
        Element? current = Parent;
        while (current is not null)
        {
            if (current is OverlayHost host)
                return host;
            current = current.Parent;
        }

        // Fallback: find the page's root layout
        current = Parent;
        while (current is not null)
        {
            if (current is Page page)
            {
                // ShinyContentPage — use its OverlayHost
                if (page is ShinyContentPage scp)
                    return scp.OverlayHost;

                // Regular page — try to find a Grid as root content
                if (page is ContentPage cp && cp.Content is Grid grid)
                    return grid;

                break;
            }
            current = current.Parent;
        }

        return null;
    }

    async Task OpenAsync()
    {
        if (isAnimating) return;
        isAnimating = true;

        ResetTransform();

        // Sync source to overlay image
        overlayImage.Source = Source;

        // Find host and inject overlay
        overlayParent = FindOverlayParent()
            ?? throw new InvalidOperationException(
                "ImageViewer could not find a suitable parent layout. " +
                "Place it inside an OverlayHost, ShinyContentPage, or a Grid that is the root content of a ContentPage.");
        overlayGrid.BindingContext = BindingContext;
        overlayParent.Children.Add(overlayGrid);

        var fadeTargets = new List<VisualElement> { backdrop, overlayImage, closeView };
        if (headerView != null) fadeTargets.Add(headerView);
        if (footerView != null) fadeTargets.Add(footerView);

        foreach (var v in fadeTargets) v.Opacity = 0;
        await Task.WhenAll(fadeTargets.Select(v => v.FadeTo(1, AnimationDuration)));

        isAnimating = false;
    }

    async Task CloseAsync()
    {
        if (isAnimating) return;
        isAnimating = true;

        var fadeTargets = new List<VisualElement> { backdrop, overlayImage, closeView };
        if (headerView != null) fadeTargets.Add(headerView);
        if (footerView != null) fadeTargets.Add(footerView);

        await Task.WhenAll(fadeTargets.Select(v => v.FadeTo(0, AnimationDuration)));

        // Remove overlay from host
        overlayParent?.Children.Remove(overlayGrid);
        overlayParent = null;

        ResetTransform();
        isAnimating = false;
    }

    void ResetTransform()
    {
        currentScale = 1;
        xOffset = 0;
        yOffset = 0;
        overlayImage.Scale = 1;
        overlayImage.TranslationX = 0;
        overlayImage.TranslationY = 0;
        overlayImage.GestureRecognizers.Remove(panGesture);
    }

    #endregion

    #region Gestures

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

                var pinchX = (e.ScaleOrigin.X - 0.5) * overlayImage.Width;
                var pinchY = (e.ScaleOrigin.Y - 0.5) * overlayImage.Height;
                var scaleDelta = currentScale - startScale;

                var targetX = xOffset - pinchX * scaleDelta;
                var targetY = yOffset - pinchY * scaleDelta;

                overlayImage.TranslationX = ClampX(targetX);
                overlayImage.TranslationY = ClampY(targetY);
                overlayImage.Scale = currentScale;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                isPinching = false;
                xOffset = overlayImage.TranslationX;
                yOffset = overlayImage.TranslationY;

                if (currentScale <= MinScale)
                    _ = AnimateResetAsync();
                else if (!overlayImage.GestureRecognizers.Contains(panGesture))
                    overlayImage.GestureRecognizers.Add(panGesture);
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
                overlayImage.TranslationX = ClampX(startX + e.TotalX);
                overlayImage.TranslationY = ClampY(startY + e.TotalY);
                break;

            case GestureStatus.Completed:
                xOffset = overlayImage.TranslationX;
                yOffset = overlayImage.TranslationY;
                break;
        }
    }

    void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (isAnimating) return;

        if (UseFeedback)
            FeedbackHelper.Execute(typeof(ImageViewer), "DoubleTapped");

        if (currentScale > MinScale)
            _ = AnimateResetAsync();
        else
            _ = AnimateZoomInAsync(e);
    }

    async Task AnimateZoomInAsync(TappedEventArgs e)
    {
        isAnimating = true;

        var targetScale = Math.Min(2.5, MaxZoom);
        var point = e.GetPosition(overlayImage);

        double tx = 0, ty = 0;
        if (point.HasValue)
        {
            tx = -(point.Value.X - overlayImage.Width / 2) * (targetScale - 1);
            ty = -(point.Value.Y - overlayImage.Height / 2) * (targetScale - 1);
        }

        currentScale = targetScale;
        tx = ClampX(tx);
        ty = ClampY(ty);
        xOffset = tx;
        yOffset = ty;

        await Task.WhenAll(
            overlayImage.ScaleTo(targetScale, AnimationDuration, Easing.CubicOut),
            overlayImage.TranslateTo(tx, ty, AnimationDuration, Easing.CubicOut)
        );

        if (!overlayImage.GestureRecognizers.Contains(panGesture))
            overlayImage.GestureRecognizers.Add(panGesture);

        isAnimating = false;
    }

    async Task AnimateResetAsync()
    {
        isAnimating = true;
        overlayImage.GestureRecognizers.Remove(panGesture);

        await Task.WhenAll(
            overlayImage.ScaleTo(1, AnimationDuration, Easing.CubicOut),
            overlayImage.TranslateTo(0, 0, AnimationDuration, Easing.CubicOut)
        );

        currentScale = 1;
        xOffset = 0;
        yOffset = 0;
        isAnimating = false;
    }

    double ClampX(double x)
    {
        if (currentScale <= MinScale) return 0;
        var maxX = overlayImage.Width * (currentScale - 1) / 2;
        return Math.Clamp(x, -maxX, maxX);
    }

    double ClampY(double y)
    {
        if (currentScale <= MinScale) return 0;
        var maxY = overlayImage.Height * (currentScale - 1) / 2;
        return Math.Clamp(y, -maxY, maxY);
    }

    #endregion
}
