using Shiny.Maui.Controls.MermaidDiagrams.Layout;
using Shiny.Maui.Controls.MermaidDiagrams.Models;
using Shiny.Maui.Controls.MermaidDiagrams.Parsing;
using Shiny.Maui.Controls.MermaidDiagrams.Rendering;
using Shiny.Maui.Controls.MermaidDiagrams.Theming;

namespace Shiny.Maui.Controls.MermaidDiagrams.Controls;

public partial class MermaidDiagramControl : ContentView
{
    readonly GraphicsView graphicsView;
    readonly DiagramDrawable drawable;
    readonly ILayoutEngine layoutEngine = new SugiyamaLayoutEngine();

    public static readonly BindableProperty DiagramTextProperty = BindableProperty.Create(
        nameof(DiagramText),
        typeof(string),
        typeof(MermaidDiagramControl),
        string.Empty,
        propertyChanged: OnDiagramTextChanged);

    public static readonly BindableProperty ThemeProperty = BindableProperty.Create(
        nameof(Theme),
        typeof(DiagramTheme),
        typeof(MermaidDiagramControl),
        new DefaultTheme(),
        propertyChanged: OnThemeChanged);

    public static readonly BindableProperty ZoomLevelProperty = BindableProperty.Create(
        nameof(ZoomLevel),
        typeof(float),
        typeof(MermaidDiagramControl),
        1f,
        propertyChanged: OnZoomLevelChanged);

    public static readonly BindableProperty ParseErrorProperty = BindableProperty.Create(
        nameof(ParseError),
        typeof(string),
        typeof(MermaidDiagramControl),
        null);

    public static readonly BindableProperty DiagramLayoutOptionsProperty = BindableProperty.Create(
        nameof(DiagramLayoutOptions),
        typeof(DiagramLayoutOptions),
        typeof(MermaidDiagramControl),
        new DiagramLayoutOptions(),
        propertyChanged: OnLayoutOptionsChanged);

    public static readonly BindableProperty AllowPanningProperty = BindableProperty.Create(
        nameof(AllowPanning),
        typeof(bool),
        typeof(MermaidDiagramControl),
        true,
        propertyChanged: OnGestureSettingsChanged);

    public static readonly BindableProperty AllowZoomingProperty = BindableProperty.Create(
        nameof(AllowZooming),
        typeof(bool),
        typeof(MermaidDiagramControl),
        true,
        propertyChanged: OnGestureSettingsChanged);

    public string DiagramText
    {
        get => (string)GetValue(DiagramTextProperty);
        set => SetValue(DiagramTextProperty, value);
    }

    public DiagramTheme Theme
    {
        get => (DiagramTheme)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public float ZoomLevel
    {
        get => (float)GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }

    public string? ParseError
    {
        get => (string?)GetValue(ParseErrorProperty);
        set => SetValue(ParseErrorProperty, value);
    }

    public DiagramLayoutOptions DiagramLayoutOptions
    {
        get => (DiagramLayoutOptions)GetValue(DiagramLayoutOptionsProperty);
        set => SetValue(DiagramLayoutOptionsProperty, value);
    }

    public bool AllowPanning
    {
        get => (bool)GetValue(AllowPanningProperty);
        set => SetValue(AllowPanningProperty, value);
    }

    public bool AllowZooming
    {
        get => (bool)GetValue(AllowZoomingProperty);
        set => SetValue(AllowZoomingProperty, value);
    }

    public MermaidDiagramControl()
    {
        drawable = new DiagramDrawable();

        graphicsView = new GraphicsView
        {
            Drawable = drawable,
            HorizontalOptions = Microsoft.Maui.Controls.LayoutOptions.Fill,
            VerticalOptions = Microsoft.Maui.Controls.LayoutOptions.Fill
        };

        SetupGestures();
        Content = graphicsView;
    }

    void UpdateDiagram()
    {
        var text = DiagramText;
        if (string.IsNullOrWhiteSpace(text))
        {
            drawable.LayoutResult = null;
            ParseError = null;
            graphicsView.Invalidate();
            return;
        }

        try
        {
            var model = MermaidParser.Parse(text);
            var options = DiagramLayoutOptions;
            var result = layoutEngine.Layout(model, options);

            drawable.LayoutResult = result;
            ParseError = null;
        }
        catch (ParseException ex)
        {
            ParseError = ex.Message;
            drawable.LayoutResult = null;
        }
        catch (Exception ex)
        {
            ParseError = $"Error: {ex.Message}";
            drawable.LayoutResult = null;
        }

        graphicsView.Invalidate();
    }

    static void OnDiagramTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MermaidDiagramControl control)
            control.UpdateDiagram();
    }

    static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MermaidDiagramControl control)
        {
            control.drawable.Theme = control.Theme;
            control.graphicsView.Invalidate();
        }
    }

    static void OnZoomLevelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MermaidDiagramControl control)
        {
            control.drawable.UserScale = control.ZoomLevel;
            control.graphicsView.Invalidate();
        }
    }

    static void OnLayoutOptionsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MermaidDiagramControl control)
            control.UpdateDiagram();
    }

    static void OnGestureSettingsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MermaidDiagramControl control)
            control.UpdateGestures();
    }
}