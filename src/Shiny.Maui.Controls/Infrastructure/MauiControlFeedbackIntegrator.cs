namespace Shiny.Maui.Controls.Infrastructure;

public class MauiControlFeedbackIntegrator(IReadOnlyList<IControlFeedbackHook> hooks) : IMauiInitializeService
{
    IFeedbackService? feedback;

    public void Initialize(IServiceProvider services)
    {
        feedback = services.GetService<IFeedbackService>();
        if (feedback == null)
            return;

        var app = services.GetRequiredService<IApplication>();
        if (app is Application application)
            Attach(application);
    }

    void Attach(Application application)
    {
        foreach (var descendant in application.GetVisualTreeDescendants().Skip(1))
        {
            if (descendant is VisualElement element)
                BindElement(element);
        }

        application.DescendantAdded += OnDescendantAdded;
        application.DescendantRemoved += OnDescendantRemoved;
    }

    void OnDescendantAdded(object? sender, ElementEventArgs e)
    {
        if (e.Element is VisualElement element)
            BindElement(element);
    }

    void OnDescendantRemoved(object? sender, ElementEventArgs e)
    {
        if (e.Element is VisualElement element)
            UnbindElement(element);
    }

    void BindElement(VisualElement element)
    {
        foreach (var hook in hooks)
        {
            if (hook.TryBind(element, feedback!))
                break;
        }
    }

    void UnbindElement(VisualElement element)
    {
        foreach (var hook in hooks)
        {
            if (hook.TryUnbind(element))
                break;
        }
    }
}
