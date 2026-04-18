namespace Shiny.Maui.Controls.Infrastructure;

static class HapticHelper
{
    public static void PerformClick()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch
        {
            // Haptic feedback may not be available on all platforms/devices
        }
    }

    public static void PerformLongPress()
    {
        try
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        }
        catch
        {
            // Haptic feedback may not be available on all platforms/devices
        }
    }
}
