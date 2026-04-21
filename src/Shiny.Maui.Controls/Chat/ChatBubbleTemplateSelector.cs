using Shiny.Maui.Controls.Chat.Internal;

namespace Shiny.Maui.Controls.Chat;

class ChatBubbleTemplateSelector : DataTemplateSelector
{
    readonly ChatView chatView;
    readonly DataTemplate myTemplate;
    readonly DataTemplate otherTemplate;

    public ChatBubbleTemplateSelector(ChatView chatView)
    {
        this.chatView = chatView;
        myTemplate = new DataTemplate(() => new ChatBubbleView(chatView, isMe: true));
        otherTemplate = new DataTemplate(() => new ChatBubbleView(chatView, isMe: false));
    }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        return item is ChatMessage msg && msg.IsFromMe ? myTemplate : otherTemplate;
    }
}
