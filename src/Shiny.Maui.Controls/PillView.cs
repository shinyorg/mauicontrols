namespace Shiny.Maui.Controls;

public class PillView : ContentView
{
    readonly Border border;
    readonly Label label;

    bool isUpdatingFromType;

    public PillView()
    {
        label = new Label
        {
            FontSize = 12,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.NoWrap
        };

        border = new Border
        {
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = 12
            },
            StrokeThickness = 1,
            Padding = new Thickness(12, 4),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center,
            Content = label
        };

        Content = border;

        // Apply default (None) styling
        ApplyPillType(PillType.None);
    }


    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(PillView),
        string.Empty,
        propertyChanged: (b, _, n) => ((PillView)b).label.Text = (string)n);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PillTypeProperty = BindableProperty.Create(
        nameof(Type),
        typeof(PillType),
        typeof(PillView),
        PillType.None,
        propertyChanged: OnPillTypeChanged);

    public PillType Type
    {
        get => (PillType)GetValue(PillTypeProperty);
        set => SetValue(PillTypeProperty, value);
    }

    public static readonly BindableProperty PillColorProperty = BindableProperty.Create(
        nameof(PillColor),
        typeof(Color),
        typeof(PillView),
        null,
        propertyChanged: OnPillColorChanged);

    public Color? PillColor
    {
        get => (Color?)GetValue(PillColorProperty);
        set => SetValue(PillColorProperty, value);
    }

    public static readonly BindableProperty PillTextColorProperty = BindableProperty.Create(
        nameof(PillTextColor),
        typeof(Color),
        typeof(PillView),
        null,
        propertyChanged: (b, _, n) =>
        {
            var pill = (PillView)b;
            if (n is Color c)
                pill.label.TextColor = c;
        });

    public Color? PillTextColor
    {
        get => (Color?)GetValue(PillTextColorProperty);
        set => SetValue(PillTextColorProperty, value);
    }

    public static readonly BindableProperty PillBorderColorProperty = BindableProperty.Create(
        nameof(PillBorderColor),
        typeof(Color),
        typeof(PillView),
        null,
        propertyChanged: (b, _, n) =>
        {
            var pill = (PillView)b;
            if (n is Color c)
                pill.border.Stroke = c;
        });

    public Color? PillBorderColor
    {
        get => (Color?)GetValue(PillBorderColorProperty);
        set => SetValue(PillBorderColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(PillView),
        12.0,
        propertyChanged: (b, _, n) => ((PillView)b).label.FontSize = (double)n);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(PillView),
        12.0,
        propertyChanged: (b, _, n) =>
        {
            var pill = (PillView)b;
            var r = (double)n;
            pill.border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(r)
            };
        });

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(PillView),
        Microsoft.Maui.Controls.FontAttributes.None,
        propertyChanged: (b, _, n) => ((PillView)b).label.FontAttributes = (FontAttributes)n);

    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }



    static void OnPillTypeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var pill = (PillView)bindable;
        pill.ApplyPillType((PillType)newValue);
    }

    static void OnPillColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var pill = (PillView)bindable;
        if (pill.isUpdatingFromType) return;

        if (newValue is Color baseColor)
            pill.ApplyBaseColor(baseColor);
    }



    void ApplyPillType(PillType type)
    {
        var (bg, text, borderColor) = type switch
        {
            PillType.Success => (Color.FromArgb("#DCFCE7"), Color.FromArgb("#166534"), Color.FromArgb("#86EFAC")),
            PillType.Info => (Color.FromArgb("#DBEAFE"), Color.FromArgb("#1E40AF"), Color.FromArgb("#93C5FD")),
            PillType.Warning => (Color.FromArgb("#FEF9C3"), Color.FromArgb("#854D0E"), Color.FromArgb("#FDE047")),
            PillType.Caution => (Color.FromArgb("#FFEDD5"), Color.FromArgb("#9A3412"), Color.FromArgb("#FDBA74")),
            PillType.Critical => (Color.FromArgb("#FEE2E2"), Color.FromArgb("#991B1B"), Color.FromArgb("#FCA5A5")),
            _ => (Color.FromArgb("#F3F4F6"), Color.FromArgb("#374151"), Color.FromArgb("#D1D5DB")),
        };

        isUpdatingFromType = true;
        border.BackgroundColor = bg;
        border.Stroke = PillBorderColor ?? borderColor;
        label.TextColor = PillTextColor ?? text;
        isUpdatingFromType = false;
    }

    void ApplyBaseColor(Color baseColor)
    {
        border.BackgroundColor = baseColor;

        if (PillBorderColor is null)
            border.Stroke = DarkenColor(baseColor, 0.2f);

        if (PillTextColor is null)
            label.TextColor = GetContrastTextColor(baseColor);
    }

    static Color DarkenColor(Color color, float amount)
    {
        color.ToHsl(out var h, out var s, out var l);
        l = Math.Max(0, l - amount);
        return Color.FromHsla(h, s, l);
    }

    static Color GetContrastTextColor(Color bgColor)
    {
        // Relative luminance per WCAG
        var luminance = 0.2126 * Linearize(bgColor.Red)
                      + 0.7152 * Linearize(bgColor.Green)
                      + 0.0722 * Linearize(bgColor.Blue);

        // Light backgrounds get dark text, dark backgrounds get white text
        if (luminance > 0.4)
            return DarkenColor(bgColor, 0.5f);

        return Colors.White;
    }

    static double Linearize(float channel)
    {
        return channel <= 0.04045
            ? channel / 12.92
            : Math.Pow((channel + 0.055) / 1.055, 2.4);
    }

}