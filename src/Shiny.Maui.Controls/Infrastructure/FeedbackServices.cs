namespace Shiny.Maui.Controls.Infrastructure;


public class HapticFeedbackService : IFeedbackService
{
    public virtual void OnRequested(Type controlType, string eventName, string? details)
    {
        try
        {
            if (!HapticFeedback.IsSupported)
                return;
            
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