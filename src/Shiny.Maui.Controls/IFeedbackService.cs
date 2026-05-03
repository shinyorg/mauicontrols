namespace Shiny.Maui.Controls;

public interface IFeedbackService
{
    void OnRequested(object control, string eventName, object? args = null);
}