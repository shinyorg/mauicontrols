using System.Windows.Input;

namespace Shiny.Maui.Controls.Chat;

public partial class ChatView
{
    // Data
    public static readonly BindableProperty MessagesProperty = BindableProperty.Create(
        nameof(Messages),
        typeof(IList<ChatMessage>),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).OnMessagesChanged());

    public IList<ChatMessage>? Messages
    {
        get => (IList<ChatMessage>?)GetValue(MessagesProperty);
        set => SetValue(MessagesProperty, value);
    }

    public static readonly BindableProperty ParticipantsProperty = BindableProperty.Create(
        nameof(Participants),
        typeof(IList<ChatParticipant>),
        typeof(ChatView));

    public IList<ChatParticipant>? Participants
    {
        get => (IList<ChatParticipant>?)GetValue(ParticipantsProperty);
        set => SetValue(ParticipantsProperty, value);
    }

    public static readonly BindableProperty IsMultiPersonProperty = BindableProperty.Create(
        nameof(IsMultiPerson),
        typeof(bool),
        typeof(ChatView),
        false,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public bool IsMultiPerson
    {
        get => (bool)GetValue(IsMultiPersonProperty);
        set => SetValue(IsMultiPersonProperty, value);
    }

    public static readonly BindableProperty ShowAvatarsInSingleChatProperty = BindableProperty.Create(
        nameof(ShowAvatarsInSingleChat),
        typeof(bool),
        typeof(ChatView),
        false,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public bool ShowAvatarsInSingleChat
    {
        get => (bool)GetValue(ShowAvatarsInSingleChatProperty);
        set => SetValue(ShowAvatarsInSingleChatProperty, value);
    }

    // Bubble colors
    public static readonly BindableProperty MyBubbleColorProperty = BindableProperty.Create(
        nameof(MyBubbleColor),
        typeof(Color),
        typeof(ChatView),
        Color.FromArgb("#DCF8C6"));

    public Color MyBubbleColor
    {
        get => (Color)GetValue(MyBubbleColorProperty);
        set => SetValue(MyBubbleColorProperty, value);
    }

    public static readonly BindableProperty MyTextColorProperty = BindableProperty.Create(
        nameof(MyTextColor),
        typeof(Color),
        typeof(ChatView),
        Colors.Black);

    public Color MyTextColor
    {
        get => (Color)GetValue(MyTextColorProperty);
        set => SetValue(MyTextColorProperty, value);
    }

    public static readonly BindableProperty OtherBubbleColorProperty = BindableProperty.Create(
        nameof(OtherBubbleColor),
        typeof(Color),
        typeof(ChatView),
        Colors.White);

    public Color OtherBubbleColor
    {
        get => (Color)GetValue(OtherBubbleColorProperty);
        set => SetValue(OtherBubbleColorProperty, value);
    }

    public static readonly BindableProperty OtherTextColorProperty = BindableProperty.Create(
        nameof(OtherTextColor),
        typeof(Color),
        typeof(ChatView),
        Colors.Black);

    public Color OtherTextColor
    {
        get => (Color)GetValue(OtherTextColorProperty);
        set => SetValue(OtherTextColorProperty, value);
    }

    // Input bar
    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(
        nameof(PlaceholderText),
        typeof(string),
        typeof(ChatView),
        "Type a message...",
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.PlaceholderText = (string)n);

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public static readonly BindableProperty SendButtonTextProperty = BindableProperty.Create(
        nameof(SendButtonText),
        typeof(string),
        typeof(ChatView),
        "Send",
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.SendButtonText = (string)n);

    public string SendButtonText
    {
        get => (string)GetValue(SendButtonTextProperty);
        set => SetValue(SendButtonTextProperty, value);
    }

    public static readonly BindableProperty IsInputBarVisibleProperty = BindableProperty.Create(
        nameof(IsInputBarVisible),
        typeof(bool),
        typeof(ChatView),
        true,
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.IsVisible = (bool)n);

    public bool IsInputBarVisible
    {
        get => (bool)GetValue(IsInputBarVisibleProperty);
        set => SetValue(IsInputBarVisibleProperty, value);
    }

    // Typing
    public static readonly BindableProperty ShowTypingIndicatorProperty = BindableProperty.Create(
        nameof(ShowTypingIndicator),
        typeof(bool),
        typeof(ChatView),
        true,
        propertyChanged: (b, _, _) => ((ChatView)b).SyncTypingBubbles());

    public bool ShowTypingIndicator
    {
        get => (bool)GetValue(ShowTypingIndicatorProperty);
        set => SetValue(ShowTypingIndicatorProperty, value);
    }

    public static readonly BindableProperty TypingParticipantsProperty = BindableProperty.Create(
        nameof(TypingParticipants),
        typeof(IList<ChatParticipant>),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).OnTypingParticipantsChanged());

    public IList<ChatParticipant>? TypingParticipants
    {
        get => (IList<ChatParticipant>?)GetValue(TypingParticipantsProperty);
        set => SetValue(TypingParticipantsProperty, value);
    }

    // Commands
    public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create(
        nameof(LoadMoreCommand),
        typeof(ICommand),
        typeof(ChatView));

    public ICommand? LoadMoreCommand
    {
        get => (ICommand?)GetValue(LoadMoreCommandProperty);
        set => SetValue(LoadMoreCommandProperty, value);
    }

    public static readonly BindableProperty SendCommandProperty = BindableProperty.Create(
        nameof(SendCommand),
        typeof(ICommand),
        typeof(ChatView));

    public ICommand? SendCommand
    {
        get => (ICommand?)GetValue(SendCommandProperty);
        set => SetValue(SendCommandProperty, value);
    }

    public static readonly BindableProperty AttachImageCommandProperty = BindableProperty.Create(
        nameof(AttachImageCommand),
        typeof(ICommand),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, n) => ((ChatView)b).inputBar.ShowAttachButton = n is not null);

    public ICommand? AttachImageCommand
    {
        get => (ICommand?)GetValue(AttachImageCommandProperty);
        set => SetValue(AttachImageCommandProperty, value);
    }

    public static readonly BindableProperty MessageTappedCommandProperty = BindableProperty.Create(
        nameof(MessageTappedCommand),
        typeof(ICommand),
        typeof(ChatView));

    public ICommand? MessageTappedCommand
    {
        get => (ICommand?)GetValue(MessageTappedCommandProperty);
        set => SetValue(MessageTappedCommandProperty, value);
    }

    // Scroll behavior
    public static readonly BindableProperty ScrollToFirstUnreadProperty = BindableProperty.Create(
        nameof(ScrollToFirstUnread),
        typeof(bool),
        typeof(ChatView),
        false);

    public bool ScrollToFirstUnread
    {
        get => (bool)GetValue(ScrollToFirstUnreadProperty);
        set => SetValue(ScrollToFirstUnreadProperty, value);
    }

    public static readonly BindableProperty FirstUnreadMessageIdProperty = BindableProperty.Create(
        nameof(FirstUnreadMessageId),
        typeof(string),
        typeof(ChatView));

    public string? FirstUnreadMessageId
    {
        get => (string?)GetValue(FirstUnreadMessageIdProperty);
        set => SetValue(FirstUnreadMessageIdProperty, value);
    }

    // Message template
    public static readonly BindableProperty MessageTemplateProperty = BindableProperty.Create(
        nameof(MessageTemplate),
        typeof(DataTemplate),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public DataTemplate? MessageTemplate
    {
        get => (DataTemplate?)GetValue(MessageTemplateProperty);
        set => SetValue(MessageTemplateProperty, value);
    }

    public static readonly BindableProperty MessageTemplateSelectorProperty = BindableProperty.Create(
        nameof(MessageTemplateSelector),
        typeof(DataTemplateSelector),
        typeof(ChatView),
        null,
        propertyChanged: (b, _, _) => ((ChatView)b).RefreshBubbles());

    public DataTemplateSelector? MessageTemplateSelector
    {
        get => (DataTemplateSelector?)GetValue(MessageTemplateSelectorProperty);
        set => SetValue(MessageTemplateSelectorProperty, value);
    }

    // Haptic
    public static readonly BindableProperty UseHapticFeedbackProperty = BindableProperty.Create(
        nameof(UseHapticFeedback),
        typeof(bool),
        typeof(ChatView),
        true);

    public bool UseHapticFeedback
    {
        get => (bool)GetValue(UseHapticFeedbackProperty);
        set => SetValue(UseHapticFeedbackProperty, value);
    }
}
