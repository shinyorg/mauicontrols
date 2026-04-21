using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Shiny.Blazor.Controls.Chat;

public partial class ChatView : IAsyncDisposable
{
    static readonly Regex UrlRegex = new(
        @"(https?://[^\s]+|www\.[^\s]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    IJSObjectReference? module;
    DotNetObjectReference<ChatView>? selfRef;
    ElementReference rootEl;
    ElementReference messagesEl;
    ElementReference inputEl;
    string? inputText;
    string? typingText;
    int lastMessageCount;
    bool initialized;
    bool isNearBottom = true;
    int unreadCount;

    // Data
    [Parameter] public IList<ChatMessage>? Messages { get; set; }
    [Parameter] public IList<ChatParticipant>? Participants { get; set; }
    [Parameter] public bool IsMultiPerson { get; set; }
    [Parameter] public bool ShowAvatarsInSingleChat { get; set; }

    // Colors (CSS strings)
    [Parameter] public string MyBubbleColor { get; set; } = "#DCF8C6";
    [Parameter] public string MyTextColor { get; set; } = "#000000";
    [Parameter] public string OtherBubbleColor { get; set; } = "#FFFFFF";
    [Parameter] public string OtherTextColor { get; set; } = "#000000";

    // Input
    [Parameter] public string PlaceholderText { get; set; } = "Type a message...";
    [Parameter] public string SendButtonText { get; set; } = "Send";
    [Parameter] public bool IsInputBarVisible { get; set; } = true;

    // Typing
    [Parameter] public bool ShowTypingIndicator { get; set; } = true;
    [Parameter] public IList<ChatParticipant>? TypingParticipants { get; set; }

    // Commands/Events
    [Parameter] public EventCallback<string> SendCommand { get; set; }
    [Parameter] public EventCallback AttachImageCommand { get; set; }
    [Parameter] public EventCallback LoadMoreCommand { get; set; }
    [Parameter] public EventCallback<ChatMessage> MessageTappedCommand { get; set; }

    // Scroll
    [Parameter] public bool ScrollToFirstUnread { get; set; }
    [Parameter] public string? FirstUnreadMessageId { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            module = await JS.InvokeAsync<IJSObjectReference>(
                "import",
                "./_content/Shiny.Blazor.Controls/chat.js");

            selfRef = DotNetObjectReference.Create(this);
            await module.InvokeVoidAsync("init", messagesEl, selfRef);
            initialized = true;

            // Initial scroll
            await PerformInitialScrollAsync();
            lastMessageCount = Messages?.Count ?? 0;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        UpdateTypingText();

        if (!initialized || module is null)
            return;

        var currentCount = Messages?.Count ?? 0;
        if (currentCount != lastMessageCount)
        {
            var wasAppended = currentCount > lastMessageCount;
            lastMessageCount = currentCount;

            // Let the DOM update first
            await Task.Yield();

            if (wasAppended)
            {
                // Check if the newest message is from the local user
                var newest = Messages![currentCount - 1];

                if (newest.IsFromMe)
                {
                    // Always scroll to bottom for own messages
                    unreadCount = 0;
                    await PerformInitialScrollAsync();
                }
                else
                {
                    isNearBottom = await module.InvokeAsync<bool>("isNearBottom", messagesEl);

                    if (isNearBottom)
                    {
                        unreadCount = 0;
                        await PerformInitialScrollAsync();
                    }
                    else
                    {
                        unreadCount++;
                    }
                }
            }
        }
    }

    async Task PerformInitialScrollAsync()
    {
        if (module is null || Messages is not { Count: > 0 })
            return;

        if (ScrollToFirstUnread && FirstUnreadMessageId is not null)
        {
            var index = FindMessageIndex(FirstUnreadMessageId);
            if (index >= 0)
            {
                await module.InvokeVoidAsync("scrollToMessage", messagesEl, index);
                return;
            }
        }

        await module.InvokeVoidAsync("scrollToEnd", messagesEl, false);
    }

    int FindMessageIndex(string messageId)
    {
        if (Messages is null)
            return -1;

        for (var i = 0; i < Messages.Count; i++)
        {
            if (Messages[i].Id == messageId)
                return i;
        }
        return -1;
    }

    void UpdateTypingText()
    {
        if (!ShowTypingIndicator || TypingParticipants is not { Count: > 0 })
        {
            typingText = null;
            return;
        }

        var p = TypingParticipants;
        typingText = p.Count switch
        {
            1 => $"{p[0].DisplayName} is typing\u2026",
            2 => $"{p[0].DisplayName}, {p[1].DisplayName} are typing\u2026",
            3 => $"{p[0].DisplayName}, {p[1].DisplayName}, {p[2].DisplayName} are typing\u2026",
            _ => "Multiple users are typing\u2026"
        };
    }

    ChatParticipant? GetParticipant(string senderId)
    {
        if (Participants is null)
            return null;

        for (var i = 0; i < Participants.Count; i++)
        {
            if (Participants[i].Id == senderId)
                return Participants[i];
        }
        return null;
    }

    bool ShouldShowAvatar(ChatMessage message, bool isFirstInGroup)
    {
        if (message.IsFromMe || !isFirstInGroup)
            return false;

        return IsMultiPerson || ShowAvatarsInSingleChat;
    }

    async Task OnSendClick()
    {
        await TrySendAsync();
    }

    async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
            await TrySendAsync();
    }

    async Task TrySendAsync()
    {
        var text = inputText?.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        inputText = string.Empty;

        if (SendCommand.HasDelegate)
            await SendCommand.InvokeAsync(text);
    }

    async Task OnMessageTap(ChatMessage msg)
    {
        if (MessageTappedCommand.HasDelegate)
            await MessageTappedCommand.InvokeAsync(msg);
    }

    async Task OnAttachClick()
    {
        if (AttachImageCommand.HasDelegate)
            await AttachImageCommand.InvokeAsync();
    }

    async Task OnLoadMoreClick()
    {
        if (module is null)
            return;

        // Save scroll position before prepending
        var prevHeight = await module.InvokeAsync<double>("getScrollHeight", messagesEl);

        if (LoadMoreCommand.HasDelegate)
            await LoadMoreCommand.InvokeAsync();

        // After items prepended, maintain scroll position
        await Task.Yield();
        await module.InvokeVoidAsync("maintainScrollPosition", messagesEl, prevHeight);
    }

    async Task OnNewMessagesPillClick()
    {
        unreadCount = 0;
        if (module is not null)
            await module.InvokeVoidAsync("scrollToEnd", messagesEl, true);
    }

    [JSInvokable]
    public async Task OnLoadMoreThreshold()
    {
        if (LoadMoreCommand.HasDelegate)
            await LoadMoreCommand.InvokeAsync();
    }

    static string Linkify(string text)
    {
        return UrlRegex.Replace(text, match =>
        {
            var url = match.Value;
            var href = url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                       url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                ? url
                : "https://" + url;
            return $"<a href=\"{System.Net.WebUtility.HtmlEncode(href)}\" target=\"_blank\" rel=\"noopener noreferrer\">{System.Net.WebUtility.HtmlEncode(url)}</a>";
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (module is not null)
        {
            try { await module.InvokeVoidAsync("dispose", messagesEl); } catch { }
            try { await module.DisposeAsync(); } catch { }
        }
        selfRef?.Dispose();
    }
}
