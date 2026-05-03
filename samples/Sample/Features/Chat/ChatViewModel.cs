using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny.Maui.Controls.Chat;

namespace Sample.Features.Chat;

public partial class ChatViewModel : ObservableObject
{
    readonly AppSettings appSettings;
    readonly string myId = "me";

    readonly ChatParticipant alice = new()
    {
        Id = "alice",
        DisplayName = "Alice Johnson",
        BubbleColor = Color.FromArgb("#E3F2FD"),
        Avatar = ImageSource.FromFile("dotnet_bot.png")
    };

    readonly ChatParticipant bob = new()
    {
        Id = "bob",
        DisplayName = "Bob Smith",
        BubbleColor = Color.FromArgb("#FFF3E0")
    };

    public ChatViewModel(AppSettings appSettings)
    {
        this.appSettings = appSettings;
        participants.Add(alice);
        participants.Add(bob);
        LoadSampleMessages();
    }

    public bool IsChatSpeakingEnabled
    {
        get => appSettings.IsChatSpeakingEnabled;
        set
        {
            appSettings.IsChatSpeakingEnabled = value;
            OnPropertyChanged();
        }
    }

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
            SenderId = myId,
            IsFromMe = true,
            Timestamp = DateTimeOffset.Now
        });

        // Simulate a reply after a short delay
        _ = SimulateReplyAsync();
    }

    [RelayCommand]
    void AttachImage()
    {
        Messages.Add(new ChatMessage
        {
            ImageUrl = "https://picsum.photos/300/200",
            SenderId = myId,
            IsFromMe = true,
            Timestamp = DateTimeOffset.Now
        });
    }

    [RelayCommand]
    void LoadMore()
    {
        var earliest = Messages.Count > 0
            ? Messages[0].Timestamp.AddMinutes(-5)
            : DateTimeOffset.Now.AddHours(-2);

        var olderMessages = new List<ChatMessage>
        {
            new()
            {
                Text = "This is an older message that was loaded.",
                SenderId = alice.Id,
                Timestamp = earliest.AddMinutes(-3)
            },
            new()
            {
                Text = "And another one from earlier in the conversation.",
                SenderId = myId,
                IsFromMe = true,
                Timestamp = earliest.AddMinutes(-2)
            },
            new()
            {
                Text = "Check this link: https://github.com/shinyorg/controls",
                SenderId = bob.Id,
                Timestamp = earliest.AddMinutes(-1)
            }
        };

        for (var i = olderMessages.Count - 1; i >= 0; i--)
            Messages.Insert(0, olderMessages[i]);
    }

    [RelayCommand]
    async Task SimulateIncoming()
    {
        TypingParticipants.Add(bob);
        await Task.Delay(3000);
        TypingParticipants.Remove(bob);

        Messages.Add(new ChatMessage
        {
            Text = "Hey, just checking in! Did you see the latest build?",
            SenderId = bob.Id,
            Timestamp = DateTimeOffset.Now
        });
    }

    [RelayCommand]
    void ToggleTyping()
    {
        if (TypingParticipants.Count == 0)
            TypingParticipants.Add(alice);
        else if (TypingParticipants.Count == 1)
            TypingParticipants.Add(bob);
        else
            TypingParticipants.Clear();
    }

    void LoadSampleMessages()
    {
        var now = DateTimeOffset.Now;
        var yesterday = now.AddDays(-1);

        Messages.Add(new ChatMessage
        {
            Text = "Hey everyone! Has anyone tried the new controls library?",
            SenderId = alice.Id,
            Timestamp = yesterday.AddHours(-2)
        });

        Messages.Add(new ChatMessage
        {
            Text = "Yes! The TableView is really nice.",
            SenderId = bob.Id,
            Timestamp = yesterday.AddHours(-2).AddMinutes(1)
        });

        Messages.Add(new ChatMessage
        {
            Text = "I agree, the styling system is great.",
            SenderId = myId,
            IsFromMe = true,
            Timestamp = yesterday.AddHours(-1).AddMinutes(30)
        });

        Messages.Add(new ChatMessage
        {
            Text = "Check out https://github.com/shinyorg/controls for the latest updates.",
            SenderId = alice.Id,
            Timestamp = now.AddMinutes(-30)
        });

        Messages.Add(new ChatMessage
        {
            Text = "The scheduler component is my favorite so far.",
            SenderId = alice.Id,
            Timestamp = now.AddMinutes(-30)
        });

        Messages.Add(new ChatMessage
        {
            Text = "Same here! Really clean API.",
            SenderId = bob.Id,
            Timestamp = now.AddMinutes(-28)
        });

        Messages.Add(new ChatMessage
        {
            Text = "Here's a screenshot of what I built:",
            SenderId = myId,
            IsFromMe = true,
            Timestamp = now.AddMinutes(-5)
        });

        Messages.Add(new ChatMessage
        {
            ImageUrl = "https://picsum.photos/300/200",
            SenderId = myId,
            IsFromMe = true,
            Timestamp = now.AddMinutes(-5)
        });
    }

    async Task SimulateReplyAsync()
    {
        TypingParticipants.Add(alice);
        await Task.Delay(2000);
        TypingParticipants.Remove(alice);

        Messages.Add(new ChatMessage
        {
            Text = "That's great! Thanks for sharing.",
            SenderId = alice.Id,
            Timestamp = DateTimeOffset.Now
        });
    }
}
