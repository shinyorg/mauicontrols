namespace Shiny.Maui.Controls.Chat.Internal;

class ChatTypingIndicator : ContentView
{
    readonly HorizontalStackLayout layout;
    readonly BoxView dot1;
    readonly BoxView dot2;
    readonly BoxView dot3;
    readonly Label textLabel;
    bool isAnimating;

    public ChatTypingIndicator()
    {
        dot1 = CreateDot();
        dot2 = CreateDot();
        dot3 = CreateDot();

        textLabel = new Label
        {
            FontSize = 13,
            TextColor = Colors.Grey,
            VerticalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(4, 0, 0, 0)
        };

        layout = new HorizontalStackLayout
        {
            Spacing = 3,
            Children = { dot1, dot2, dot3, textLabel }
        };

        var pill = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
            BackgroundColor = Color.FromArgb("#E8E8ED"),
            Padding = new Thickness(12, 8),
            HorizontalOptions = LayoutOptions.Start,
            Content = layout,
            Shadow = new Shadow
            {
                Brush = Colors.Black,
                Offset = new Point(0, 1),
                Radius = 3,
                Opacity = 0.15f
            }
        };

        Content = pill;
        IsVisible = false;
    }

    public string Text
    {
        get => textLabel.Text ?? string.Empty;
        set => textLabel.Text = value;
    }

    static BoxView CreateDot() => new()
    {
        WidthRequest = 6,
        HeightRequest = 6,
        CornerRadius = 3,
        Color = Colors.Grey,
        VerticalOptions = LayoutOptions.Center
    };

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsVisible))
        {
            if (IsVisible && !isAnimating)
                StartAnimation();
            else if (!IsVisible)
                StopAnimation();
        }
    }

    void StartAnimation()
    {
        isAnimating = true;
        var animation = new Animation();

        animation.Add(0.0, 0.4, new Animation(v => dot1.TranslationY = v, 0, -4));
        animation.Add(0.4, 0.8, new Animation(v => dot1.TranslationY = v, -4, 0));

        animation.Add(0.15, 0.55, new Animation(v => dot2.TranslationY = v, 0, -4));
        animation.Add(0.55, 0.95, new Animation(v => dot2.TranslationY = v, -4, 0));

        animation.Add(0.3, 0.7, new Animation(v => dot3.TranslationY = v, 0, -4));
        animation.Add(0.7, 1.0, new Animation(v => dot3.TranslationY = v, -4, 0));

        animation.Commit(this, "TypingDots", length: 1000, repeat: () => isAnimating);
    }

    void StopAnimation()
    {
        isAnimating = false;
        this.AbortAnimation("TypingDots");
        dot1.TranslationY = 0;
        dot2.TranslationY = 0;
        dot3.TranslationY = 0;
    }
}
