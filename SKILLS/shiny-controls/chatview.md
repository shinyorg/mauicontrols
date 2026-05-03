# ChatView

A modern chat UI control with message bubbles, typing indicators, load-more pagination, and a bottom input bar. Supports single-person and multi-person conversations with per-participant colors and avatars.

## Basic Usage

```xml
<shiny:ChatView Messages="{Binding Messages}"
                Participants="{Binding Participants}"
                IsMultiPerson="True"
                TypingParticipants="{Binding TypingParticipants}"
                SendCommand="{Binding SendCommand}"
                AttachImageCommand="{Binding AttachImageCommand}"
                LoadMoreCommand="{Binding LoadMoreCommand}"
                MyBubbleColor="#DCF8C6"
                OtherBubbleColor="White" />
```

## Data Models

### ChatMessage

```csharp
public class ChatMessage
{
    public string Id { get; set; }                  // Auto-generated GUID
    public string? Text { get; set; }               // null for image messages
    public string? ImageUrl { get; set; }           // null for text messages
    public string SenderId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public bool IsFromMe { get; set; }
}
```

### ChatParticipant

```csharp
// MAUI
public class ChatParticipant
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public ImageSource? Avatar { get; set; }     // MAUI ImageSource
    public Color? BubbleColor { get; set; }      // MAUI Color
}

// Blazor
public class ChatParticipant
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string? AvatarUrl { get; set; }       // URL string
    public string? BubbleColor { get; set; }     // CSS color string
}
```

## ChatView Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Messages` | `IList<ChatMessage>` | `null` | Message collection; observes `INotifyCollectionChanged` on MAUI |
| `Participants` | `IList<ChatParticipant>` | `null` | Participant info for avatar/name/color lookup by `SenderId` |
| `IsMultiPerson` | `bool` | `false` | Show avatars and names for other participants |
| `ShowAvatarsInSingleChat` | `bool` | `false` | Force avatars/names even in single-person mode |
| `MyBubbleColor` | `Color` / `string` | `#DCF8C6` | Local user bubble color |
| `MyTextColor` | `Color` / `string` | `Black` | Local user text color |
| `OtherBubbleColor` | `Color` / `string` | `White` | Default other-user bubble color (overridden by participant's `BubbleColor`) |
| `OtherTextColor` | `Color` / `string` | `Black` | Other-user text color |
| `PlaceholderText` | `string` | `"Type a message..."` | Input field placeholder |
| `SendButtonText` | `string` | `"Send"` | Send button label |
| `IsInputBarVisible` | `bool` | `true` | Show/hide the entire input bar |
| `ShowTypingIndicator` | `bool` | `true` | Enable/disable typing indicator |
| `TypingParticipants` | `IList<ChatParticipant>` | `null` | Currently typing participants (do not include "me") |
| `ScrollToFirstUnread` | `bool` | `false` | Scroll to first unread message instead of end |
| `FirstUnreadMessageId` | `string?` | `null` | ID of the first unread message |
| `UseFeedback` | `bool` | `true` | Feedback on send (MAUI only) |

## Commands (MAUI ICommand) / Events (Blazor EventCallback)

| MAUI | Blazor | Parameter | Description |
|---|---|---|---|
| `SendCommand` | `EventCallback<string>` | text string | Fires when user sends a text message via Enter or Send button |
| `AttachImageCommand` | `EventCallback` | -- | Fires when user taps attach button; user implements own image picker |
| `LoadMoreCommand` | `EventCallback` | -- | Fires when user scrolls near top; prepend older messages to the list |

## Methods

| Method | Description |
|---|---|
| `ScrollToEnd(bool animate)` | Scroll to the latest message |
| `ScrollToMessage(string messageId, bool animate)` | Scroll to a specific message by ID |

## Features

- **Bubbles**: Left-aligned for others, right-aligned for "me", with rounded corners and a smaller tail radius on the last message in a group
- **Grouping**: Consecutive messages from the same sender within the same minute are grouped visually (2px spacing within group, 12px between groups)
- **Timestamps**: Last message in each group shows timestamp. Today = time only ("2:30 PM"); previous days = full date ("Apr 15, 2:30 PM")
- **Multi-person**: First message in each group shows avatar (initials circle or image) and display name above the bubble
- **Single-person**: Avatars and names hidden by default (set `ShowAvatarsInSingleChat` to override)
- **Per-participant colors**: Each `ChatParticipant.BubbleColor` overrides `OtherBubbleColor` for that sender
- **Typing indicator**: Animated bouncing dots with text: "{Name} is typing...", "{Name1}, {Name2} are typing", or "Multiple users are typing" (3+)
- **Link detection**: URLs in text messages are auto-detected and rendered as tappable links
- **Image messages**: `ChatMessage.ImageUrl` renders as an image bubble (text and image are mutually exclusive)
- **Virtualization**: MAUI uses `CollectionView` with `RemainingItemsThreshold` for automatic load-more
- **Input bar**: `Entry` with Enter key + Send button; optional attach button (shown when `AttachImageCommand` is set)
- **Hide input bar**: Set `IsInputBarVisible = false` for read-only chat display

## ViewModel Pattern

```csharp
public partial class ChatViewModel : ObservableObject
{
    [ObservableProperty] ObservableCollection<ChatMessage> messages = [];
    [ObservableProperty] ObservableCollection<ChatParticipant> participants = [];
    [ObservableProperty] ObservableCollection<ChatParticipant> typingParticipants = [];
    [ObservableProperty] bool isMultiPerson = true;

    [RelayCommand]
    void Send(string text)
    {
        Messages.Add(new ChatMessage
        {
            Text = text,
            SenderId = "me",
            IsFromMe = true,
            Timestamp = DateTimeOffset.Now
        });
    }

    [RelayCommand]
    void AttachImage()
    {
        // User implements own image picker, then:
        Messages.Add(new ChatMessage
        {
            ImageUrl = "https://example.com/photo.jpg",
            SenderId = "me",
            IsFromMe = true,
            Timestamp = DateTimeOffset.Now
        });
    }

    [RelayCommand]
    void LoadMore()
    {
        // Prepend older messages to the beginning of Messages
    }
}
```

## Custom Message Templates

Control how each message bubble's content is rendered using `MessageTemplate` or `MessageTemplateSelector`. The bubble chrome (avatar, name, timestamp, colors, corner radius, alignment) is still managed by ChatView — you only control what renders *inside* the bubble.

### MessageTemplate (single template for all messages)

```xml
<shiny:ChatView Messages="{Binding Messages}">
    <shiny:ChatView.MessageTemplate>
        <DataTemplate x:DataType="shiny:ChatMessage">
            <VerticalStackLayout Spacing="4">
                <Label Text="{Binding Text}" />
                <Button Text="Reply" Command="{Binding Source={RelativeSource AncestorType={x:Type vm:MyViewModel}}, Path=ReplyCommand}"
                        CommandParameter="{Binding .}" />
            </VerticalStackLayout>
        </DataTemplate>
    </shiny:ChatView.MessageTemplate>
</shiny:ChatView>
```

### MessageTemplateSelector (different templates per message type)

Subclass `ChatMessage` for different message types, then use a `DataTemplateSelector`:

```csharp
public class ActionChatMessage : ChatMessage
{
    public string ActionText { get; set; } = "Accept";
}

public class CardChatMessage : ChatMessage
{
    public string CardTitle { get; set; } = string.Empty;
}

public class ChatMessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? ActionTemplate { get; set; }
    public DataTemplate? CardTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        return item switch
        {
            ActionChatMessage => ActionTemplate,
            CardChatMessage => CardTemplate,
            _ => TextTemplate
        };
    }
}
```

```xml
<shiny:ChatView Messages="{Binding Messages}">
    <shiny:ChatView.MessageTemplateSelector>
        <local:ChatMessageTemplateSelector>
            <local:ChatMessageTemplateSelector.TextTemplate>
                <DataTemplate x:DataType="shiny:ChatMessage">
                    <Label Text="{Binding Text}" />
                </DataTemplate>
            </local:ChatMessageTemplateSelector.TextTemplate>
            <local:ChatMessageTemplateSelector.ActionTemplate>
                <DataTemplate x:DataType="local:ActionChatMessage">
                    <VerticalStackLayout Spacing="8">
                        <Label Text="{Binding Text}" />
                        <Button Text="{Binding ActionText}" />
                    </VerticalStackLayout>
                </DataTemplate>
            </local:ChatMessageTemplateSelector.ActionTemplate>
        </local:ChatMessageTemplateSelector>
    </shiny:ChatView.MessageTemplateSelector>
</shiny:ChatView>
```

| Property | Type | Default | Description |
|---|---|---|---|
| `MessageTemplate` | `DataTemplate?` | `null` | Single template for all message bubble content |
| `MessageTemplateSelector` | `DataTemplateSelector?` | `null` | Per-message-type template selector (takes priority) |

When neither is set, the default text/image rendering is used.

## Code Generation Guidance

- Use `ChatView` for any chat/messaging/conversation UI — do not hand-build bubble layouts with `CollectionView`
- Always provide a `Participants` list for multi-person chats; each participant's `BubbleColor` is optional
- `SendCommand` receives the text string — the control clears the input after sending
- `AttachImageCommand` fires a signal; the user implements their own image picker and adds a `ChatMessage` with `ImageUrl`
- `LoadMoreCommand` fires when the user scrolls near the top; prepend older messages with `Insert(0, msg)`
- `TypingParticipants` should never include the local user (the "you are typing" is excluded by design)
- Set `IsInputBarVisible = false` for read-only chat views (e.g., chat history, support logs)
- Use `MessageTemplate` for simple customization (e.g., adding action buttons to every message)
- Use `MessageTemplateSelector` when you have multiple message types (text, action buttons, cards, etc.) — subclass `ChatMessage` for each type
