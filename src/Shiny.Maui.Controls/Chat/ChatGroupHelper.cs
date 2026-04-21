namespace Shiny.Maui.Controls.Chat;

static class ChatGroupHelper
{
    public static bool IsNewGroup(ChatMessage current, ChatMessage? previous)
    {
        if (previous is null)
            return true;

        if (current.SenderId != previous.SenderId)
            return true;

        var currentMinute = new DateTimeOffset(
            current.Timestamp.Year, current.Timestamp.Month, current.Timestamp.Day,
            current.Timestamp.Hour, current.Timestamp.Minute, 0, current.Timestamp.Offset);

        var prevMinute = new DateTimeOffset(
            previous.Timestamp.Year, previous.Timestamp.Month, previous.Timestamp.Day,
            previous.Timestamp.Hour, previous.Timestamp.Minute, 0, previous.Timestamp.Offset);

        return currentMinute != prevMinute;
    }

    public static string FormatTimestamp(DateTimeOffset timestamp)
    {
        var today = DateTimeOffset.Now.Date;
        return timestamp.Date == today
            ? timestamp.ToString("h:mm tt")
            : timestamp.ToString("MMM d, h:mm tt");
    }

    public static string GetInitials(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return "?";

        var parts = displayName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();

        return parts[0][0].ToString().ToUpperInvariant();
    }
}
