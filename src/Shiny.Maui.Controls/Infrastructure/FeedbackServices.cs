namespace Shiny.Maui.Controls.Infrastructure;


public class HapticFeedbackService : IFeedbackService
{
    public virtual void OnRequested(object control, string eventName, object? args)
    {
        try
        {
            if (!HapticFeedback.IsSupported)
                return;

            if (eventName.Equals("LongPress", StringComparison.OrdinalIgnoreCase))
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
    public void OnRequested(object control, string eventName, object? args = null)
    {
    }
}