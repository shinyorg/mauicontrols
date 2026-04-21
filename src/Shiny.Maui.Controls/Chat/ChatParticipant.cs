namespace Shiny.Maui.Controls.Chat;

public class ChatParticipant
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public ImageSource? Avatar { get; set; }
    public Color? BubbleColor { get; set; }
}
