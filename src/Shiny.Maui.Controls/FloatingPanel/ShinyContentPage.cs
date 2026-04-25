namespace Shiny.Maui.Controls.FloatingPanel;

/// <summary>
/// A ContentPage with a built-in OverlayHost. Set page content via <see cref="PageContent"/>
/// and add FloatingPanels via <see cref="Panels"/>. The overlay layer is automatically
/// managed — no need to wrap your layout in a Grid + OverlayHost manually.
/// </summary>
[ContentProperty(nameof(PageContent))]
public class ShinyContentPage : ContentPage
{
    readonly Grid rootGrid;
    readonly OverlayHost overlayHost;

    public ShinyContentPage()
    {
        overlayHost = new OverlayHost();
        rootGrid = new Grid
        {
            Children = { overlayHost }
        };
        base.Content = rootGrid;
    }

    public static readonly BindableProperty PageContentProperty = BindableProperty.Create(
        nameof(PageContent),
        typeof(View),
        typeof(ShinyContentPage),
        null,
        propertyChanged: OnPageContentChanged);

    public View? PageContent
    {
        get => (View?)GetValue(PageContentProperty);
        set => SetValue(PageContentProperty, value);
    }

    /// <summary>
    /// The OverlayHost for this page. Add FloatingPanels directly,
    /// or use the <see cref="Panels"/> collection for XAML convenience.
    /// </summary>
    public OverlayHost OverlayHost => overlayHost;

    public static readonly BindableProperty BackdropColorProperty = BindableProperty.Create(
        nameof(BackdropColor),
        typeof(Color),
        typeof(ShinyContentPage),
        Colors.Black,
        propertyChanged: (b, _, n) => ((ShinyContentPage)b).overlayHost.BackdropColor = (Color)n);

    public Color BackdropColor
    {
        get => (Color)GetValue(BackdropColorProperty);
        set => SetValue(BackdropColorProperty, value);
    }

    public static readonly BindableProperty BackdropMaxOpacityProperty = BindableProperty.Create(
        nameof(BackdropMaxOpacity),
        typeof(double),
        typeof(ShinyContentPage),
        0.5,
        propertyChanged: (b, _, n) => ((ShinyContentPage)b).overlayHost.BackdropMaxOpacity = (double)n);

    public double BackdropMaxOpacity
    {
        get => (double)GetValue(BackdropMaxOpacityProperty);
        set => SetValue(BackdropMaxOpacityProperty, value);
    }

    /// <summary>
    /// Collection of FloatingPanels to display in the overlay layer.
    /// </summary>
    public IList<IView> Panels => overlayHost.Children;

    /// <summary>
    /// Hides the base Content property — use <see cref="PageContent"/> instead.
    /// </summary>
    public new View? Content
    {
        get => PageContent;
        set => PageContent = value;
    }

    static void OnPageContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var page = (ShinyContentPage)bindable;

        if (oldValue is View oldView)
            page.rootGrid.Children.Remove(oldView);

        if (newValue is View newView)
        {
            // Insert content behind the overlay host
            page.rootGrid.Children.Insert(0, newView);
        }
    }
}
