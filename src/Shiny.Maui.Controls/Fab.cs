using System.Windows.Input;
using Shiny.Maui.Controls.Infrastructure;

namespace Shiny.Maui.Controls;

public class Fab : ContentView
{
    const double DefaultSize = 56;
    const double DefaultIconSize = 24;
    const double DefaultFontSize = 14;

    readonly Border border;
    readonly Grid innerGrid;
    readonly Image iconImage;
    readonly Label textLabel;
    readonly TapGestureRecognizer tap;

    public Fab()
    {
        iconImage = new Image
        {
            WidthRequest = DefaultIconSize,
            HeightRequest = DefaultIconSize,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Aspect = Aspect.AspectFit
        };

        textLabel = new Label
        {
            FontSize = DefaultFontSize,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.NoWrap,
            IsVisible = false
        };

        innerGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 8,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        innerGrid.Add(iconImage, 0, 0);
        innerGrid.Add(textLabel, 1, 0);

        border = new Border
        {
            HeightRequest = DefaultSize,
            MinimumWidthRequest = DefaultSize,
            Padding = new Thickness(16, 0),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(DefaultSize / 2)
            },
            BackgroundColor = Color.FromArgb("#2196F3"),
            Content = innerGrid,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.End,
            Shadow = new Shadow
            {
                Brush = Brush.Black,
                Opacity = 0.25f,
                Radius = 8,
                Offset = new Point(0, 4)
            }
        };

        tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        border.GestureRecognizers.Add(tap);

        Content = border;
        HorizontalOptions = LayoutOptions.End;
        VerticalOptions = LayoutOptions.End;

        UpdateTextVisibility();
        ApplyCircularShape();
    }


    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(ImageSource),
        typeof(Fab),
        null,
        propertyChanged: (b, _, n) =>
        {
            var fab = (Fab)b;
            fab.iconImage.Source = n as ImageSource;
            fab.iconImage.IsVisible = n is not null;
        });
    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(Fab),
        null,
        propertyChanged: (b, _, n) =>
        {
            var fab = (Fab)b;
            fab.textLabel.Text = n as string ?? string.Empty;
            fab.UpdateTextVisibility();
            fab.ApplyCircularShape();
        });
    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command),
        typeof(ICommand),
        typeof(Fab),
        null);
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter),
        typeof(object),
        typeof(Fab),
        null);
    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public static readonly BindableProperty FabBackgroundColorProperty = BindableProperty.Create(
        nameof(FabBackgroundColor),
        typeof(Color),
        typeof(Fab),
        Color.FromArgb("#2196F3"),
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((Fab)b).border.BackgroundColor = c;
        });
    public Color? FabBackgroundColor
    {
        get => (Color?)GetValue(FabBackgroundColorProperty);
        set => SetValue(FabBackgroundColorProperty, value);
    }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(Fab),
        null,
        propertyChanged: (b, _, n) =>
        {
            var fab = (Fab)b;
            if (n is Color c)
                fab.border.Stroke = c;
        });
    public Color? BorderColor
    {
        get => (Color?)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(double),
        typeof(Fab),
        0.0,
        propertyChanged: (b, _, n) => ((Fab)b).border.StrokeThickness = (double)n);
    public double BorderThickness
    {
        get => (double)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(Fab),
        Colors.White,
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((Fab)b).textLabel.TextColor = c;
        });
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(Fab),
        DefaultFontSize,
        propertyChanged: (b, _, n) => ((Fab)b).textLabel.FontSize = (double)n);
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(Fab),
        Microsoft.Maui.Controls.FontAttributes.None,
        propertyChanged: (b, _, n) => ((Fab)b).textLabel.FontAttributes = (FontAttributes)n);
    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }

    public static readonly BindableProperty SizeProperty = BindableProperty.Create(
        nameof(Size),
        typeof(double),
        typeof(Fab),
        DefaultSize,
        propertyChanged: (b, _, _) =>
        {
            var fab = (Fab)b;
            fab.border.HeightRequest = fab.Size;
            fab.border.MinimumWidthRequest = fab.Size;
            fab.ApplyCircularShape();
        });
    public double Size
    {
        get => (double)GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }

    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSize),
        typeof(double),
        typeof(Fab),
        DefaultIconSize,
        propertyChanged: (b, _, n) =>
        {
            var fab = (Fab)b;
            var s = (double)n;
            fab.iconImage.WidthRequest = s;
            fab.iconImage.HeightRequest = s;
        });
    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(
        nameof(HasShadow),
        typeof(bool),
        typeof(Fab),
        true,
        propertyChanged: (b, _, n) =>
        {
            var fab = (Fab)b;
            fab.border.Shadow = (bool)n
                ? new Shadow { Brush = Brush.Black, Opacity = 0.25f, Radius = 8, Offset = new Point(0, 4) }
                : null!;
        });
    public bool HasShadow
    {
        get => (bool)GetValue(HasShadowProperty);
        set => SetValue(HasShadowProperty, value);
    }


    public static readonly BindableProperty UseFeedbackProperty = BindableProperty.Create(
        nameof(UseFeedback),
        typeof(bool),
        typeof(Fab),
        true);
    public bool UseFeedback
    {
        get => (bool)GetValue(UseFeedbackProperty);
        set => SetValue(UseFeedbackProperty, value);
    }


    public event EventHandler? Clicked;


    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (UseFeedback)
            FeedbackHelper.Execute(this, nameof(Clicked));

        Clicked?.Invoke(this, EventArgs.Empty);
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }

    void UpdateTextVisibility()
        => textLabel.IsVisible = !string.IsNullOrEmpty(textLabel.Text);

    void ApplyCircularShape()
    {
        // If no text, make the FAB a perfect circle sized to Size
        if (string.IsNullOrEmpty(Text))
        {
            border.WidthRequest = Size;
            border.HeightRequest = Size;
            border.Padding = 0;
            border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(Size / 2)
            };
        }
        else
        {
            border.WidthRequest = -1;
            border.HeightRequest = Size;
            border.Padding = new Thickness(20, 0);
            border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(Size / 2)
            };
        }
    }
}
