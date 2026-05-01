namespace Shiny.Maui.Controls;

/// <summary>
/// A view that applies a frosted glass (blur) effect behind its content.
/// On iOS/macCatalyst uses UIVisualEffectView with UIBlurEffect.
/// On Android uses RenderEffect with a blur shader.
/// Falls back to a semi-transparent background on unsupported platforms.
/// </summary>
[ContentProperty(nameof(GlassContent))]
public class FrostedGlassView : ContentView
{
    public static readonly BindableProperty GlassContentProperty = BindableProperty.Create(
        nameof(GlassContent),
        typeof(View),
        typeof(FrostedGlassView),
        null,
        propertyChanged: (b, _, _) => ((FrostedGlassView)b).BuildLayout());

    public static readonly BindableProperty BlurRadiusProperty = BindableProperty.Create(
        nameof(BlurRadius),
        typeof(double),
        typeof(FrostedGlassView),
        20d,
        propertyChanged: (b, _, _) => ((FrostedGlassView)b).UpdateEffect());

    public static readonly BindableProperty TintColorProperty = BindableProperty.Create(
        nameof(TintColor),
        typeof(Color),
        typeof(FrostedGlassView),
        Color.FromRgba(255, 255, 255, 128),
        propertyChanged: (b, _, _) => ((FrostedGlassView)b).UpdateTint());

    public static readonly BindableProperty TintOpacityProperty = BindableProperty.Create(
        nameof(TintOpacity),
        typeof(double),
        typeof(FrostedGlassView),
        0.6,
        propertyChanged: (b, _, _) => ((FrostedGlassView)b).UpdateTint());

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(FrostedGlassView),
        0d,
        propertyChanged: (b, _, _) => ((FrostedGlassView)b).UpdateCornerRadius());

    public View? GlassContent
    {
        get => (View?)GetValue(GlassContentProperty);
        set => SetValue(GlassContentProperty, value);
    }

    public double BlurRadius
    {
        get => (double)GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

    public Color TintColor
    {
        get => (Color)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    public double TintOpacity
    {
        get => (double)GetValue(TintOpacityProperty);
        set => SetValue(TintOpacityProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    readonly Grid rootGrid;
    readonly BoxView tintOverlay;
    readonly ContentView contentHost;
    Border? clipBorder;

    public FrostedGlassView()
    {
        tintOverlay = new BoxView
        {
            Color = TintColor,
            Opacity = TintOpacity
        };

        contentHost = new ContentView();

        rootGrid = new Grid();
        rootGrid.Children.Add(tintOverlay);
        rootGrid.Children.Add(contentHost);

        clipBorder = new Border
        {
            StrokeThickness = 0,
            Stroke = Colors.Transparent,
            Padding = 0,
            Content = rootGrid
        };
        UpdateCornerRadius();

        Content = clipBorder;
    }

    void BuildLayout()
    {
        contentHost.Content = GlassContent;
    }

    void UpdateTint()
    {
        tintOverlay.Color = TintColor;
        tintOverlay.Opacity = TintOpacity;
    }

    void UpdateCornerRadius()
    {
        if (clipBorder != null)
        {
            clipBorder.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new Microsoft.Maui.CornerRadius(CornerRadius)
            };
        }
    }

    void UpdateEffect()
    {
        // Platform-specific blur is applied in OnHandlerChanged
        ApplyPlatformBlur();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        ApplyPlatformBlur();
    }

    void ApplyPlatformBlur()
    {
#if IOS || MACCATALYST
        ApplyiOSBlur();
#elif ANDROID
        ApplyAndroidBlur();
#endif
    }

#if IOS || MACCATALYST
    UIKit.UIVisualEffectView? blurEffectView;

    void ApplyiOSBlur()
    {
        if (Handler?.PlatformView is not UIKit.UIView nativeView)
            return;

        // Remove existing blur view
        if (blurEffectView != null)
        {
            blurEffectView.RemoveFromSuperview();
            blurEffectView.Dispose();
            blurEffectView = null;
        }

        var blurEffect = UIKit.UIBlurEffect.FromStyle(UIKit.UIBlurEffectStyle.SystemMaterial);
        blurEffectView = new UIKit.UIVisualEffectView(blurEffect)
        {
            AutoresizingMask = UIKit.UIViewAutoresizing.FlexibleWidth | UIKit.UIViewAutoresizing.FlexibleHeight,
            Frame = nativeView.Bounds
        };

        if (CornerRadius > 0)
        {
            blurEffectView.Layer.CornerRadius = (nfloat)CornerRadius;
            blurEffectView.ClipsToBounds = true;
        }

        nativeView.InsertSubview(blurEffectView, 0);
        nativeView.ClipsToBounds = true;

        // Make the tint more subtle since UIVisualEffectView handles the tint
        tintOverlay.Opacity = TintOpacity * 0.3;
    }
#endif

#if ANDROID
    void ApplyAndroidBlur()
    {
        if (Handler?.PlatformView is not Android.Views.View nativeView)
            return;

        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
        {
            var radius = (float)BlurRadius;
            var blurEffect = Android.Graphics.RenderEffect.CreateBlurEffect(
                radius, radius, Android.Graphics.Shader.TileMode.Clamp!);
            nativeView.SetRenderEffect(blurEffect);
        }
        // On older Android, the tint overlay provides a fallback appearance
    }
#endif
}
