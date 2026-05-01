using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny.Maui.Controls.Chat;

namespace Sample.Features.Chat;

public partial class ChatTemplatePage : ContentPage
{
    public ChatTemplatePage()
    {
        InitializeComponent();
        BindingContext = new ChatTemplateViewModel();
    }
}

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

public partial class ChatTemplateViewModel : ObservableObject
{
    readonly string myId = "me";

    readonly ChatParticipant bot = new()
    {
        Id = "bot",
        DisplayName = "Assistant",
        BubbleColor = Color.FromArgb("#F0F0F0")
    };

    [ObservableProperty] ObservableCollection<ChatMessage> messages = [];
    [ObservableProperty] ObservableCollection<ChatParticipant> participants = [];

    public ChatTemplateViewModel()
    {
        participants.Add(bot);
        LoadSampleMessages();
    }

    [RelayCommand]
    void Send(string text)
    {
        Messages.Add(new ChatMessage
        {
            Text = text,
            SenderId = myId,
            IsFromMe = true,
            Timestamp = DateTimeOffset.Now
        });
    }

    [RelayCommand]
    void Action(ActionChatMessage msg)
    {
        Messages.Add(new ChatMessage
        {
            Text = $"You accepted: \"{msg.Text}\"",
            SenderId = myId,
            IsFromMe = true,
            Timestamp = DateTimeOffset.Now
        });
    }

    [RelayCommand]
    void Dismiss(ActionChatMessage msg)
    {
        Messages.Add(new ChatMessage
        {
            Text = $"Dismissed.",
            SenderId = myId,
            IsFromMe = true,
            Timestamp = DateTimeOffset.Now
        });
    }

    void LoadSampleMessages()
    {
        var now = DateTimeOffset.Now;

        Messages.Add(new ChatMessage
        {
            Text = "Hey! Here are some different message types with custom templates.",
            SenderId = bot.Id,
            Timestamp = now.AddMinutes(-10)
        });

        Messages.Add(new ActionChatMessage
        {
            Text = "Would you like to schedule a meeting for tomorrow at 2pm?",
            ActionText = "Schedule",
            SenderId = bot.Id,
            Timestamp = now.AddMinutes(-8)
        });

        Messages.Add(new ChatMessage
        {
            Text = "Sure, let me check my calendar.",
            SenderId = myId,
            IsFromMe = true,
            Timestamp = now.AddMinutes(-6)
        });

        Messages.Add(new CardChatMessage
        {
            Text = "A beautiful resort in the mountains with stunning views.",
            CardTitle = "Mountain Resort Package",
            ImageUrl = "https://picsum.photos/300/150",
            SenderId = bot.Id,
            Timestamp = now.AddMinutes(-4)
        });

        Messages.Add(new ActionChatMessage
        {
            Text = "Would you like me to book this package?",
            ActionText = "Book Now",
            SenderId = bot.Id,
            Timestamp = now.AddMinutes(-3)
        });
    }
}
