# Shiny Controls

A rich, ready-to-use UI controls library for both **.NET MAUI** and **Blazor**. One package per host covers TableView, Scheduler, SheetView, Fab/FabMenu, PillView, SecurityPin, ImageViewer, ImageEditor, and ChatView. Markdown and Mermaid Diagrams ship as separate add-on packages per host.

[![MAUI NuGet](https://img.shields.io/nuget/v/Shiny.Maui.Controls.svg?label=Shiny.Maui.Controls)](https://www.nuget.org/packages/Shiny.Maui.Controls)
[![Blazor NuGet](https://img.shields.io/nuget/v/Shiny.Blazor.Controls.svg?label=Shiny.Blazor.Controls)](https://www.nuget.org/packages/Shiny.Blazor.Controls)

## Getting Started

### .NET MAUI

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

For Mermaid Diagrams (separate package):

```bash
dotnet add package Shiny.Maui.Controls.MermaidDiagrams
```

```xml
xmlns:diagram="http://shiny.net/maui/diagrams"
```

### Blazor

```bash
dotnet add package Shiny.Blazor.Controls
dotnet add package Shiny.Blazor.Controls.Markdown       # optional
dotnet add package Shiny.Blazor.Controls.MermaidDiagrams # optional
```

Add the `@using` directives — typically in `_Imports.razor`:

```razor
@using Shiny.Blazor.Controls
@using Shiny.Blazor.Controls.Cells
@using Shiny.Blazor.Controls.Sections
@using Shiny.Blazor.Controls.Scheduler
@using Shiny.Blazor.Controls.Markdown
@using Shiny.Blazor.Controls.MermaidDiagrams
```

No DI registration is required — drop the components into any `.razor` page.

#### MAUI → Blazor quick reference

| MAUI (XAML) | Blazor (Razor) |
|---|---|
| `<shiny:TableView>` with `<shiny:TableRoot>` | `<TableView>` (no `TableRoot` wrapper) |
| `<shiny:PillView>` | `<Pill>` |
| `<shiny:SheetView>` with `SheetContent` property | `<SheetView>` with `<SheetContent>` child |
| `Value="{Binding Pin}"` (TwoWay) | `@bind-Value="pin"` |
| `IsOpen="{Binding IsOpen, Mode=TwoWay}"` | `@bind-IsOpen="isOpen"` |
| `Command="{Binding DoCommand}"` | `OnClick="DoAsync"` / `Clicked="DoAsync"` |
| `Color` type (e.g. `Colors.Blue`) | CSS color string (e.g. `"#2196F3"`) |
| `Fab.Icon="add.png"` (ImageSource) | `<Fab Icon="+">` (inline text/SVG string) |
| `ItemTemplate` as `DataTemplate` | `ItemTemplate` as `RenderFragment<object>` |

`ISchedulerEventProvider` is identical across both hosts.

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

**SchedulerAgendaView** - Day/multi-day timeline with time slots, overlapping event layout, current time marker, optional timezone columns, and switchable date picker modes (carousel, calendar sheet, or none).

```xml
<shiny:SchedulerAgendaView
    Provider="{Binding Provider}"
    SelectedDate="{Binding SelectedDate}"
    DaysToShow="{Binding DaysToShow}"
    DatePickerMode="Calendar"
    ShowAdditionalTimezones="{Binding ShowAdditionalTimezones}" />
```

**DatePickerMode** options: `Carousel` (default horizontal day picker), `Calendar` (collapsible month calendar with pull-to-expand), `None` (no picker).

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

### SheetView

A sheet overlay that slides in from the bottom or top of the screen with configurable snap positions (detents), optional header peek when minimized, and haptic feedback.

| Closed | Open | Header (Minimized) | Header (Open) | Top (Minimized) | Top (Open) |
|:---:|:---:|:---:|:---:|:---:|:---:|
| ![Closed](assets/sheet1.png) | ![Open](assets/sheet2.png) | ![Header Minimized](assets/sheet3.png) | ![Header Open](assets/sheet4.png) | ![Top Minimized](assets/sheet5.png) | ![Top Open](assets/sheet6.png) |

```xml
<shiny:SheetView
    IsOpen="{Binding IsSheetOpen}"
    Location="Bottom"
    HasBackdrop="True"
    CloseOnBackdropTap="True"
    SheetCornerRadius="16">
    <shiny:SheetView.Detents>
        <shiny:DetentValue Value="Quarter" />
        <shiny:DetentValue Value="Half" />
        <shiny:DetentValue Value="Full" />
    </shiny:SheetView.Detents>
    <!-- Your content here -->
</shiny:SheetView>
```

| Property | Type | Description |
|---|---|---|
| IsOpen | bool | Show/hide the sheet (TwoWay) |
| Location | SheetLocation | `Bottom`, `BottomTabs`, or `Top` — which edge the sheet slides from. Use `BottomTabs` when inside a Shell TabBar to clip the sheet above the tab bar |
| Detents | ObservableCollection\<DetentValue\> | Snap positions (Quarter, Half, Full) |
| SheetContent | View | Content displayed in the sheet |
| HeaderTemplate | View | Optional header view shown inside the sheet (and as a peek bar when minimized) |
| ShowHeaderWhenMinimized | bool | When true, the header peeks from the edge when the sheet is closed |
| HasBackdrop | bool | Fade backdrop behind sheet |
| CloseOnBackdropTap | bool | Close when backdrop tapped |
| SheetCornerRadius | double | Top corner radius |
| HandleColor | Color | Drag handle color |
| SheetBackgroundColor | Color | Sheet background color |
| AnimationDuration | double | Animation speed (ms) |
| ExpandOnInputFocus | bool | Auto-expand when input focused |
| IsLocked | bool | Prevents swipe/backdrop dismiss; code-only control |
| FitContent | bool | Auto-computes detent from content size |
| UseHapticFeedback | bool | Enable/disable haptic feedback on open, close, and detent snap (default: true) |

### ImageViewer

A full-screen image overlay with pinch-to-zoom, pan, double-tap zoom, and animated open/close transitions.

| Gallery | Viewer |
|:---:|:---:|
| ![Gallery](assets/imageviewer1.png) | ![Viewer](assets/imageviewer2.png) |

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
| Aspect | Aspect | Image aspect ratio mode (default: AspectFit) |
| MaxZoom | double | Maximum zoom scale (default: 5.0) |
| CloseButtonTemplate | DataTemplate? | Custom close button (tapping closes viewer) |
| HeaderTemplate | DataTemplate? | Custom header overlay |
| FooterTemplate | DataTemplate? | Custom footer overlay |
| UseHapticFeedback | bool | Enable/disable haptic feedback on double-tap zoom (default: true) |

**Features:**
- Pinch-to-zoom with origin tracking
- Pan when zoomed (clamped to image bounds)
- Double-tap to zoom in (2.5x) / reset
- Animated fade open/close with backdrop
- Close button overlay

### ImageEditor

An inline image editor with cropping, rotation, freehand drawing, text annotations, and zoom. Includes a built-in undo/redo stack, reset-to-original, and export to PNG/JPEG/WEBP at configurable resolutions. Every feature can be toggled on/off, and the default toolbar can be replaced with a custom template.

```xml
<shiny:ImageEditor Source="{Binding ImageData}"
                   CurrentToolMode="{Binding ToolMode}"
                   AllowCrop="True"
                   AllowRotate="True"
                   AllowDraw="True"
                   AllowTextAnnotation="True"
                   DrawStrokeColor="Red"
                   DrawStrokeWidth="3" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| Source | byte[]? | null | Image data to edit |
| CurrentToolMode | ImageEditorToolMode | None | Active tool (None, Crop, Draw, Text) — TwoWay |
| AllowCrop | bool | true | Enable/disable crop tool |
| AllowRotate | bool | true | Enable/disable rotate action |
| AllowDraw | bool | true | Enable/disable freehand drawing |
| AllowTextAnnotation | bool | true | Enable/disable text annotation |
| AllowZoom | bool | true | Enable/disable pinch-to-zoom |
| CanUndo | bool | false | Whether undo is available (OneWayToSource) |
| CanRedo | bool | false | Whether redo is available (OneWayToSource) |
| DrawStrokeColor | Color | Red | Drawing stroke color |
| DrawStrokeWidth | double | 3 | Drawing stroke width |
| TextFontSize | double | 16 | Text annotation font size |
| AnnotationTextColor | Color | White | Text annotation color |
| ToolbarTemplate | DataTemplate? | null | Custom toolbar (replaces default) |
| ToolbarPosition | ToolbarPosition | Bottom | Toolbar placement (Top or Bottom) |
| UseHapticFeedback | bool | true | Haptic feedback on actions |

**Features:**
- Crop with drag handles and dimmed overlay outside the selection
- 90° rotation (or arbitrary angles)
- Freehand drawing with configurable color and stroke width
- Text annotations placed by tapping the image
- Pinch-to-zoom for viewing (separate from edit actions)
- Undo/redo for every edit action
- Reset to original image
- Export as PNG, JPEG, or WEBP at custom resolutions

**Commands:** `UndoCommand`, `RedoCommand`, `RotateCommand`, `ResetCommand`, `CropCommand`, `DrawCommand`, `TextCommand`

**Methods:** `Undo()`, `Redo()`, `Rotate(float)`, `Reset()`, `ApplyCrop()`, `ExportAsync(ImageExportOptions?)`

### ChatView

A modern chat UI control with message bubbles, typing indicators, load-more pagination, and a bottom input bar. Supports single-person and multi-person conversations with per-participant colors and avatars.

```xml
<shiny:ChatView Messages="{Binding Messages}"
                Participants="{Binding Participants}"
                IsMultiPerson="True"
                TypingParticipants="{Binding TypingParticipants}"
                SendCommand="{Binding SendCommand}"
                AttachImageCommand="{Binding AttachImageCommand}"
                LoadMoreCommand="{Binding LoadMoreCommand}"
                MyBubbleColor="#DCF8C6"
                OtherBubbleColor="White"
                PlaceholderText="Type a message..." />
```

| Property | Type | Default | Description |
|---|---|---|---|
| Messages | IList\<ChatMessage\> | null | Bindable message collection (supports INotifyCollectionChanged) |
| Participants | IList\<ChatParticipant\> | null | Participant info for avatar/color lookup |
| IsMultiPerson | bool | false | Show avatars and names for other participants |
| ShowAvatarsInSingleChat | bool | false | Force avatars even in single-person mode |
| MyBubbleColor | Color | #DCF8C6 | Local user bubble color |
| MyTextColor | Color | Black | Local user text color |
| OtherBubbleColor | Color | White | Default other-user bubble color |
| OtherTextColor | Color | Black | Other-user text color |
| PlaceholderText | string | "Type a message..." | Input placeholder |
| SendButtonText | string | "Send" | Send button label |
| IsInputBarVisible | bool | true | Show/hide the input bar |
| ShowTypingIndicator | bool | true | Enable typing notifications |
| TypingParticipants | IList\<ChatParticipant\> | null | Currently typing participants |
| ScrollToFirstUnread | bool | false | Scroll to first unread instead of end |
| FirstUnreadMessageId | string? | null | ID of the first unread message |
| UseHapticFeedback | bool | true | Haptic feedback on send |

**Commands:** `SendCommand` (ICommand, receives text string), `AttachImageCommand` (ICommand), `LoadMoreCommand` (ICommand)

**Methods:** `ScrollToEnd(bool animate)`, `ScrollToMessage(string messageId, bool animate)`

**Features:**
- Chat bubbles with left/right alignment and customizable colors per participant
- Visual grouping by sender and minute (tight spacing within group, wider between groups)
- Today timestamps show time only; previous days show full date
- Multi-person: avatar (initials or image) and name shown above first message in each group
- Single-person: avatars/names hidden by default
- Typing indicators ("{Name} is typing…", "{Name1}, {Name2} are typing", "Multiple users are typing")
- Virtualized via CollectionView with `RemainingItemsThreshold` for load-more
- Auto-link detection in text messages
- Image messages (text and image are mutually exclusive per message)
- Bottom input bar with Enter key and Send button; optional attach button
- Entire input bar can be hidden for read-only use

### PillView

Pill/chip/tag elements for displaying categories, filters, or status indicators with predefined or custom color schemes.

![Pills](assets/pills.png)

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

Each `PillType` maps to a well-known style key (e.g. `ShinyPillSuccessStyle`) that can be overridden in your app's `ResourceDictionary` to customize the preset themes.

### Fab & FabMenu

A Material Design-style floating action button, plus an expanding multi-action menu that animates up from the main FAB.

| Closed | Menu Open |
|:---:|:---:|
| ![FAB Closed](assets/fab-closed.png) | ![FAB Menu Open](assets/fab-open.png) |

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
| UseHapticFeedback | bool | true | Haptic feedback on tap |

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
| UseHapticFeedback | bool | true | Haptic feedback on toggle |

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
| UseHapticFeedback | bool | true | Haptic feedback on tap |

**Placement tip**: `FabMenu` should live in a `Grid` that fills the page (the same placement pattern as `SheetView` / `ImageViewer`) so the backdrop can cover the page content.

### SecurityPin

A PIN entry control with individually rendered cells that captures input through a hidden Entry. Digits remain visible by default and can optionally be masked with any character.

![SecurityPin](assets/securitypin.png)

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

| UseHapticFeedback | bool | Enable/disable haptic feedback on digit entry (click) and completion (long press) (default: true) |

Events: `Completed` fires with a `SecurityPinCompletedEventArgs` once the entered value reaches `Length`.

Methods: `Focus()`, `Unfocus()`, `Clear()`.

### TableView

A settings-style table view with 14+ built-in cell types, section grouping, drag-to-reorder, and dynamic data binding.

| Basic | Dynamic | Drag & Sort | Pickers | Styling |
|:---:|:---:|:---:|:---:|:---:|
| ![Basic](assets/tableview-basic.png) | ![Dynamic](assets/tableview-dynamic.png) | ![Drag & Sort](assets/tableview-dragsort.png) | ![Pickers](assets/tableview-picker.png) | ![Styling](assets/tableview-styling.png) |

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

> Separate NuGet packages: `Shiny.Maui.Controls.Markdown` / `Shiny.Blazor.Controls.Markdown`

Render and edit markdown content using native MAUI controls — no WebView required on MAUI. Auto-resolves Light/Dark theming. Available for both MAUI and Blazor.

| Viewer | Editor |
|:---:|:---:|
| ![Viewer](assets/markdown-view.png) | ![Editor](assets/markdown-editor.png) |

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

> Separate NuGet packages: `Shiny.Maui.Controls.MermaidDiagrams` / `Shiny.Blazor.Controls.MermaidDiagrams`

Native Mermaid flowchart renderer — no WebView, no SkiaSharp, AOT compatible on MAUI. Parses Mermaid syntax and renders interactive diagrams with pan and zoom support. Available for both MAUI and Blazor.

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
