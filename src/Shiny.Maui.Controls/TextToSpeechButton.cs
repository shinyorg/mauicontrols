using Shiny.Maui.Controls.Infrastructure;

namespace Shiny.Maui.Controls;

public class TextToSpeechButton : ContentView
{
    const double DefaultFontSize = 14;
    const double DefaultCornerRadius = 8;
    const double DefaultIconSize = 24;

    readonly Border border;
    readonly Grid innerGrid;
    readonly Image iconImage;
    readonly Label textLabel;
    readonly TapGestureRecognizer tap;
    CancellationTokenSource? cts;

    public TextToSpeechButton()
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
            TextColor = Colors.White
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
            Padding = new Thickness(16, 10),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(DefaultCornerRadius)
            },
            BackgroundColor = Color.FromArgb("#2196F3"),
            Content = innerGrid,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        tap = new TapGestureRecognizer();
        tap.Tapped += OnTapped;
        border.GestureRecognizers.Add(tap);

        Content = border;

        UpdateIconVisibility();
        UpdateTextVisibility();
    }


    public static readonly BindableProperty SpeechTextProperty = BindableProperty.Create(
        nameof(SpeechText),
        typeof(string),
        typeof(TextToSpeechButton),
        null);
    public string? SpeechText
    {
        get => (string?)GetValue(SpeechTextProperty);
        set => SetValue(SpeechTextProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(TextToSpeechButton),
        null,
        propertyChanged: (b, _, n) =>
        {
            var btn = (TextToSpeechButton)b;
            btn.textLabel.Text = n as string ?? string.Empty;
            btn.UpdateTextVisibility();
        });
    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(ImageSource),
        typeof(TextToSpeechButton),
        null,
        propertyChanged: (b, _, _) => ((TextToSpeechButton)b).UpdateIconVisibility());
    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty ButtonBackgroundColorProperty = BindableProperty.Create(
        nameof(ButtonBackgroundColor),
        typeof(Color),
        typeof(TextToSpeechButton),
        Color.FromArgb("#2196F3"),
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((TextToSpeechButton)b).border.BackgroundColor = c;
        });
    public Color? ButtonBackgroundColor
    {
        get => (Color?)GetValue(ButtonBackgroundColorProperty);
        set => SetValue(ButtonBackgroundColorProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(TextToSpeechButton),
        Colors.White,
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((TextToSpeechButton)b).textLabel.TextColor = c;
        });
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(TextToSpeechButton),
        DefaultFontSize,
        propertyChanged: (b, _, n) => ((TextToSpeechButton)b).textLabel.FontSize = (double)n);
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(TextToSpeechButton),
        Microsoft.Maui.Controls.FontAttributes.None,
        propertyChanged: (b, _, n) => ((TextToSpeechButton)b).textLabel.FontAttributes = (FontAttributes)n);
    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(TextToSpeechButton),
        DefaultCornerRadius,
        propertyChanged: (b, _, n) =>
        {
            ((TextToSpeechButton)b).border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius((double)n)
            };
        });
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(TextToSpeechButton),
        null,
        propertyChanged: (b, _, n) =>
        {
            if (n is Color c)
                ((TextToSpeechButton)b).border.Stroke = c;
        });
    public Color? BorderColor
    {
        get => (Color?)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public static readonly BindableProperty BorderThicknessProperty = BindableProperty.Create(
        nameof(BorderThickness),
        typeof(double),
        typeof(TextToSpeechButton),
        0.0,
        propertyChanged: (b, _, n) => ((TextToSpeechButton)b).border.StrokeThickness = (double)n);
    public double BorderThickness
    {
        get => (double)GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSize),
        typeof(double),
        typeof(TextToSpeechButton),
        DefaultIconSize,
        propertyChanged: (b, _, n) =>
        {
            var btn = (TextToSpeechButton)b;
            var s = (double)n;
            btn.iconImage.WidthRequest = s;
            btn.iconImage.HeightRequest = s;
        });
    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly BindableProperty IsSpeakingProperty = BindableProperty.Create(
        nameof(IsSpeaking),
        typeof(bool),
        typeof(TextToSpeechButton),
        false,
        defaultBindingMode: BindingMode.OneWayToSource);
    public bool IsSpeaking
    {
        get => (bool)GetValue(IsSpeakingProperty);
        private set => SetValue(IsSpeakingProperty, value);
    }

    public static readonly BindableProperty PitchProperty = BindableProperty.Create(
        nameof(Pitch),
        typeof(float),
        typeof(TextToSpeechButton),
        1.0f);
    public float Pitch
    {
        get => (float)GetValue(PitchProperty);
        set => SetValue(PitchProperty, value);
    }

    public static readonly BindableProperty VolumeProperty = BindableProperty.Create(
        nameof(Volume),
        typeof(float),
        typeof(TextToSpeechButton),
        1.0f);
    public float Volume
    {
        get => (float)GetValue(VolumeProperty);
        set => SetValue(VolumeProperty, value);
    }

    public static readonly BindableProperty LocaleProperty = BindableProperty.Create(
        nameof(Locale),
        typeof(Locale),
        typeof(TextToSpeechButton),
        null);
    public Locale? Locale
    {
        get => (Locale?)GetValue(LocaleProperty);
        set => SetValue(LocaleProperty, value);
    }

    public static readonly BindableProperty UseHapticFeedbackProperty = BindableProperty.Create(
        nameof(UseHapticFeedback),
        typeof(bool),
        typeof(TextToSpeechButton),
        true);
    public bool UseHapticFeedback
    {
        get => (bool)GetValue(UseHapticFeedbackProperty);
        set => SetValue(UseHapticFeedbackProperty, value);
    }

    public static readonly BindableProperty HasShadowProperty = BindableProperty.Create(
        nameof(HasShadow),
        typeof(bool),
        typeof(TextToSpeechButton),
        false,
        propertyChanged: (b, _, n) =>
        {
            ((TextToSpeechButton)b).border.Shadow = (bool)n
                ? new Shadow { Brush = Brush.Black, Opacity = 0.25f, Radius = 8, Offset = new Point(0, 4) }
                : null!;
        });
    public bool HasShadow
    {
        get => (bool)GetValue(HasShadowProperty);
        set => SetValue(HasShadowProperty, value);
    }


    public event EventHandler? Clicked;


    async void OnTapped(object? sender, TappedEventArgs e)
    {
        if (UseHapticFeedback)
            HapticHelper.PerformClick();

        Clicked?.Invoke(this, EventArgs.Empty);

        if (IsSpeaking)
        {
            Cancel();
            return;
        }

        var speechText = SpeechText;
        if (string.IsNullOrWhiteSpace(speechText))
            return;

        cts = new CancellationTokenSource();
        IsSpeaking = true;
        try
        {
            var options = new SpeechOptions
            {
                Pitch = Pitch,
                Volume = Volume,
                Locale = Locale
            };
            await TextToSpeech.Default.SpeakAsync(speechText, options, cts.Token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            IsSpeaking = false;
            cts?.Dispose();
            cts = null;
        }
    }

    public void Cancel()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    void UpdateIconVisibility()
        => iconImage.IsVisible = Icon is not null;

    void UpdateTextVisibility()
        => textLabel.IsVisible = !string.IsNullOrEmpty(textLabel.Text);
}
