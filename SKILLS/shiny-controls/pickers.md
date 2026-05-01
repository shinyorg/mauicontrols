# ShinyDurationPicker

A standalone duration picker control that opens a FloatingPanel for selection with hour/minute pickers and "hr"/"min" labels.

**Requirement:** Pages must use `ShinyContentPage` (or have an `OverlayHost` in the visual tree) for the floating panel to render.

## Usage

```xml
<shiny:ShinyDurationPicker Duration="{Binding SelectedDuration, Mode=TwoWay}"
                           MinDuration="0:15:00"
                           MaxDuration="8:00:00"
                           MinuteInterval="5"
                           Placeholder="Choose duration" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Duration` | `TimeSpan?` | `null` | Selected duration (TwoWay bindable) |
| `MinDuration` | `TimeSpan` | `0:00:00` | Minimum allowed duration |
| `MaxDuration` | `TimeSpan` | `24:00:00` | Maximum allowed duration |
| `MinuteInterval` | `int` | `5` | Minute increment step |
| `Format` | `string` | `@"h\:mm"` | Display format string |
| `Placeholder` | `string` | `"Select duration"` | Text shown when no duration selected |
| `PlaceholderColor` | `Color` | `Gray` | Placeholder text color |
| `TextColor` | `Color?` | `null` | Selected duration text color |
| `FontSize` | `double` | `16` | Display font size |

**Event:** `DurationSelected` — fires with the selected `TimeSpan` value.

## TableView DurationPickerCell

The `DurationPickerCell` uses the same FloatingPanel-based picker internally.

| Property | Type | Default |
|---|---|---|
| `MinuteInterval` | `int` | `5` |

**Note:** When using `DurationPickerCell` in a TableView, the page must use `ShinyContentPage` so the floating panel has an overlay host to render into.
