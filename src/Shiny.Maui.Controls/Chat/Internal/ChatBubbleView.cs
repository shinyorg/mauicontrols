using System.Text.RegularExpressions;

namespace Shiny.Maui.Controls.Chat.Internal;

partial class ChatBubbleView : ContentView
{
    static readonly Regex UrlRegex = CreateUrlRegex();

    readonly ChatView chatView;
    readonly bool isMe;

    readonly Grid rootLayout;
    readonly Grid avatarNameRow;
    readonly Border avatarBorder;
    readonly Label avatarLabel;
    readonly Image avatarImage;
    readonly Label nameLabel;
    readonly Border bubbleBorder;
    readonly Label textLabel;
    readonly Image imageView;
    readonly Label timestampLabel;

    public ChatBubbleView(ChatView chatView, bool isMe)
    {
        this.chatView = chatView;
        this.isMe = isMe;

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

        textLabel = new Label
        {
            LineBreakMode = LineBreakMode.WordWrap,
            FontSize = 15
        };

        imageView = new Image
        {
            Aspect = Aspect.AspectFit,
            MaximumHeightRequest = 250,
            MaximumWidthRequest = 250,
            IsVisible = false
        };

        bubbleBorder = new Border
        {
            Padding = new Thickness(12, 8),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },

            MaximumWidthRequest = 280,
            Content = new VerticalStackLayout
            {
                Children = { textLabel, imageView }
            }
        };

        var bubbleTap = new TapGestureRecognizer();
        bubbleTap.Tapped += OnBubbleTapped;
        bubbleBorder.GestureRecognizers.Add(bubbleTap);

        timestampLabel = new Label
        {
            FontSize = 11,
            TextColor = Colors.Grey,
            Margin = new Thickness(4, 2, 4, 0)
        };

        rootLayout = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            Padding = new Thickness(12, 0)
        };
        rootLayout.Add(avatarNameRow, 0, 0);
        rootLayout.Add(bubbleBorder, 0, 1);
        rootLayout.Add(timestampLabel, 0, 2);

        Content = rootLayout;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is not ChatMessage message)
            return;

        Configure(message);
    }

    void Configure(ChatMessage message)
    {
        var messages = chatView.Messages;
        if (messages is null or { Count: 0 })
            return;

        var index = -1;
        for (var i = 0; i < messages.Count; i++)
        {
            if (ReferenceEquals(messages[i], message))
            {
                index = i;
                break;
            }
        }

        var prev = index > 0 ? messages[index - 1] : null;
        var next = index < messages.Count - 1 ? messages[index + 1] : null;
        var isFirst = ChatGroupHelper.IsNewGroup(message, prev);
        var isLast = next is null || ChatGroupHelper.IsNewGroup(next, message);

        var showAvatar = ShouldShowAvatar(message, isFirst);
        var participant = GetParticipant(message.SenderId);

        // Alignment
        if (isMe)
        {
            rootLayout.HorizontalOptions = LayoutOptions.End;
            timestampLabel.HorizontalTextAlignment = TextAlignment.End;
        }
        else
        {
            rootLayout.HorizontalOptions = LayoutOptions.Start;
            timestampLabel.HorizontalTextAlignment = TextAlignment.Start;
        }

        // Avatar + Name
        avatarNameRow.IsVisible = showAvatar;
        if (showAvatar)
        {
            nameLabel.Text = participant?.DisplayName ?? "Unknown";

            var avatarColor = participant?.BubbleColor ?? chatView.OtherBubbleColor;
            avatarBorder.BackgroundColor = avatarColor;

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

        // Bubble colors
        var bubbleColor = isMe
            ? chatView.MyBubbleColor
            : (participant?.BubbleColor ?? chatView.OtherBubbleColor);
        var textColor = isMe ? chatView.MyTextColor : chatView.OtherTextColor;

        bubbleBorder.BackgroundColor = bubbleColor;

        // Corner radius: rounded with smaller tail corner
        var tailRadius = isLast ? 4 : 18;
        bubbleBorder.StrokeShape = isMe
            ? new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(18, 18, 18, tailRadius) }
            : new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = new CornerRadius(18, 18, tailRadius, 18) };

        // Content: text or image
        if (!string.IsNullOrEmpty(message.ImageUrl))
        {
            textLabel.IsVisible = false;
            imageView.IsVisible = true;
            imageView.Source = message.ImageUrl;
            bubbleBorder.Padding = new Thickness(4);
        }
        else
        {
            textLabel.IsVisible = true;
            imageView.IsVisible = false;
            textLabel.TextColor = textColor;
            SetTextWithLinks(textLabel, message.Text ?? string.Empty, textColor);
            bubbleBorder.Padding = new Thickness(12, 8);
        }

        // Timestamp
        timestampLabel.IsVisible = isLast;
        if (isLast)
            timestampLabel.Text = ChatGroupHelper.FormatTimestamp(message.Timestamp);

        // Spacing
        Margin = new Thickness(0, isFirst ? 12 : 2, 0, 0);
    }

    bool ShouldShowAvatar(ChatMessage message, bool isFirstInGroup)
    {
        if (message.IsFromMe)
            return false;

        if (!isFirstInGroup)
            return false;

        if (chatView.IsMultiPerson)
            return true;

        return chatView.ShowAvatarsInSingleChat;
    }

    void OnBubbleTapped(object? sender, TappedEventArgs e)
    {
        if (BindingContext is ChatMessage msg)
            chatView.OnMessageTapped(msg);
    }

    ChatParticipant? GetParticipant(string senderId)
    {
        var participants = chatView.Participants;
        if (participants is null)
            return null;

        for (var i = 0; i < participants.Count; i++)
        {
            if (participants[i].Id == senderId)
                return participants[i];
        }
        return null;
    }

    static void SetTextWithLinks(Label label, string text, Color textColor)
    {
        var matches = UrlRegex.Matches(text);
        if (matches.Count == 0)
        {
            label.FormattedText = null;
            label.Text = text;
            return;
        }

        var formatted = new FormattedString();
        var lastIndex = 0;

        foreach (Match match in matches)
        {
            if (match.Index > lastIndex)
            {
                formatted.Spans.Add(new Span
                {
                    Text = text[lastIndex..match.Index],
                    TextColor = textColor
                });
            }

            var urlSpan = new Span
            {
                Text = match.Value,
                TextColor = Colors.CornflowerBlue,
                TextDecorations = TextDecorations.Underline
            };

            var url = match.Value;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = "https://" + url;

            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => _ = Launcher.OpenAsync(new Uri(url));
            urlSpan.GestureRecognizers.Add(tap);

            formatted.Spans.Add(urlSpan);
            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < text.Length)
        {
            formatted.Spans.Add(new Span
            {
                Text = text[lastIndex..],
                TextColor = textColor
            });
        }

        label.FormattedText = formatted;
    }

    [GeneratedRegex(@"(https?://[^\s]+|www\.[^\s]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex CreateUrlRegex();
}
