namespace Shiny.Maui.Controls;

public interface IFeedbackService
{
    // ie: when chat view receives a message that is not "from user", the event could be "ExternalChatMessage" which engages a Text-To-Speech Service
    void OnRequested(Type controlType, string eventName, string? details = null);
}