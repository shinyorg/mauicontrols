namespace Shiny.Blazor.Controls;

public interface IFeedbackService
{
    // ie: when chat view receives a message that is not "from user", the event could be "ExternalChatMessage" which engages a Text-To-Speech Service
    void OnRequested(Type controlType, string eventName, string? details = null);
}

public class HapticFeedbackService : IFeedbackService
{
    public void OnRequested(Type controlType, string eventName, string? details)
    {
        try
        {
            if (details?.Equals("LongPress") ?? false)
                HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            else
                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
            // Haptic feedback may not be available on all platforms/devices
        }
    }
}


class NoFeedbackService : IFeedbackService
{
    public void OnRequested(Type controlType, string eventName, string? details = null)
    {
    }
}