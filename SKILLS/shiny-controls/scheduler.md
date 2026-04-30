# Scheduler Views

Three views for calendar/scheduling UI, all sharing a common `ISchedulerEventProvider` interface. Built programmatically (no XAML internals), using AOT-safe lambda bindings, with custom DataTemplate support.

## Core Interface: ISchedulerEventProvider

```csharp
public interface ISchedulerEventProvider
{
    Task<IReadOnlyList<SchedulerEvent>> GetEvents(DateTimeOffset start, DateTimeOffset end);
    void OnEventSelected(SchedulerEvent selectedEvent);
    bool CanCalendarSelect(DateOnly selectedDate);
    void OnCalendarDateSelected(DateOnly selectedDate);
    void OnAgendaTimeSelected(DateTimeOffset selectedTime);
    bool CanSelectAgendaTime(DateTimeOffset selectedTime);
}
```

## SchedulerEvent Model

```csharp
public class SchedulerEvent
{
    public string Identifier { get; set; }   // Default: new Guid
    public string Title { get; set; }
    public string? Description { get; set; }
    public Color? Color { get; set; }
    public bool IsAllDay { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
```

## Supporting Models

### CalendarListDayGroup
Used for custom `DayHeaderTemplate` bindings in `SchedulerCalendarListView`:

```csharp
public class CalendarListDayGroup : List<SchedulerEvent>
{
    public DateOnly Date { get; }
    public string DateDisplay { get; }       // "dddd, MMMM d, yyyy"
    public bool IsToday { get; }
    public string EventCountDisplay { get; } // "3 events"
}
```

### AgendaDatePickerMode
Controls which date picker is shown on the `SchedulerAgendaView`:

```csharp
public enum AgendaDatePickerMode
{
    None,      // No picker — control date externally
    Carousel,  // Horizontal day picker (Apple Calendar-style, default)
    Calendar   // Collapsible month calendar sheet with pull-to-expand
}
```

### DatePickerItemContext
Used for custom `DayPickerItemTemplate` bindings in `SchedulerAgendaView`:

```csharp
public class DatePickerItemContext
{
    public DateOnly Date { get; set; }
    public string DayNumber { get; set; }   // "30"
    public string DayName { get; set; }     // "MON"
    public string MonthName { get; set; }   // "MAR"
    public bool IsSelected { get; set; }
    public bool IsToday { get; set; }
}
```

### CalendarOverflowContext
Used for custom `OverflowItemTemplate` bindings in `SchedulerCalendarView`:

```csharp
public class CalendarOverflowContext
{
    public int EventCount { get; set; }
    public DateOnly Date { get; set; }
}
```

## SchedulerCalendarView

Monthly calendar grid with swipe navigation, event display per cell, and pinch-to-zoom.

```xml
<shiny:SchedulerCalendarView
    Provider="{Binding Provider}"
    SelectedDate="{Binding SelectedDate}"
    DisplayMonth="{Binding DisplayMonth}"
    MaxEventsPerCell="3"
    CurrentDayColor="DodgerBlue" />
```

### SchedulerCalendarView Properties

| Property | Type | Default |
|----------|------|---------|
| `Provider` | `ISchedulerEventProvider?` | `null` |
| `SelectedDate` | `DateOnly` | Today (TwoWay) |
| `DisplayMonth` | `DateOnly` | Today (TwoWay) |
| `MinDate` | `DateOnly?` | `null` |
| `MaxDate` | `DateOnly?` | `null` |
| `ShowCalendarCellEventCountOnly` | `bool` | `false` |
| `EventItemTemplate` | `DataTemplate?` | `null` |
| `OverflowItemTemplate` | `DataTemplate?` | `null` |
| `LoaderTemplate` | `DataTemplate?` | `null` |
| `MaxEventsPerCell` | `int` | `3` |
| `CalendarCellColor` | `Color` | `White` |
| `CalendarCellSelectedColor` | `Color` | `LightBlue` |
| `CurrentDayColor` | `Color` | `DodgerBlue` |
| `FirstDayOfWeek` | `DayOfWeek` | `Sunday` |
| `AllowPan` | `bool` | `true` |
| `AllowZoom` | `bool` | `false` |
| `UseHapticFeedback` | `bool` | `true` |

## SchedulerAgendaView

Day/multi-day timeline with overlap detection, Apple Calendar-style date picker, current time marker, and timezone support.

```xml
<shiny:SchedulerAgendaView
    Provider="{Binding Provider}"
    SelectedDate="{Binding SelectedDate}"
    DaysToShow="{Binding DaysToShow}"
    Use24HourTime="False"
    ShowCurrentTimeMarker="True"
    ShowCarouselDatePicker="True"
    TimeSlotHeight="60" />
```

### SchedulerAgendaView Properties

| Property | Type | Default |
|----------|------|---------|
| `Provider` | `ISchedulerEventProvider?` | `null` |
| `SelectedDate` | `DateOnly` | Today (TwoWay) |
| `MinDate` | `DateOnly?` | `null` |
| `MaxDate` | `DateOnly?` | `null` |
| `DaysToShow` | `int` | `1` (clamped 1-7) |
| `ShowCarouselDatePicker` | `bool` | `true` |
| `DatePickerMode` | `AgendaDatePickerMode` | `Carousel` |
| `ShowCurrentTimeMarker` | `bool` | `true` |
| `Use24HourTime` | `bool` | `true` |
| `EventItemTemplate` | `DataTemplate?` | `null` |
| `LoaderTemplate` | `DataTemplate?` | `null` |
| `DayPickerItemTemplate` | `DataTemplate?` | `null` |
| `CurrentTimeMarkerColor` | `Color` | `Red` |
| `TimezoneColor` | `Color` | `Gray` |
| `SeparatorColor` | `Color` | Light gray |
| `DefaultEventColor` | `Color` | `CornflowerBlue` |
| `TimeSlotHeight` | `double` | `60.0` |
| `AllowPan` | `bool` | `true` |
| `AllowZoom` | `bool` | `false` |
| `ShowAdditionalTimezones` | `bool` | `false` |
| `AdditionalTimezones` | `IList<TimeZoneInfo>` | empty |
| `UseHapticFeedback` | `bool` | `true` |

## SchedulerCalendarListView

Vertically scrolling event list grouped by day with infinite scroll forward/backward.

```xml
<shiny:SchedulerCalendarListView
    Provider="{Binding Provider}"
    SelectedDate="{Binding SelectedDate}"
    DaysPerPage="30"
    DefaultEventColor="CornflowerBlue" />
```

### SchedulerCalendarListView Properties

| Property | Type | Default |
|----------|------|---------|
| `Provider` | `ISchedulerEventProvider?` | `null` |
| `SelectedDate` | `DateOnly` | Today (TwoWay) |
| `MinDate` | `DateOnly?` | `null` |
| `MaxDate` | `DateOnly?` | `null` |
| `EventItemTemplate` | `DataTemplate?` | `null` |
| `DayHeaderTemplate` | `DataTemplate?` | `null` |
| `LoaderTemplate` | `DataTemplate?` | `null` |
| `DaysPerPage` | `int` | `30` |
| `DefaultEventColor` | `Color` | `CornflowerBlue` |
| `DayHeaderBackgroundColor` | `Color` | `Transparent` |
| `DayHeaderTextColor` | `Color` | `Black` |
| `AllowPan` | `bool` | `true` |
| `AllowZoom` | `bool` | `false` |

## Default Templates

The `DefaultTemplates` static class provides reusable AOT-safe templates:

| Method | Binds To |
|--------|----------|
| `CreateEventItemTemplate()` | `SchedulerEvent` — Color bar + title |
| `CreateOverflowTemplate()` | `CalendarOverflowContext` — "+N more" |
| `CreateLoaderTemplate()` | None — ActivityIndicator + "Loading..." |
| `CreateCalendarListDayHeaderTemplate()` | `CalendarListDayGroup` — Accent bar + today dot + bold date + event count |
| `CreateCalendarListEventItemTemplate()` | `SchedulerEvent` — Card with color bar, title, description, time range |
| `CreateAppleCalendarDayPickerTemplate()` | `DatePickerItemContext` — Apple Calendar-style day picker |

## Event Provider Implementation

```csharp
public class MyEventProvider : ISchedulerEventProvider
{
    public async Task<IReadOnlyList<SchedulerEvent>> GetEvents(DateTimeOffset start, DateTimeOffset end)
    {
        // Fetch events from your data source for the given range
        // Multi-day events should be included if they overlap the range at all
        return events;
    }

    public void OnEventSelected(SchedulerEvent selectedEvent)
    {
        // Handle event taps — navigate to detail, show dialog, etc.
    }

    public bool CanCalendarSelect(DateOnly selectedDate) => true;
    public void OnCalendarDateSelected(DateOnly selectedDate) { }
    public void OnAgendaTimeSelected(DateTimeOffset selectedTime) { }
    public bool CanSelectAgendaTime(DateTimeOffset selectedTime) => true;
}
```

## Scheduler ViewModel Pattern

```csharp
public class MySchedulerViewModel : INotifyPropertyChanged
{
    DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);

    public MySchedulerViewModel(ISchedulerEventProvider provider)
    {
        Provider = provider;
    }

    public ISchedulerEventProvider Provider { get; }

    public DateOnly SelectedDate
    {
        get => _selectedDate;
        set { _selectedDate = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

Register in DI:
```csharp
builder.Services.AddSingleton<ISchedulerEventProvider, MyEventProvider>();
```

## Scheduler Page Pattern

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:shiny="http://shiny.net/maui/controls"
             xmlns:vm="clr-namespace:MyApp.ViewModels"
             x:Class="MyApp.Pages.MyCalendarPage"
             x:DataType="vm:MySchedulerViewModel"
             Title="Calendar">

    <shiny:SchedulerCalendarListView
        Provider="{Binding Provider}"
        SelectedDate="{Binding SelectedDate}" />
</ContentPage>
```

Code-behind with constructor injection:
```csharp
public partial class MyCalendarPage : ContentPage
{
    public MyCalendarPage(MySchedulerViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}
```

## Agenda with Calendar Picker Example

```xml
<shiny:SchedulerAgendaView
    Provider="{Binding Provider}"
    SelectedDate="{Binding SelectedDate}"
    DaysToShow="{Binding DaysToShow}"
    DatePickerMode="Calendar"
    ShowCarouselDatePicker="True"
    ShowCurrentTimeMarker="True"
    CurrentTimeMarkerColor="Red"
    DefaultEventColor="CornflowerBlue"
    TimeSlotHeight="60"
    Use24HourTime="False" />
```

The calendar picker starts collapsed showing only the week of the selected date. Pull the handle down or tap it to expand to a full month view with navigation arrows. Tapping a date selects it and updates the agenda. The picker auto-navigates months when a date outside the current display month is selected.

## Custom Template Example (AOT-Safe)

```csharp
var myEventTemplate = new DataTemplate(() =>
{
    var grid = new Grid
    {
        ColumnDefinitions =
        {
            new ColumnDefinition(new GridLength(4)),
            new ColumnDefinition(GridLength.Star)
        },
        Padding = new Thickness(8),
        ColumnSpacing = 8
    };

    var colorBar = new BoxView { CornerRadius = 2 };
    colorBar.SetBinding(BoxView.ColorProperty, static (SchedulerEvent e) => e.Color);

    var title = new Label { FontSize = 14 };
    title.SetBinding(Label.TextProperty, static (SchedulerEvent e) => e.Title);

    grid.Add(colorBar, 0);
    grid.Add(title, 1);
    return grid;
});
```

## Scheduler Important Notes

- All three views extend `ContentView` — place anywhere in your XAML layout
- `SelectedDate` is `TwoWay` on all views — changes propagate back to the ViewModel
- All views support `MinDate`/`MaxDate` to constrain navigation and selection
- All views support `AllowPan` and `AllowZoom` to control gesture interactions
- All views support `LoaderTemplate` for custom loading indicators
- The provider's `GetEvents()` may be called with any date range — implement it to handle arbitrary ranges
- Multi-day events should span the full range (the views handle per-day duplication internally)
- `SchedulerCalendarListView` skips empty days (only days with events are shown)
- `SchedulerAgendaView` supports three date picker modes via `DatePickerMode`: `Carousel` (Apple Calendar-style day picker, default), `Calendar` (collapsible month calendar sheet with pull-to-expand), or `None` (hides both pickers for external date control)
- When using `DatePickerMode="Carousel"`, override with `DayPickerItemTemplate` using `DatePickerItemContext`
- `SchedulerAgendaView` supports 24-hour and 12-hour (AM/PM) time via `Use24HourTime`
- `SchedulerAgendaView` supports multiple timezone columns via `AdditionalTimezones` + `ShowAdditionalTimezones`
- All-day events always sort to top within their day group
- Custom templates must use AOT-safe static lambda bindings, never string-based bindings
- `AdditionalTimezones` is an `IList<TimeZoneInfo>` — add timezones in code-behind
