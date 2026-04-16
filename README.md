* Prompt
Update the shiny documentation at ~/Desktop/shinyorg/shiny - we're going to create a new section called "MAUI Controls" and rename "MAUI" to "MAUI Services".
Controls will have one set of release notes since it 1 solidified package.  Update the homepage and add a new card for the controls.  Update the skills, however, the skills
will still go under "MAUI" as will the services.  Also update the App/Lib builder changing the packages used by scheduler, tableview, & mermaid diagrams while also adding
bottom sheet & markdown controls

Update the MAUI template to reflect the new package structure and control usage.  You will need to remove scheduler, tableview, and mermaid diagram references from the main MAUI package and add them to the new controls package.  Also add references for the new bottom sheet and markdown controls.  Update the XAML namespaces and usage examples in the template to reflect the new control locations.  Finally, update the documentation to reflect the new structure and provide clear guidance on how to use the new controls in a MAUI application.

# Shiny.Maui.Controls

A .NET MAUI controls library providing rich, ready-to-use UI components for mobile and desktop applications.

[![NuGet](https://img.shields.io/nuget/v/Shiny.Maui.Controls.svg)](https://www.nuget.org/packages/Shiny.Maui.Controls)

## Getting Started

```bash
dotnet add package Shiny.Maui.Controls
```

Register in your `MauiProgram.cs`:

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseShinyControls();
```

Add the XAML namespace:

```xml
xmlns:shiny="http://shiny.net/maui/controls"
```

For Markdown controls (separate package):

```bash
dotnet add package Shiny.Maui.Controls.Markdown
```

```xml
xmlns:md="http://shiny.net/maui/markdown"
```

## Controls

### Scheduler

Calendar and agenda views for displaying events and appointments, powered by `ISchedulerEventProvider`.

| Calendar | Agenda | Event List |
|:---:|:---:|:---:|
| ![Calendar](assets/scheduler1.png) | ![Agenda](assets/scheduler2.png) | ![Event List](assets/scheduler3.png) |

**SchedulerCalendarView** - Month calendar grid with event indicators, swipe navigation, and date selection.

```xml
<shiny:SchedulerCalendarView
    Provider="{Binding Provider}"
    SelectedDate="{Binding SelectedDate}"
    DisplayMonth="{Binding DisplayMonth}" />
```

**SchedulerAgendaView** - Day/multi-day timeline with time slots, overlapping event layout, current time marker, and optional timezone columns.

```xml
<shiny:SchedulerAgendaView
    Provider="{Binding Provider}"
    SelectedDate="{Binding SelectedDate}"
    DaysToShow="{Binding DaysToShow}"
    ShowAdditionalTimezones="{Binding ShowAdditionalTimezones}" />
```

**SchedulerCalendarListView** - Scrollable event list grouped by day with infinite scroll loading.

```xml
<shiny:SchedulerCalendarListView
    Provider="{Binding Provider}"
    SelectedDate="{Binding SelectedDate}" />
```

**ISchedulerEventProvider** - Implement this interface to supply event data:

```csharp
public class MyEventProvider : ISchedulerEventProvider
{
    public Task<IReadOnlyList<SchedulerEvent>> GetEvents(DateTimeOffset start, DateTimeOffset end) { ... }
    public void OnEventSelected(SchedulerEvent selectedEvent) { ... }
    public bool CanCalendarSelect(DateOnly selectedDate) => true;
    public void OnCalendarDateSelected(DateOnly selectedDate) { }
    public bool CanSelectAgendaTime(DateTimeOffset selectedTime) => true;
    public void OnAgendaTimeSelected(DateTimeOffset selectedTime) { }
}
```

### BottomSheetView

A bottom sheet overlay that slides up from the bottom of the screen with configurable snap positions (detents).

<!-- TODO: Add screenshots -->

```xml
<shiny:BottomSheetView
    IsOpen="{Binding IsSheetOpen}"
    HasBackdrop="True"
    CloseOnBackdropTap="True"
    SheetCornerRadius="16">
    <shiny:BottomSheetView.Detents>
        <shiny:DetentValue Value="Quarter" />
        <shiny:DetentValue Value="Half" />
        <shiny:DetentValue Value="Full" />
    </shiny:BottomSheetView.Detents>
    <shiny:BottomSheetView.SheetContent>
        <!-- Your content here -->
    </shiny:BottomSheetView.SheetContent>
</shiny:BottomSheetView>
```

| Property | Type | Description |
|---|---|---|
| IsOpen | bool | Show/hide the sheet (TwoWay) |
| Detents | ObservableCollection\<DetentValue\> | Snap positions (Quarter, Half, Full) |
| SheetContent | View | Content displayed in the sheet |
| HasBackdrop | bool | Fade backdrop behind sheet |
| CloseOnBackdropTap | bool | Close when backdrop tapped |
| SheetCornerRadius | double | Top corner radius |
| HandleColor | Color | Drag handle color |
| SheetBackgroundColor | Color | Sheet background color |
| AnimationDuration | double | Animation speed (ms) |
| ExpandOnInputFocus | bool | Auto-expand when input focused |

### ImageViewer

A full-screen image overlay with pinch-to-zoom, pan, double-tap zoom, and animated open/close transitions.

<!-- TODO: Add screenshots -->

```xml
<Grid>
    <!-- Page content with tappable images -->
    <ScrollView>
        <VerticalStackLayout>
            <Image Source="photo.png">
                <Image.GestureRecognizers>
                    <TapGestureRecognizer Command="{Binding OpenViewerCommand}"
                                          CommandParameter="photo.png" />
                </Image.GestureRecognizers>
            </Image>
        </VerticalStackLayout>
    </ScrollView>

    <!-- ImageViewer overlays on top -->
    <shiny:ImageViewer Source="{Binding SelectedImage}"
                       IsOpen="{Binding IsViewerOpen}" />
</Grid>
```

| Property | Type | Description |
|---|---|---|
| Source | ImageSource? | The image to display |
| IsOpen | bool | Show/hide the viewer (TwoWay) |
| MaxZoom | double | Maximum zoom scale (default: 5.0) |

**Features:**
- Pinch-to-zoom with origin tracking
- Pan when zoomed (clamped to image bounds)
- Double-tap to zoom in (2.5x) / reset
- Animated fade open/close with backdrop
- Close button overlay

### PillView

Pill/chip/tag elements for displaying categories, filters, or status indicators with predefined or custom color schemes.

<!-- TODO: Add screenshots -->

```xml
<shiny:PillView Text="Success" Type="Success" />
<shiny:PillView Text="Warning" Type="Warning" />
<shiny:PillView Text="Custom" PillColor="Purple" PillTextColor="White" />
```

| Pill Type | Description |
|---|---|
| None | Default/neutral |
| Success | Green |
| Info | Blue |
| Warning | Yellow |
| Caution | Orange |
| Critical | Red |

### TableView

A settings-style table view with 14+ built-in cell types, section grouping, drag-to-reorder, and dynamic data binding.

<!-- TODO: Add screenshots -->

```xml
<shiny:TableView>
    <shiny:TableRoot>
        <shiny:TableSection Title="General">
            <shiny:SwitchCell Title="Wi-Fi" On="{Binding WifiEnabled}" />
            <shiny:EntryCell Title="Username" Text="{Binding Username}" />
            <shiny:PickerCell Title="Theme" ItemsSource="{Binding Themes}" SelectedItem="{Binding SelectedTheme}" />
        </shiny:TableSection>
    </shiny:TableRoot>
</shiny:TableView>
```

**Cell Types:**

| Cell | Description |
|---|---|
| SwitchCell | Toggle switch |
| EntryCell | Text input field |
| CheckboxCell | Checkbox with accent color |
| RadioCell | Radio button with section-level grouping |
| CommandCell | Tappable row with optional arrow indicator |
| ButtonCell | Command-bound button |
| LabelCell | Read-only text display |
| PickerCell | Single or multi-select picker |
| TextPickerCell | String list picker |
| DatePickerCell | Date selection with min/max bounds |
| TimePickerCell | Time selection |
| DurationPickerCell | TimeSpan picker with min/max |
| NumberPickerCell | Integer picker with min/max/unit |
| SimpleCheckCell | Checkmark indicator |
| CustomCell | Custom view content with drag-reorder support |

**Dynamic Sections** - Bind to a collection to generate sections from data:

```xml
<shiny:TableView ItemsSource="{Binding Items}" ItemTemplate="{StaticResource SectionTemplate}" />
```

### Markdown Controls

> Separate NuGet package: `Shiny.Maui.Controls.Markdown`

Render and edit markdown content using native MAUI controls — no WebView required. Auto-resolves Light/Dark theming.

<!-- TODO: Add screenshots -->

**MarkdownView** — Read-only markdown renderer:

```xml
<md:MarkdownView Markdown="{Binding DocumentContent}" Padding="16" />
```

| Property | Type | Description |
|---|---|---|
| Markdown | string | Markdown content to render |
| Theme | MarkdownTheme? | Rendering theme (auto Light/Dark if null) |
| IsScrollEnabled | bool | Enable/disable scrolling (default: true) |

Events: `LinkTapped` — fired when a link is tapped; set `Handled = true` to prevent default browser launch.

**MarkdownEditor** — Editor with formatting toolbar and live preview:

```xml
<md:MarkdownEditor Markdown="{Binding NoteContent, Mode=TwoWay}"
                   Placeholder="Start writing..."
                   Padding="8" />
```

| Property | Type | Description |
|---|---|---|
| Markdown | string | Markdown content (TwoWay) |
| Theme | MarkdownTheme? | Preview theme (auto Light/Dark if null) |
| Placeholder | string | Placeholder text |
| ToolbarItems | IReadOnlyList\<MarkdownToolbarItem\>? | Toolbar buttons (default set provided) |
| IsPreviewVisible | bool | Toggle preview pane (TwoWay) |
| ToolbarBackgroundColor | Color? | Toolbar background |
| EditorBackgroundColor | Color? | Editor background |

**Features:**
- Formatting toolbar: bold, italic, headings, lists, code, links, blockquotes
- Live preview toggle
- Auto-growing editor
- Full Markdig support: tables, task lists, strikethrough, fenced code blocks
- Customizable themes with colors, font sizes, and spacing
- Custom toolbar item support

### MermaidDiagramControl

> Separate NuGet package: `Shiny.Maui.Controls.MermaidDiagrams`

Native .NET MAUI Mermaid flowchart renderer — no WebView, no SkiaSharp, AOT compatible. Parses Mermaid syntax and renders interactive diagrams with pan and zoom support.

| Flowchart | Editor | Themes | Subgraphs |
|:---:|:---:|:---:|:---:|
| ![Flowchart](assets/mermaid-flowchart.png) | ![Editor](assets/mermaid-editor.png) | ![Themes](assets/mermaid-themes.png) | ![Subgraphs](assets/mermaid-subgraphs.png) |

```bash
dotnet add package Shiny.Maui.Controls.MermaidDiagrams
```

```xml
xmlns:diagram="http://shiny.net/maui/diagrams"
```

```xml
<diagram:MermaidDiagramControl
    DiagramText="graph TD&#10;    A[Start] --> B{Decision}&#10;    B -->|Yes| C[Do Something]&#10;    B -->|No| D[Do Other]&#10;    C --> E[End]&#10;    D --> E"
    HorizontalOptions="Fill"
    VerticalOptions="Fill" />
```

**Features:**
- Mermaid `graph` / `flowchart` syntax (TD, LR, BT, RL directions)
- 6 node shapes: Rectangle, RoundedRectangle, Stadium, Circle, Diamond, Hexagon
- 6 edge styles: Solid, Open, Dotted, DottedOpen, Thick, ThickOpen
- Subgraph support with nested grouping
- 4 built-in themes: Default, Dark, Forest, Neutral
- Pan and pinch-to-zoom gestures
- Sugiyama layered graph layout algorithm
