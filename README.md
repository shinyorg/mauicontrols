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

### Fab & FabMenu

A Material Design-style floating action button, plus an expanding multi-action menu that animates up from the main FAB.

<!-- TODO: Add screenshots -->

```xml
<!-- Single Fab -->
<shiny:Fab Icon="add.png"
           Text="Add Item"
           FabBackgroundColor="#4CAF50"
           TextColor="White"
           Command="{Binding AddCommand}"
           HorizontalOptions="End"
           VerticalOptions="End"
           Margin="24" />

<!-- FabMenu with child items -->
<shiny:FabMenu IsOpen="{Binding IsMenuOpen}"
               Icon="plus.png"
               FabBackgroundColor="#2196F3"
               HorizontalOptions="End"
               VerticalOptions="End"
               Margin="24">
    <shiny:FabMenuItem Icon="share.png"  Text="Share"  Command="{Binding ShareCommand}" />
    <shiny:FabMenuItem Icon="edit.png"   Text="Edit"   Command="{Binding EditCommand}" />
    <shiny:FabMenuItem Icon="delete.png" Text="Delete" Command="{Binding DeleteCommand}" />
</shiny:FabMenu>
```

**Fab** properties:

| Property | Type | Default | Description |
|---|---|---|---|
| Icon | ImageSource? | null | Button icon |
| Text | string? | null | Optional label; when null the Fab is a perfect circle |
| Command | ICommand? | null | Invoked when the Fab is tapped |
| CommandParameter | object? | null | Parameter passed to the Command |
| FabBackgroundColor | Color | #2196F3 | Fill color |
| BorderColor | Color? | null | Outline stroke color |
| BorderThickness | double | 0 | Outline stroke thickness |
| TextColor | Color | White | Label color |
| FontSize | double | 14 | Label font size |
| FontAttributes | FontAttributes | None | Label font attributes |
| Size | double | 56 | Height of the Fab (diameter when circular) |
| IconSize | double | 24 | Icon image size |
| HasShadow | bool | true | Show drop shadow |

Events: `Clicked`.

**FabMenu** properties (plus all main-Fab pass-throughs above):

| Property | Type | Default | Description |
|---|---|---|---|
| IsOpen | bool | false | Two-way bindable; opens/closes the menu with animation |
| Items | `IList<FabMenuItem>` | empty | Menu items (content property — place items directly inside the FabMenu) |
| HasBackdrop | bool | true | Show a dim backdrop while open |
| BackdropColor | Color | Black | Backdrop color |
| BackdropOpacity | double | 0.4 | Backdrop peak opacity |
| CloseOnBackdropTap | bool | true | Close when backdrop is tapped |
| CloseOnItemTap | bool | true | Close after any item is tapped |
| AnimationDuration | uint | 200 | Open/close animation duration (ms) |

Events: `ItemTapped` — fires the `FabMenuItem` that was tapped.

Methods: `Open()`, `Close()`, `Toggle()`.

**FabMenuItem** properties:

| Property | Type | Default | Description |
|---|---|---|---|
| Icon | ImageSource? | null | Circular icon |
| Text | string? | null | Side label next to the icon |
| Command | ICommand? | null | Invoked when tapped |
| CommandParameter | object? | null | Parameter for the Command |
| FabBackgroundColor | Color | #2196F3 | Icon button fill |
| BorderColor | Color? | null | Icon button outline |
| BorderThickness | double | 0 | Icon button outline thickness |
| TextColor | Color | Black | Side-label text color |
| LabelBackgroundColor | Color | White | Side-label background |
| FontSize | double | 13 | Side-label font size |
| Size | double | 44 | Icon button diameter |
| IconSize | double | 20 | Icon image size |

**Placement tip**: `FabMenu` should live in a `Grid` that fills the page (the same placement pattern as `BottomSheetView` / `ImageViewer`) so the backdrop can cover the page content.

### SecurityPin

A PIN entry control with individually rendered cells that captures input through a hidden Entry. Digits remain visible by default and can optionally be masked with any character.

<!-- TODO: Add screenshots -->

```xml
<shiny:SecurityPin Length="4"
                   HideCharacter="*"
                   Value="{Binding Pin}"
                   Keyboard="Numeric"
                   Completed="OnPinCompleted" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| Length | int | 4 | Number of PIN cells |
| Value | string | "" | Current PIN value (TwoWay) |
| Keyboard | Keyboard | Numeric | Keyboard type for input |
| HideCharacter | string? | null | When set, masks entered characters; when null/empty, shows actual values |
| CellSize | double | 50 | Width/height of each cell |
| CellSpacing | double | 8 | Space between cells |
| CellCornerRadius | double | 8 | Border corner radius |
| CellBorderColor | Color? | null | Cell border color |
| CellFocusedBorderColor | Color? | null | Border color for the active cell |
| CellBackgroundColor | Color? | null | Cell fill color |
| CellTextColor | Color? | null | Entered character color |
| FontSize | double | 24 | Character font size |

Events: `Completed` fires with a `SecurityPinCompletedEventArgs` once the entered value reaches `Length`.

Methods: `Focus()`, `Unfocus()`, `Clear()`.

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
