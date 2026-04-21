namespace Shiny.Blazor.Controls.Chat;

public class ChatParticipant
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? BubbleColor { get; set; }
}
