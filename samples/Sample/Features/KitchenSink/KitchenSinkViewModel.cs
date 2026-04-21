using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shiny;
using Shiny.Maui.Controls;
using Shiny.Maui.Controls.Chat;

namespace Sample.Features.KitchenSink;

public partial class KitchenSinkViewModel(INavigator navigator) : ObservableObject
{
    public ObservableCollection<DetentValue> SheetDetents { get; } = [DetentValue.Half, DetentValue.Full];
    readonly string myId = "me";

    readonly ChatDefinition[] chatDefinitions =
    [
        new("alice", "Alice Johnson", "#E3F2FD", [
            ("Hey! Just got back from the hike.", -60),
            ("The views were incredible.", -59),
            ("https://picsum.photos/seed/mountain/400/300", -58),
            ("You should come next time!", -55)
        ]),
        new("bob", "Bob Smith", "#FFF3E0", [
            ("Did you see the game last night?", -120),
            ("What a finish!", -119),
            ("https://picsum.photos/seed/stadium/400/300", -90),
            ("We should watch the next one together.", -85)
        ]),
        new("carol", "Carol Davis", "#F3E5F5", [
            ("Meeting moved to 3pm.", -30),
            ("Can you bring the design mockups?", -28),
            ("https://picsum.photos/seed/office/400/300", -20),
            ("Thanks!", -18)
        ])
    ];

    [ObservableProperty] bool isChatOpen;
    [ObservableProperty] bool isImageViewerOpen;
    [ObservableProperty] string? viewerImageSource;
    [ObservableProperty] string chatTitle = string.Empty;
    [ObservableProperty] ObservableCollection<ChatMessage> messages = [];
    [ObservableProperty] ObservableCollection<ChatParticipant> participants = [];
    [ObservableProperty] ObservableCollection<ChatParticipant> typingParticipants = [];

    ChatDefinition? activeChatDef;

    [RelayCommand]
    void OpenChat(string participantId)
    {
        activeChatDef = chatDefinitions.FirstOrDefault(c => c.Id == participantId);
        if (activeChatDef is null)
            return;

        ChatTitle = activeChatDef.DisplayName;

        var participant = new ChatParticipant
        {
            Id = activeChatDef.Id,
            DisplayName = activeChatDef.DisplayName,
            BubbleColor = Color.FromArgb(activeChatDef.BubbleColorHex),
            Avatar = activeChatDef.Id == "alice" ? ImageSource.FromFile("dotnet_bot.png") : null
        };

        Participants = [participant];
        Messages = [];

        var now = DateTimeOffset.Now;
        foreach (var (content, minutesAgo) in activeChatDef.SeedMessages)
        {
            var isImage = content.StartsWith("https://");
            Messages.Add(new ChatMessage
            {
                Text = isImage ? null : content,
                ImageUrl = isImage ? content : null,
                SenderId = activeChatDef.Id,
                Timestamp = now.AddMinutes(minutesAgo)
            });
        }

        // Add a greeting from "me"
        Messages.Add(new ChatMessage
        {
            Text = $"Hey {activeChatDef.DisplayName}!",
            SenderId = myId,
            IsFromMe = true,
            Timestamp = now.AddMinutes(-10)
        });

        IsChatOpen = true;
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

        _ = SimulateReplyAsync();
    }

    [RelayCommand]
    void MessageTapped(ChatMessage msg)
    {
        if (msg.ImageUrl is null)
            return;

        ViewerImageSource = msg.ImageUrl;
        IsImageViewerOpen = true;
    }

    [RelayCommand]
    async Task EditImage()
    {
        if (ViewerImageSource is null)
            return;

        IsImageViewerOpen = false;

        // Download the image bytes and navigate to the editor
        using var client = new HttpClient();
        var bytes = await client.GetByteArrayAsync(ViewerImageSource);

        await navigator.NavigateTo<ImageEditor.ImageEditorViewModel>(vm => vm.ImageData = bytes);
    }

    async Task SimulateReplyAsync()
    {
        if (activeChatDef is null)
            return;

        var participant = Participants.FirstOrDefault(p => p.Id == activeChatDef.Id);
        if (participant is null)
            return;

        TypingParticipants.Add(participant);
        await Task.Delay(1500);
        TypingParticipants.Remove(participant);

        var replies = new[]
        {
            "That's awesome!",
            "Tell me more!",
            "Ha, nice one 😄",
            "Sounds good to me.",
            "Let's do it!"
        };

        Messages.Add(new ChatMessage
        {
            Text = replies[Random.Shared.Next(replies.Length)],
            SenderId = activeChatDef.Id,
            Timestamp = DateTimeOffset.Now
        });
    }

    record ChatDefinition(
        string Id,
        string DisplayName,
        string BubbleColorHex,
        (string Content, int MinutesAgo)[] SeedMessages);
}
