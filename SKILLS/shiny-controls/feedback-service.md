# Feedback Service

An extensible feedback service that all Shiny Controls use for interaction responses. By default provides haptic feedback; replaceable with text-to-speech, sounds, analytics, or any custom behavior.

## Setup

`UseShinyControls()` registers `HapticFeedbackService` by default. Override with a custom implementation:

```csharp
builder.UseShinyControls(cfg =>
{
    cfg.SetCustomFeedback<MyFeedbackService>();
});
```

## IFeedbackService Interface

```csharp
public interface IFeedbackService
{
    void OnRequested(object control, string eventName, object? args = null);
}
```

- `control`: The actual control instance (e.g. the `ChatView`, `SecurityPin`, `Button`, etc.) — use pattern matching like `control is ChatView` to identify the source
- `eventName`: The interaction (e.g. `"MessageSent"`, `"Clicked"`, `"Opened"`)
- `args`: Optional context — for `ChatView`, this is the `ChatMessage` object; for standard MAUI controls, the native `EventArgs`; `"LongPress"` string for SecurityPin completion

## Default Implementation

```csharp
public class HapticFeedbackService : IFeedbackService
{
    public void OnRequested(object control, string eventName, object? args)
    {
        if (eventName.Equals("LongPress", StringComparison.OrdinalIgnoreCase))
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        else
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }
}
```

## Control Events

| Control | Event | Args | Description |
|---|---|---|---|
| `ChatView` | `MessageSent` | `ChatMessage` | User sent a message |
| `ChatView` | `MessageReceived` | `ChatMessage` | External message arrived |
| `ChatView` | `MessageTapped` | `ChatMessage` | Message bubble tapped |
| `ChatView` | `AttachImage` | — | Attach button tapped |
| `Fab` | `Clicked` | — | Fab tapped |
| `FabMenu` | `Toggled` | — | Menu opened/closed |
| `FabMenuItem` | `Clicked` | — | Menu item tapped |
| `FloatingPanel` | `Opened` | — | Panel opened |
| `FloatingPanel` | `Closed` | — | Panel closed |
| `FloatingPanel` | `DetentChanged` | — | Panel snapped to detent |
| `ImageViewer` | `Opened` | — | Viewer opened |
| `ImageViewer` | `Closed` | — | Viewer closed |
| `ImageViewer` | `DoubleTapped` | — | Double-tap zoom |
| `ImageEditor` | `ToolModeChanged` | mode name | Tool changed |
| `ImageEditor` | `Undo` | — | Undo |
| `ImageEditor` | `Redo` | — | Redo |
| `ImageEditor` | `Rotate` | — | Rotated |
| `ImageEditor` | `Reset` | — | Reset |
| `ImageEditor` | `CropApplied` | — | Crop applied |
| `ImageEditor` | `Saved` | — | Saved |
| `SecurityPin` | `Completed` | `"LongPress"` | All digits entered |
| `SecurityPin` | `DigitEntered` | — | Digit entered |
| `SchedulerCalendarView` | `DaySelected` | — | Day tapped |
| `SchedulerCalendarView` | `EventSelected` | — | Event tapped |
| `SchedulerAgendaView` | `EventSelected` | — | Event tapped |
| `SchedulerAgendaView` | `TimeSlotSelected` | — | Time slot tapped |
| `CellBase` | `Tapped` | — | Table cell tapped |
| `Toaster` | `Show` | toast text | Toast shown |

## Standard MAUI Control Integration

Pluggable, AOT-compatible integration that hooks feedback into standard MAUI controls without modifying them. Uses `Application.DescendantAdded`/`DescendantRemoved` to automatically bind/unbind events across the visual tree.

### With all defaults (Button, Entry, Slider, etc.)

```csharp
builder.UseShinyControls(cfg =>
{
    cfg.AddDefaultMauiControlFeedback();
});
```

### Defaults + custom hooks

```csharp
cfg.AddDefaultMauiControlFeedback(x =>
{
    // Add your own control hooks on top of the defaults
    x.Hook<MyCustomControl>(nameof(MyCustomControl.Tapped),
        (c, h) => c.Tapped += h,
        (c, h) => c.Tapped -= h);
});
```

### Custom hooks only (no defaults)

```csharp
cfg.AddMauiControlFeedback(x =>
{
    x.Hook<Button>(nameof(Button.Clicked),
        (btn, h) => btn.Clicked += h,
        (btn, h) => btn.Clicked -= h);

    x.Hook<Slider, ValueChangedEventArgs>(nameof(Slider.ValueChanged),
        (s, h) => s.ValueChanged += h,
        (s, h) => s.ValueChanged -= h);
});
```

Two `Hook` overloads:
- `Hook<TControl>(eventName, subscribe, unsubscribe)` — for `EventHandler` events (args passed as `EventArgs`)
- `Hook<TControl, TEventArgs>(eventName, subscribe, unsubscribe)` — for `EventHandler<TEventArgs>` events (typed args passed through)

### Default MAUI Control Hooks

| Control | Event | Args |
|---|---|---|
| `Button` | `Clicked` | `EventArgs` |
| `Entry` | `TextChanged` | `TextChangedEventArgs` |
| `Slider` | `ValueChanged` | `ValueChangedEventArgs` |
| `Switch` | `Toggled` | `ToggledEventArgs` |
| `CheckBox` | `CheckedChanged` | `CheckedChangedEventArgs` |
| `DatePicker` | `DateSelected` | `DateChangedEventArgs` |
| `TimePicker` | `TimeChanged` | `PropertyChangedEventArgs` |
| `Picker` | `SelectedIndexChanged` | `EventArgs` |
| `SearchBar` | `SearchButtonPressed` | `EventArgs` |
| `Stepper` | `ValueChanged` | `ValueChangedEventArgs` |
| `Editor` | `TextChanged` | `TextChangedEventArgs` |
| `RadioButton` | `CheckedChanged` | `CheckedChangedEventArgs` |

## ChatView + TTS Example

ChatView passes the `ChatMessage` object as `args`, enabling text-to-speech for incoming messages:

```csharp
public class ChatTtsFeedbackService(ITextToSpeech tts) : HapticFeedbackService
{
    public override async void OnRequested(object control, string eventName, object? args)
    {
        base.OnRequested(control, eventName, args);

        if (control is ChatView && eventName == "MessageReceived" && args is ChatMessage { IsFromMe: false } msg)
        {
            var say = $"Message from {msg.SenderId}. {msg.Text}";
            await tts.SpeakAsync(say);
        }
    }
}
```

## Disabling Feedback

Set `UseFeedback="False"` on any control:

```xml
<shiny:FloatingPanel UseFeedback="False" ... />
```

```csharp
await toaster.ShowAsync("Silent", cfg => cfg.UseFeedback = false);
```

## Code Generation Guidance

- Always check `UseFeedback` before calling `FeedbackHelper.Execute()`
- Pass `this` (the control instance) as the first argument — never `typeof(...)`
- Pass meaningful event names that match the control's public events (e.g. `nameof(Clicked)`, `nameof(Opened)`)
- Pass contextual `args` when useful for TTS or analytics (e.g. `ChatMessage` object, event args, `"LongPress"`)
- `IFeedbackService` is registered as singleton — implementations must be thread-safe
