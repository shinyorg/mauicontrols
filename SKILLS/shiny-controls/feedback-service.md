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
    void OnRequested(Type controlType, string eventName, string? details = null);
}
```

- `controlType`: The control's `Type` (e.g. `typeof(ChatView)`)
- `eventName`: The interaction (e.g. `"MessageSent"`, `"Clicked"`, `"Opened"`)
- `details`: Optional context — message text for ChatView, `"LongPress"` for SecurityPin completion

## Default Implementation

```csharp
public class HapticFeedbackService : IFeedbackService
{
    public void OnRequested(Type controlType, string eventName, string? details)
    {
        if (details?.Equals("LongPress") ?? false)
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
        else
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
    }
}
```

## Control Events

| Control | Event | Details | Description |
|---|---|---|---|
| `ChatView` | `MessageSent` | message text | User sent a message |
| `ChatView` | `MessageReceived` | message text | External message arrived |
| `ChatView` | `MessageTapped` | message text | Message bubble tapped |
| `ChatView` | `AttachImage` | — | Attach button tapped |
| `Fab` | `Clicked` | — | Fab tapped |
| `FabMenu` | `Toggled` | — | Menu opened/closed |
| `FabMenuItem` | `Clicked` | — | Menu item tapped |
| `FloatingPanel` | `Opened` | — | Panel opened |
| `FloatingPanel` | `Closed` | — | Panel closed |
| `FloatingPanel` | `DetentChanged` | — | Panel snapped to detent |
| `ImageViewer` | `DoubleTapped` | — | Double-tap zoom |
| `SecurityPin` | `Completed` | `"LongPress"` | All digits entered |
| `SecurityPin` | `DigitEntered` | — | Digit entered |
| `SchedulerCalendarView` | `DaySelected` | — | Day tapped |
| `SchedulerCalendarView` | `EventSelected` | — | Event tapped |
| `SchedulerAgendaView` | `EventSelected` | — | Event tapped |
| `SchedulerAgendaView` | `TimeSlotSelected` | — | Time slot tapped |
| `TextToSpeechButton` | `Clicked` | — | Button tapped |
| `CellBase` | `Tapped` | — | Table cell tapped |
| `Toaster` | `Show` | toast text | Toast shown |

## Standard MAUI Control Integration

Opt-in integration that hooks feedback into standard MAUI controls (Button, Entry, Slider, etc.) without modifying those controls. Uses `Application.DescendantAdded`/`DescendantRemoved` to automatically bind/unbind events across the visual tree.

```csharp
builder.UseShinyControls(cfg =>
{
    cfg.AddDefaultMauiControlFeedback();
});
```

### Integrated MAUI Controls

| Control | Event | Details |
|---|---|---|
| `Button` | `Clicked` | — |
| `Entry` | `TextChanged` | new text value |
| `Slider` | `ValueChanged` | new value (F2) |
| `Switch` | `Toggled` | value |
| `CheckBox` | `CheckedChanged` | value |
| `DatePicker` | `DateSelected` | new date |
| `TimePicker` | `TimeChanged` | — |
| `Picker` | `SelectedIndexChanged` | — |
| `SearchBar` | `SearchButtonPressed` | — |
| `Stepper` | `ValueChanged` | new value (F2) |
| `Editor` | `TextChanged` | new text value |
| `RadioButton` | `CheckedChanged` | value |

## ChatView + TTS Example

ChatView passes message text as `details`, enabling text-to-speech for incoming messages:

```csharp
public class ChatTtsFeedbackService : IFeedbackService
{
    readonly ITextToSpeech tts;
    public ChatTtsFeedbackService(ITextToSpeech tts) => this.tts = tts;

    public async void OnRequested(Type controlType, string eventName, string? details)
    {
        if (controlType == typeof(ChatView) && eventName == "MessageReceived" && details is not null)
        {
            await tts.SpeakAsync(details);
            return;
        }

        try { HapticFeedback.Default.Perform(HapticFeedbackType.Click); }
        catch { }
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
- Pass meaningful event names that match the control's public events (e.g. `nameof(Clicked)`, `nameof(Opened)`)
- Pass contextual `details` when useful for TTS or analytics (message text, "LongPress")
- `IFeedbackService` is registered as singleton — implementations must be thread-safe
