using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Shiny.Maui.Controls.Infrastructure;

public class ControlFeedbackHook<TControl> : IControlFeedbackHook
    where TControl : VisualElement
{
    readonly string eventName;
    readonly Action<TControl, EventHandler> subscribe;
    readonly Action<TControl, EventHandler> unsubscribe;
    readonly ConditionalWeakTable<TControl, EventHandler> handlers = new();

    public ControlFeedbackHook(
        string eventName,
        Action<TControl, EventHandler> subscribe,
        Action<TControl, EventHandler> unsubscribe)
    {
        this.eventName = eventName;
        this.subscribe = subscribe;
        this.unsubscribe = unsubscribe;
    }

    public bool TryBind(VisualElement element, IFeedbackService feedback)
    {
        if (element is not TControl control)
            return false;

        EventHandler handler = (s, e) => feedback.OnRequested(s!, eventName, e);
        handlers.AddOrUpdate(control, handler);
        subscribe(control, handler);
        return true;
    }

    public bool TryUnbind(VisualElement element)
    {
        if (element is not TControl control)
            return false;

        if (handlers.TryGetValue(control, out var handler))
        {
            unsubscribe(control, handler);
            handlers.Remove(control);
        }
        return true;
    }
}

public class ControlFeedbackHook<TControl, TEventArgs> : IControlFeedbackHook
    where TControl : VisualElement
    where TEventArgs : EventArgs
{
    readonly string eventName;
    readonly Action<TControl, EventHandler<TEventArgs>> subscribe;
    readonly Action<TControl, EventHandler<TEventArgs>> unsubscribe;
    readonly ConditionalWeakTable<TControl, EventHandler<TEventArgs>> handlers = new();

    public ControlFeedbackHook(
        string eventName,
        Action<TControl, EventHandler<TEventArgs>> subscribe,
        Action<TControl, EventHandler<TEventArgs>> unsubscribe)
    {
        this.eventName = eventName;
        this.subscribe = subscribe;
        this.unsubscribe = unsubscribe;
    }

    public bool TryBind(VisualElement element, IFeedbackService feedback)
    {
        if (element is not TControl control)
            return false;

        EventHandler<TEventArgs> handler = (s, e) => feedback.OnRequested(s!, eventName, e);
        handlers.AddOrUpdate(control, handler);
        subscribe(control, handler);
        return true;
    }

    public bool TryUnbind(VisualElement element)
    {
        if (element is not TControl control)
            return false;

        if (handlers.TryGetValue(control, out var handler))
        {
            unsubscribe(control, handler);
            handlers.Remove(control);
        }
        return true;
    }
}

class TimePickerFeedbackHook : IControlFeedbackHook
{
    readonly ConditionalWeakTable<TimePicker, PropertyChangedEventHandler> handlers = new();

    public bool TryBind(VisualElement element, IFeedbackService feedback)
    {
        if (element is not TimePicker tp)
            return false;

        PropertyChangedEventHandler handler = (s, e) =>
        {
            if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
                feedback.OnRequested(s!, "TimeChanged", e);
        };
        handlers.AddOrUpdate(tp, handler);
        tp.PropertyChanged += handler;
        return true;
    }

    public bool TryUnbind(VisualElement element)
    {
        if (element is not TimePicker tp)
            return false;

        if (handlers.TryGetValue(tp, out var handler))
        {
            tp.PropertyChanged -= handler;
            handlers.Remove(tp);
        }
        return true;
    }
}
