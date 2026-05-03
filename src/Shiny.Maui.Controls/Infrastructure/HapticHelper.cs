namespace Shiny.Maui.Controls.Infrastructure;

static class FeedbackHelper
{
    static IFeedbackService? resolved;

    static IFeedbackService? Service => resolved ??=
#if ANDROID || IOS || MACCATALYST
        IPlatformApplication.Current?.Services?.GetService<IFeedbackService>();
#else
        null;
#endif

    public static void Execute(object control, string eventName, object? args = null)
    {
        Service?.OnRequested(control, eventName, args);
    }
}
