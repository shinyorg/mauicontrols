namespace Shiny.Maui.Controls.Chat;

public class ChatMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;
    public bool IsFromMe { get; set; }
}
