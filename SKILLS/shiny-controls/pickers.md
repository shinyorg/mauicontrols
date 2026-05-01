# Pickers (Date, Time, Duration)

Standalone picker controls that open a FloatingPanel for selection. These provide a consistent, cross-platform experience without relying on native platform pickers.

**Requirement:** Pages must use `ShinyContentPage` (or have an `OverlayHost` in the visual tree) for the floating panel to render.

## ShinyDatePicker

Opens a calendar view in a floating panel. Selecting a date automatically closes the panel.

```xml
<shiny:ShinyDatePicker Date="{Binding SelectedDate, Mode=TwoWay}"
                       MinDate="{Binding MinDate}"
                       MaxDate="{Binding MaxDate}"
                       Format="D"
                       Placeholder="Choose a date"
                       FirstDayOfWeek="Monday" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Date` | `DateOnly?` | `null` | Selected date (TwoWay bindable) |
| `MinDate` | `DateOnly?` | `null` | Minimum selectable date |
| `MaxDate` | `DateOnly?` | `null` | Maximum selectable date |
| `Format` | `string` | `"d"` | Display format string |
| `Placeholder` | `string` | `"Select date"` | Text shown when no date selected |
| `PlaceholderColor` | `Color` | `Gray` | Placeholder text color |
| `TextColor` | `Color?` | `null` | Selected date text color |
| `FontSize` | `double` | `16` | Display font size |
| `FirstDayOfWeek` | `DayOfWeek` | `Sunday` | First day of week in calendar |

**Event:** `DateSelected` — fires with the selected `DateOnly` value.

## ShinyTimePicker

Opens hour/minute pickers in a floating panel with Done/Cancel buttons.

```xml
<shiny:ShinyTimePicker Time="{Binding SelectedTime, Mode=TwoWay}"
                       Use24Hour="True"
                       MinuteInterval="15"
                       Format="HH:mm"
                       Placeholder="Choose a time" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Time` | `TimeSpan?` | `null` | Selected time (TwoWay bindable) |
| `Format` | `string` | `"t"` | Display format string |
| `Use24Hour` | `bool` | `false` | Use 24-hour format instead of AM/PM |
| `MinuteInterval` | `int` | `1` | Minute increment step (e.g. 5, 10, 15) |
| `Placeholder` | `string` | `"Select time"` | Text shown when no time selected |
| `PlaceholderColor` | `Color` | `Gray` | Placeholder text color |
| `TextColor` | `Color?` | `null` | Selected time text color |
| `FontSize` | `double` | `16` | Display font size |

**Event:** `TimeSelected` — fires with the selected `TimeSpan` value.

## ShinyDurationPicker

Opens hour/minute pickers with "hr"/"min" labels in a floating panel.

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

## TableView Cell Integration

The TableView cells `DatePickerCell`, `TimePickerCell`, and `DurationPickerCell` now use these same FloatingPanel-based pickers internally instead of native platform pickers. This provides a consistent look across platforms.

**DatePickerCell** additional properties: `MinuteInterval` not applicable; uses calendar view.

**TimePickerCell** additional properties:
| Property | Type | Default |
|---|---|---|
| `MinuteInterval` | `int` | `1` |
| `Use24Hour` | `bool` | `false` |

**DurationPickerCell** additional properties:
| Property | Type | Default |
|---|---|---|
| `MinuteInterval` | `int` | `5` |

**Note:** When using these cells in a TableView, the page must use `ShinyContentPage` so the floating panel has an overlay host to render into.
