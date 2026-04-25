namespace Shiny.Maui.Controls.Chat.Internal;

class ChatTypingBubbleView : ContentView
{
    readonly ChatView chatView;
    readonly Grid avatarNameRow;
    readonly Border avatarBorder;
    readonly Label avatarLabel;
    readonly Image avatarImage;
    readonly Label nameLabel;
    readonly Border bubbleBorder;
    readonly BoxView dot1;
    readonly BoxView dot2;
    readonly BoxView dot3;
    bool isAnimating;

    public ChatTypingBubbleView(ChatView chatView)
    {
        this.chatView = chatView;

        avatarImage = new Image
        {
            WidthRequest = 32,
            HeightRequest = 32,
            Aspect = Aspect.AspectFill
        };

        avatarLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        avatarBorder = new Border
        {
            WidthRequest = 32,
            HeightRequest = 32,
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            Padding = 0,
            VerticalOptions = LayoutOptions.Center
        };

        nameLabel = new Label
        {
            FontSize = 12,
            TextColor = Colors.Grey,
            Margin = new Thickness(4, 0, 0, 2),
            VerticalOptions = LayoutOptions.Center
        };

        avatarNameRow = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Auto)
            },
            ColumnSpacing = 6,
            Margin = new Thickness(0, 0, 0, 2)
        };
        avatarNameRow.Add(avatarBorder, 0, 0);
        avatarNameRow.Add(nameLabel, 1, 0);

        dot1 = CreateDot();
        dot2 = CreateDot();
        dot3 = CreateDot();

        var dotsLayout = new HorizontalStackLayout
        {
            Spacing = 4,
            Children = { dot1, dot2, dot3 }
        };

        bubbleBorder = new Border
        {
            Padding = new Thickness(14, 10),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
            Content = dotsLayout,
            HorizontalOptions = LayoutOptions.Start
        };

        var rootLayout = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            Padding = new Thickness(12, 0),
            HorizontalOptions = LayoutOptions.Start
        };
        rootLayout.Add(avatarNameRow, 0, 0);
        rootLayout.Add(bubbleBorder, 0, 1);

        Margin = new Thickness(0, 4, 0, 0);
        Content = rootLayout;
    }

    static BoxView CreateDot() => new()
    {
        WidthRequest = 8,
        HeightRequest = 8,
        CornerRadius = 4,
        Color = Colors.Grey,
        VerticalOptions = LayoutOptions.Center
    };

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is not ChatMessage message || !message.IsTypingIndicator)
            return;

        Configure(message);
    }

    void Configure(ChatMessage message)
    {
        var participant = GetParticipant(message.SenderId);
        var bubbleColor = participant?.BubbleColor ?? chatView.OtherBubbleColor;
        bubbleBorder.BackgroundColor = bubbleColor;

        var showAvatar = chatView.IsMultiPerson || chatView.ShowAvatarsInSingleChat;
        avatarNameRow.IsVisible = showAvatar;

        if (showAvatar)
        {
            nameLabel.Text = participant?.DisplayName ?? "Unknown";
            avatarBorder.BackgroundColor = bubbleColor;

            if (participant?.Avatar is not null)
            {
                avatarImage.Source = participant.Avatar;
                avatarBorder.Content = avatarImage;
            }
            else
            {
                avatarLabel.Text = ChatGroupHelper.GetInitials(participant?.DisplayName);
                avatarBorder.Content = avatarLabel;
            }
        }

        StartAnimation();
    }

    void StartAnimation()
    {
        if (isAnimating) return;
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

    ChatParticipant? GetParticipant(string senderId)
    {
        var participants = chatView.Participants;
        if (participants is null) return null;
        for (var i = 0; i < participants.Count; i++)
        {
            if (participants[i].Id == senderId)
                return participants[i];
        }
        return null;
    }
}
