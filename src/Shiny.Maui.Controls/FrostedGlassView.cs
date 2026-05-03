namespace Shiny.Maui.Controls;

/// <summary>
/// A view that applies a frosted glass (blur) effect behind its content.
/// On iOS/macCatalyst uses UIVisualEffectView with UIBlurEffect for true real-time backdrop blur.
/// On Android 12+ captures the background behind the view and applies RenderEffect blur.
/// Falls back to a semi-transparent tinted background on unsupported platforms.
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
            BackgroundColor = Colors.Transparent,
            Content = rootGrid
        };
        UpdateCornerRadius();

        BackgroundColor = Colors.Transparent;
        rootGrid.BackgroundColor = Colors.Transparent;
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
#if IOS || MACCATALYST
        if (blurEffectView != null)
            tintOverlay.Opacity = TintOpacity * 0.4;
#endif
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
        ApplyPlatformBlur();
    }

    void UpdateEffect()
    {
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

        // Clear all backgrounds in the native view hierarchy so blur shows through
        ClearNativeBackgrounds(nativeView);

        // Pick style based on app theme for natural frosted appearance
        var blurStyle = Application.Current?.UserAppTheme == AppTheme.Dark
            ? UIKit.UIBlurEffectStyle.SystemUltraThinMaterialDark
            : UIKit.UIBlurEffectStyle.SystemUltraThinMaterialLight;

        var blurEffect = UIKit.UIBlurEffect.FromStyle(blurStyle);
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

        // UIVisualEffectView provides its own tint, so reduce our overlay
        tintOverlay.Opacity = TintOpacity * 0.4;
    }

    static void ClearNativeBackgrounds(UIKit.UIView view)
    {
        view.BackgroundColor = UIKit.UIColor.Clear;
        view.Opaque = false;
        foreach (var subview in view.Subviews)
        {
            // Only clear container views, not actual content views
            if (subview is not UIKit.UIVisualEffectView &&
                subview is not UIKit.UILabel &&
                subview is not UIKit.UIImageView)
            {
                subview.BackgroundColor = UIKit.UIColor.Clear;
                subview.Opaque = false;
            }
        }
    }
#endif

#if ANDROID
    Android.Widget.ImageView? blurredBackgroundView;
    BlurPreDrawListener? preDrawListener;

    void ApplyAndroidBlur()
    {
        if (Handler?.PlatformView is not Android.Views.View nativeView)
            return;

        // Clear any previous RenderEffect from the view itself (old broken approach)
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            nativeView.SetRenderEffect(null);

        nativeView.SetBackgroundColor(Android.Graphics.Color.Transparent);

        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            AttachBlurCapture(nativeView);
    }

    void AttachBlurCapture(Android.Views.View nativeView)
    {
        if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.S)
            return;

        // Clean up previous listener
        DetachBlurCapture(nativeView);

        preDrawListener = new BlurPreDrawListener(nativeView, (float)BlurRadius, (float)CornerRadius);
        nativeView.ViewTreeObserver?.AddOnPreDrawListener(preDrawListener);
    }

    void DetachBlurCapture(Android.Views.View nativeView)
    {
        if (preDrawListener != null)
        {
            nativeView.ViewTreeObserver?.RemoveOnPreDrawListener(preDrawListener);
            preDrawListener.Dispose();
            preDrawListener = null;
        }
    }

    class BlurPreDrawListener : Java.Lang.Object, Android.Views.ViewTreeObserver.IOnPreDrawListener
    {
        readonly Android.Views.View targetView;
        readonly float blurRadius;
        readonly float cornerRadius;
        Android.Widget.ImageView? blurLayer;
        bool isCapturing;

        public BlurPreDrawListener(Android.Views.View target, float blur, float corner)
        {
            targetView = target;
            blurRadius = Math.Clamp(blur, 1f, 150f);
            cornerRadius = corner;
        }

        public bool OnPreDraw()
        {
            if (isCapturing) return true;
            if (targetView.Width <= 0 || targetView.Height <= 0) return true;
            if (targetView.Parent is not Android.Views.ViewGroup parent) return true;

            try
            {
                isCapturing = true;

                // Hide our view temporarily to capture what's behind it
                var wasVisible = targetView.Visibility;
                targetView.Visibility = Android.Views.ViewStates.Invisible;

                // Capture the parent at our view's location
                var scale = 0.25f; // Downsample for performance
                var w = (int)(targetView.Width * scale);
                var h = (int)(targetView.Height * scale);
                if (w <= 0 || h <= 0) { targetView.Visibility = wasVisible; return true; }

                var bitmap = Android.Graphics.Bitmap.CreateBitmap(w, h, Android.Graphics.Bitmap.Config.Argb8888!);
                if (bitmap == null) { targetView.Visibility = wasVisible; return true; }

                var canvas = new Android.Graphics.Canvas(bitmap);
                canvas.Scale(scale, scale);
                canvas.Translate(-targetView.Left, -targetView.Top);
                parent.Draw(canvas);

                targetView.Visibility = wasVisible;

                // Apply blur via RenderEffect on a background ImageView
                if (blurLayer == null)
                {
                    blurLayer = new Android.Widget.ImageView(targetView.Context);
                    blurLayer.SetScaleType(Android.Widget.ImageView.ScaleType.FitXy);

                    // Insert behind the MAUI content
                    if (targetView is Android.Views.ViewGroup vg)
                    {
                        blurLayer.LayoutParameters = new Android.Views.ViewGroup.LayoutParams(
                            Android.Views.ViewGroup.LayoutParams.MatchParent,
                            Android.Views.ViewGroup.LayoutParams.MatchParent);
                        vg.AddView(blurLayer, 0);
                    }
                }

                blurLayer.SetImageBitmap(bitmap);

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
                {
                    var effect = Android.Graphics.RenderEffect.CreateBlurEffect(
                        blurRadius / scale, blurRadius / scale,
                        Android.Graphics.Shader.TileMode.Clamp!);
                    blurLayer.SetRenderEffect(effect);
                }

                if (cornerRadius > 0)
                {
                    blurLayer.ClipToOutline = true;
                    blurLayer.OutlineProvider = new RoundedOutlineProvider(cornerRadius);
                }
            }
            catch
            {
                // Blur is best-effort; don't crash the app
            }
            finally
            {
                isCapturing = false;
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (blurLayer?.Parent is Android.Views.ViewGroup vg)
                    vg.RemoveView(blurLayer);
                blurLayer?.Dispose();
                blurLayer = null;
            }
            base.Dispose(disposing);
        }
    }

    class RoundedOutlineProvider : Android.Views.ViewOutlineProvider
    {
        readonly float radius;
        public RoundedOutlineProvider(float radius) => this.radius = radius;

        public override void GetOutline(Android.Views.View? view, Android.Graphics.Outline? outline)
        {
            if (view == null || outline == null) return;
            outline.SetRoundRect(0, 0, view.Width, view.Height, radius);
        }
    }
#endif
}
