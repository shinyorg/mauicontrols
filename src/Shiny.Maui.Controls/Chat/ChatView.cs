using System.Collections.Specialized;
using Shiny.Maui.Controls.Chat.Internal;

namespace Shiny.Maui.Controls.Chat;

public partial class ChatView : ContentView
{
    public event EventHandler<ChatMessage>? MessageTapped;

    readonly CollectionView collectionView;
    readonly ChatInputBar inputBar;
    readonly VerticalStackLayout typingBubbleHost;
    readonly Border toastPill;
    readonly Label toastNewMessagesLabel;
    readonly Label toastTypingLabel;

    INotifyCollectionChanged? observedCollection;
    INotifyCollectionChanged? observedTypingCollection;
    bool isLoadingMore;
    bool isNearBottom = true;
    int unreadCount;

    public ChatView()
    {
        collectionView = new CollectionView
        {
            ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Vertical)
            {
                ItemSpacing = 0
            },
            ItemTemplate = new ChatBubbleTemplateSelector(this),
            RemainingItemsThreshold = 5
        };
        collectionView.RemainingItemsThresholdReached += OnRemainingItemsThresholdReached;
        collectionView.Scrolled += OnCollectionViewScrolled;

        // Shared toast pill — shows new messages on top, typing below
        toastNewMessagesLabel = new Label
        {
            TextColor = Colors.White,
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            IsVisible = false
        };

        toastTypingLabel = new Label
        {
            TextColor = Color.FromArgb("#D0E8FF"),
            FontSize = 12,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            IsVisible = false
        };

        toastPill = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Color.FromArgb("#007AFF"),
            Padding = new Thickness(16, 8),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 12),
            IsVisible = false,
            Content = new VerticalStackLayout
            {
                Spacing = 2,
                Children = { toastNewMessagesLabel, toastTypingLabel }
            }
        };

        var pillTap = new TapGestureRecognizer();
        pillTap.Tapped += OnToastPillTapped;
        toastPill.GestureRecognizers.Add(pillTap);

        // Messages area: CollectionView + toast pill overlay
        var messageArea = new Grid
        {
            IsClippedToBounds = true,
            Children = { collectionView, toastPill }
        };

        // Typing bubble host — sits between messages and input bar, outside the CollectionView
        typingBubbleHost = new VerticalStackLayout
        {
            IsVisible = false,
            Spacing = 0
        };

        // Input bar: always visible, pinned at bottom
        inputBar = new ChatInputBar();
        inputBar.SendRequested += OnSendRequested;
        inputBar.AttachRequested += OnAttachRequested;

        // Root: messages fill space, typing bubbles below, input bar at bottom
        var rootGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            }
        };
        rootGrid.Add(messageArea, 0, 0);
        rootGrid.Add(typingBubbleHost, 0, 1);
        rootGrid.Add(inputBar, 0, 2);

        Content = rootGrid;
    }
}
