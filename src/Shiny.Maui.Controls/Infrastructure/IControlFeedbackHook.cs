namespace Shiny.Maui.Controls.Infrastructure;

public interface IControlFeedbackHook
{
    bool TryBind(VisualElement element, IFeedbackService feedback);
    bool TryUnbind(VisualElement element);
}
