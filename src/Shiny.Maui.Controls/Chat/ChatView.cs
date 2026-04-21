using System.Collections.Specialized;
using Shiny.Maui.Controls.Chat.Internal;

namespace Shiny.Maui.Controls.Chat;

public partial class ChatView : ContentView
{
    public event EventHandler<ChatMessage>? MessageTapped;

    readonly CollectionView collectionView;
    readonly ChatInputBar inputBar;
    readonly ChatTypingIndicator typingIndicator;
    readonly Border newMessagesPill;
    readonly Label newMessagesPillLabel;

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

        typingIndicator = new ChatTypingIndicator
        {
            HorizontalOptions = LayoutOptions.Start,
            Margin = new Thickness(16, 4, 0, 4),
            IsVisible = false
        };

        // CollectionView footer is just the typing indicator (inline at bottom of messages)
        collectionView.Footer = typingIndicator;

        // "New Messages" pill
        newMessagesPillLabel = new Label
        {
            TextColor = Colors.White,
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        newMessagesPill = new Border
        {
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            BackgroundColor = Color.FromArgb("#007AFF"),
            Padding = new Thickness(16, 8),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 12),
            IsVisible = false,
            Content = newMessagesPillLabel
        };

        var pillTap = new TapGestureRecognizer();
        pillTap.Tapped += OnNewMessagesPillTapped;
        newMessagesPill.GestureRecognizers.Add(pillTap);

        // Messages area: CollectionView + pill overlay
        var messageArea = new Grid
        {
            IsClippedToBounds = true,
            Children = { collectionView, newMessagesPill }
        };

        // Input bar: always visible, pinned at bottom
        inputBar = new ChatInputBar();
        inputBar.SendRequested += OnSendRequested;
        inputBar.AttachRequested += OnAttachRequested;

        // Root: messages fill available space, input bar is fixed at bottom
        var rootGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            }
        };
        rootGrid.Add(messageArea, 0, 0);
        rootGrid.Add(inputBar, 0, 1);

        Content = rootGrid;
    }
}
