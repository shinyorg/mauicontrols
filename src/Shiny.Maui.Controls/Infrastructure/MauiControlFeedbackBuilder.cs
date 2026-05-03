namespace Shiny.Maui.Controls.Infrastructure;

public class MauiControlFeedbackBuilder
{
    internal List<IControlFeedbackHook> Hooks { get; } = [];

    public MauiControlFeedbackBuilder Hook<TControl>(
        string eventName,
        Action<TControl, EventHandler> subscribe,
        Action<TControl, EventHandler> unsubscribe)
        where TControl : VisualElement
    {
        Hooks.Add(new ControlFeedbackHook<TControl>(eventName, subscribe, unsubscribe));
        return this;
    }

    public MauiControlFeedbackBuilder Hook<TControl, TEventArgs>(
        string eventName,
        Action<TControl, EventHandler<TEventArgs>> subscribe,
        Action<TControl, EventHandler<TEventArgs>> unsubscribe)
        where TControl : VisualElement
        where TEventArgs : EventArgs
    {
        Hooks.Add(new ControlFeedbackHook<TControl, TEventArgs>(eventName, subscribe, unsubscribe));
        return this;
    }

    public MauiControlFeedbackBuilder AddDefaults()
    {
        Hook<Button>(nameof(Button.Clicked),
            (btn, h) => btn.Clicked += h,
            (btn, h) => btn.Clicked -= h);

        Hook<Entry, TextChangedEventArgs>(nameof(Entry.TextChanged),
            (e, h) => e.TextChanged += h,
            (e, h) => e.TextChanged -= h);

        Hook<Slider, ValueChangedEventArgs>(nameof(Slider.ValueChanged),
            (s, h) => s.ValueChanged += h,
            (s, h) => s.ValueChanged -= h);

        Hook<Switch, ToggledEventArgs>(nameof(Switch.Toggled),
            (s, h) => s.Toggled += h,
            (s, h) => s.Toggled -= h);

        Hook<CheckBox, CheckedChangedEventArgs>(nameof(CheckBox.CheckedChanged),
            (cb, h) => cb.CheckedChanged += h,
            (cb, h) => cb.CheckedChanged -= h);

        Hook<DatePicker, DateChangedEventArgs>(nameof(DatePicker.DateSelected),
            (dp, h) => dp.DateSelected += h,
            (dp, h) => dp.DateSelected -= h);

        Hooks.Add(new TimePickerFeedbackHook());

        Hook<Picker>(nameof(Picker.SelectedIndexChanged),
            (p, h) => p.SelectedIndexChanged += h,
            (p, h) => p.SelectedIndexChanged -= h);

        Hook<SearchBar>(nameof(SearchBar.SearchButtonPressed),
            (sb, h) => sb.SearchButtonPressed += h,
            (sb, h) => sb.SearchButtonPressed -= h);

        Hook<Stepper, ValueChangedEventArgs>(nameof(Stepper.ValueChanged),
            (s, h) => s.ValueChanged += h,
            (s, h) => s.ValueChanged -= h);

        Hook<Editor, TextChangedEventArgs>(nameof(Editor.TextChanged),
            (e, h) => e.TextChanged += h,
            (e, h) => e.TextChanged -= h);

        Hook<RadioButton, CheckedChangedEventArgs>(nameof(RadioButton.CheckedChanged),
            (rb, h) => rb.CheckedChanged += h,
            (rb, h) => rb.CheckedChanged -= h);

        return this;
    }
}
