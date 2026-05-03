---
name: shiny-controls
description: Generate UI for .NET MAUI (Shiny.Maui.Controls) and Blazor (Shiny.Blazor.Controls) - includes TableView with 14 cell types, FloatingPanel/OverlayHost/ShinyContentPage (bottom/top overlay panels) with detents and header peek, ShinyDurationPicker (duration picker with FloatingPanel), FrostedGlassView (native blur/glass effect), Toast service (code-invoked toast notifications with queue/stack, auto-dismiss, spinner, progress bar, pill/fill modes), PillView status badges, ImageViewer with pinch/pan/double-tap zoom, ImageEditor with crop/rotate/draw/text/undo/redo/export, ChatView with bubbles/typing/load-more/input-bar and custom MessageTemplate/MessageTemplateSelector for per-message rendering, SecurityPin entry, Fab and FabMenu (floating action button and expanding action menu), Scheduler views (calendar grid, agenda timeline, event list), Markdown controls (MarkdownView renderer, MarkdownEditor with toolbar), AutoCompleteEntry with debounced search and dropdown suggestions, CountryPicker with flag/dial code, AddressEntry with geocoding, SignaturePad for capturing signatures with canvas drawing and PNG export, Feedback Service (extensible IFeedbackService with haptic default, replaceable with TTS/sound/analytics), and UseFeedback support across all interactive controls
auto_invoke: true
triggers:
  - tableview
  - table view
  - settings page
  - settings view
  - settingsview
  - sheet view
  - sheetview
  - bottom sheet
  - bottomsheet
  - floating panel
  - floatingpanel
  - overlay host
  - overlayhost
  - shiny content page
  - shinycontentpage
  - pill
  - badge
  - status badge
  - scheduler
  - calendar
  - agenda
  - event list
  - calendar view
  - timeline
  - image viewer
  - imageviewer
  - image zoom
  - pinch to zoom
  - photo viewer
  - image editor
  - imageeditor
  - image editing
  - crop image
  - draw on image
  - annotate image
  - image annotation
  - photo editor
  - markdown
  - markdown view
  - markdown editor
  - markdown preview
  - rich text
  - security pin
  - securitypin
  - pin code
  - pin entry
  - otp
  - one time password
  - pin control
  - fab
  - floating action button
  - floating button
  - fab menu
  - fabmenu
  - speed dial
  - action menu
  - action button
  - shiny blazor controls
  - blazor tableview
  - blazor sheetview
  - blazor bottomsheet
  - blazor floating panel
  - blazor fab
  - blazor pillview
  - blazor imageviewer
  - blazor imageeditor
  - blazor securitypin
  - blazor scheduler
  - blazor markdown
  - blazor mermaid
  - autocomplete
  - auto complete
  - autocompleteentry
  - auto complete entry
  - search input
  - typeahead
  - type ahead
  - country picker
  - countrypicker
  - country selector
  - country search
  - address entry
  - addressentry
  - address search
  - address lookup
  - geocoding
  - geocode
  - blazor autocomplete
  - blazor country picker
  - blazor address entry
  - chat
  - chatview
  - chat view
  - chat bubbles
  - messaging
  - chat control
  - typing indicator
  - blazor chatview
  - signature pad
  - signaturepad
  - signature
  - signature capture
  - sign here
  - e-signature
  - esignature
  - draw signature
  - blazor signaturepad
  - blazor signature
  - duration picker
  - durationpicker
  - shinydurationpicker
  - frosted glass
  - frostedglass
  - glass effect
  - blur effect
  - acrylic
  - backdrop blur
  - glassmorphism
  - chat template
  - message template
  - chat button
  - action message
  - toast
  - toast notification
  - toast service
  - snackbar
  - show toast
  - blazor toast
  - feedback service
  - ifeedbackservice
  - haptic
  - haptic feedback
  - custom feedback
  - usefeedback
references:
  - tableview.md
  - floating-panel.md
  - pillview.md
  - image-viewer.md
  - image-editor.md
  - chatview.md
  - security-pin.md
  - fab.md
  - markdown.md
  - scheduler.md
  - autocomplete.md
  - country-picker.md
  - address-entry.md
  - signature-pad.md
  - pickers.md
  - frosted-glass.md
  - toast.md
  - feedback-service.md
---

# Shiny Controls Skill

You are an expert in the Shiny Controls library, which ships a single shared control surface across two hosts:
- **.NET MAUI** — `Shiny.Maui.Controls` (plus `Shiny.Maui.Controls.Markdown`, `Shiny.Maui.Controls.MermaidDiagrams`)
- **Blazor** — `Shiny.Blazor.Controls` (plus `Shiny.Blazor.Controls.Markdown`, `Shiny.Blazor.Controls.MermaidDiagrams`)

Every control below is available on **both** MAUI and Blazor. The feature set (properties, events, behavior) is intentionally mirrored — the same concepts apply on either host; only the syntax differs (XAML + `BindableProperty` on MAUI, Razor markup + `[Parameter]` on Blazor).

The library contains:
- **TableView**: A pure MAUI settings-style TableView with 14 cell types, cascading styles, sections, drag-sort reordering, and full MVVM/binding support
- **FloatingPanel + OverlayHost**: A floating panel overlay system (MAUI only). Panels slide from bottom or top with configurable detents, header peek when closed, backdrop dimming, and feedback. Multiple panels coexist without blocking touches. Use with `OverlayHost` (manual Grid setup) or `ShinyContentPage` (convenience ContentPage with built-in overlay). Blazor uses `SheetView` with CSS-based overlays instead
- **PillView**: A status badge/label control with 6 preset themes, custom colors, and WCAG-accessible contrast
- **ImageViewer**: A full-screen image overlay with pinch-to-zoom, pan when zoomed, double-tap to toggle zoom, animated open/close, and a close button
- **ImageEditor**: An inline image editor with cropping (drag-handle selection with dimmed overlay), rotation, freehand drawing with color, text annotations, undo/redo, reset, and export to PNG/JPEG/WEBP at configurable resolutions
- **ChatView**: A modern chat UI with message bubbles, per-participant colors and avatars, visual grouping by sender/minute, typing indicators, virtualized message list with load-more, auto-link detection, image messages, and a bottom input bar with send/attach
- **SecurityPin**: A PIN/OTP entry control with individual cells, configurable length, keyboard, and optional character masking
- **Fab**: A Material-style floating action button with Icon, Text, Command, custom colors, border, and shadow
- **FabMenu**: A floating action menu with an expanding, animated child `FabMenuItem` stack and two-way `IsOpen`
- **SchedulerCalendarView**: Monthly calendar grid with swipe navigation, event display, and pinch-to-zoom
- **SchedulerAgendaView**: Day/multi-day timeline with overlap detection, Apple Calendar-style date picker, and timezone support
- **SchedulerCalendarListView**: Vertically scrolling event list grouped by day with infinite scroll
- **MarkdownView**: A read-only markdown renderer that converts markdown text to native MAUI controls with theming and link handling
- **MarkdownEditor**: A markdown editor with formatting toolbar, live preview toggle, and customizable toolbar items
- **AutoCompleteEntry**: A text input with debounced search, dropdown suggestions, busy indicator, custom item templates, and full styling control via CSS custom properties (Blazor) or bindable properties (MAUI)
- **CountryPicker**: A country search control built on AutoCompleteEntry with flag emoji, country name, and dial code
- **AddressEntry**: An address search control built on AutoCompleteEntry with geocoding (Nominatim/OpenStreetMap by default) and structured address results
- **SignaturePad**: A signature capture control that opens in a FloatingPanel (MAUI) or SheetView (Blazor). Users draw on a canvas and export to PNG. Configurable stroke color/width, background, export dimensions, sign/cancel buttons, and panel styling. Like FloatingPanel, it must be placed inside an `OverlayHost` or `ShinyContentPage` (MAUI). The Sign button is disabled until the user draws something
- **Toast**: A service-first toast notification system invoked via DI-injected `IToaster` (registered by `UseShinyControls()`). Supports auto-dismiss with configurable duration, manual dismiss via `IDisposable`, pill or fill-horizontal display modes, top/bottom positioning, queue or stack mode for multiple toasts, indeterminate spinner, countdown progress bar, icon, tap command, feedback, and screen reader announcement. No XAML or OverlayHost required — the overlay auto-attaches to the current page. Blazor uses `IToastService` with `<ToastHost>` component
- **Feedback Service**: All interactive controls fire events through `IFeedbackService`. Default `HapticFeedbackService` provides tactile feedback. Replace with `SetCustomFeedback<T>()` in `UseShinyControls()` for TTS, sounds, analytics, or custom responses. ChatView passes message text as `details` for TTS integration

## When to Use This Skill

Invoke this skill when the user wants to:
- Create a settings page or preferences UI in .NET MAUI
- Add or configure TableView cells (switch, checkbox, entry, picker, command, etc.)
- Style a TableView with global cascading styles or per-cell overrides
- Use sections with headers, footers, and dynamic ItemTemplate cells
- Enable drag-to-reorder within a section
- Bind cell properties to a ViewModel using MVVM
- Create radio button groups, date/time pickers, number pickers, or multi-select pickers
- Build any form-like or list-based settings UI
- Add a bottom sheet / sliding panel / floating panel to a page
- Show status badges, tags, or labels (pill views)
- Display categorized status indicators (success, warning, critical, etc.)
- Add a zoomable image viewer / photo viewer overlay
- Display full-screen images with pinch-to-zoom, pan, and double-tap zoom
- Edit images with crop, rotate, draw, or text annotations
- Build an image editor with undo/redo and export
- Build a chat or messaging UI with bubbles, typing indicators, and message history
- Create a conversational interface with load-more pagination and auto-scroll
- Build a PIN entry / OTP / passcode input screen
- Capture numeric or alphanumeric codes in individual cells with optional masking
- Add a floating action button (FAB) to a page, or a speed-dial style multi-action menu
- Expose primary/contextual actions in the bottom corner with animated reveal
- Create scheduler/calendar views (monthly grid, day/week agenda, event list)
- Implement event providers for calendar data
- Customize event templates or day header templates for scheduler views
- Configure agenda timeline with overlap detection, timezone support, and time markers
- Set up infinite scrolling event lists grouped by day
- Build any calendar, appointment, or scheduling UI
- Render markdown text as native MAUI controls
- Build a markdown editor with formatting toolbar and live preview
- Display documentation, notes, or rich text content from markdown strings
- Add a search/autocomplete text input with dropdown suggestions
- Build a typeahead or search-as-you-type control with debounce
- Add a country picker or country selector with flag display
- Build an address search/lookup field with geocoding
- Implement a custom search provider for address or location queries
- Capture a signature or e-signature from the user
- Add a signature pad / drawing pad to a page
- Export a captured signature as a PNG image
- Show toast notifications, snackbar messages, or transient alerts from code
- Display progress/loading toasts with spinner while operations complete
- Queue or stack multiple notifications
- Replace haptic feedback with custom feedback (text-to-speech, sounds, analytics)
- Wire up IFeedbackService for control interaction events
- Enable text-to-speech on incoming chat messages via feedback service

## Library Overview

### .NET MAUI

**NuGet**: `Shiny.Maui.Controls` (+ `Shiny.Maui.Controls.Markdown`, `Shiny.Maui.Controls.MermaidDiagrams`)
**Namespace**: `Shiny.Maui.Controls`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)

### Blazor

**NuGet**: `Shiny.Blazor.Controls` (+ `Shiny.Blazor.Controls.Markdown`, `Shiny.Blazor.Controls.MermaidDiagrams`)
**Namespaces**: `Shiny.Blazor.Controls`, `Shiny.Blazor.Controls.Cells`, `Shiny.Blazor.Controls.Sections`, `Shiny.Blazor.Controls.Scheduler`, `Shiny.Blazor.Controls.Chat`, `Shiny.Blazor.Controls.Markdown`, `Shiny.Blazor.Controls.MermaidDiagrams`

## Setup

### .NET MAUI

1. Install the NuGet package
   ```bash
   dotnet add package Shiny.Maui.Controls
   ```

2. Configure in `MauiProgram.cs`
   ```csharp
   using Shiny;

   var builder = MauiApp.CreateBuilder();
   builder
       .UseMauiApp<App>()
       .UseShinyControls();
   ```

3. Add the XAML namespace to your pages
   ```xml
   xmlns:shiny="http://shiny.net/maui/controls"
   ```

### Blazor

1. Install the NuGet package
   ```bash
   dotnet add package Shiny.Blazor.Controls
   ```

2. Add `@using` directives (typically in `_Imports.razor`)
   ```razor
   @using Shiny.Blazor.Controls
   @using Shiny.Blazor.Controls.Cells
   @using Shiny.Blazor.Controls.Sections
   @using Shiny.Blazor.Controls.Scheduler
   @using Shiny.Blazor.Controls.Chat
   @using Shiny.Blazor.Controls.Markdown
   @using Shiny.Blazor.Controls.MermaidDiagrams
   ```

No DI registration is required for Blazor — components are used directly in `.razor` pages.

## MAUI → Blazor Translation Cheat Sheet

All controls exist on both hosts, but the Blazor surface is idiomatic Razor, not a 1:1 XAML port. When generating Blazor code, translate with these rules:

### Component name differences

| MAUI (XAML)             | Blazor (Razor)    | Notes                                           |
|-------------------------|-------------------|-------------------------------------------------|
| `shiny:TableView`       | `<TableView>`     | No prefix on Blazor; `TableRoot` is not needed  |
| `shiny:TableRoot`       | *(omitted)*       | Sections go directly inside `<TableView>`       |
| `shiny:TableSection`    | `<TableSection>`  |                                                 |
| `shiny:PillView`        | `<Pill>`          | Renamed to just `Pill` on Blazor                |
| `shiny:FloatingPanel` in `shiny:OverlayHost` | `<SheetView>` | MAUI uses FloatingPanel+OverlayHost; Blazor uses SheetView with CSS overlay. Content goes in `<SheetContent>` named slot on Blazor |
| `shiny:Fab`             | `<Fab>`           | `Icon` takes inline SVG/text string, not `ImageSource` |
| `shiny:FabMenu`         | `<FabMenu>`       | Items passed via `Items` parameter (List<FabMenuItem>), not as children |
| `shiny:ImageViewer`     | `<ImageViewer>`   | `Source` is a URL string                        |
| `shiny:ImageEditor`     | `<ImageEditor>`   | `Source` is `byte[]` (MAUI) or URL string + `ImageData` byte[] (Blazor); colors are CSS strings on Blazor |
| `shiny:ChatView`        | `<ChatView>`      | Colors are CSS strings on Blazor; `SendCommand` is `EventCallback<string>` on Blazor; uses `@using Shiny.Blazor.Controls.Chat` |
| `shiny:SecurityPin`     | `<SecurityPin>`   |                                                 |
| `md:MarkdownView`       | `<MarkdownView>`  |                                                 |
| `md:MarkdownEditor`     | `<MarkdownEditor>`|                                                 |
| `diagram:MermaidDiagramControl` | `<MermaidDiagramControl>` |                                     |
| `shiny:AutoCompleteEntry` | `<AutoCompleteEntry>` | Colors are CSS strings; `SearchCommand` is `EventCallback<string>`; supports `CssClass`, `InputClass`, `DropDownClass`, and `AdditionalAttributes` on Blazor |
| `shiny:CountryPicker`  | `<CountryPicker>` | Colors are CSS strings on Blazor |
| `shiny:AddressEntry`   | `<AddressEntry>`  | Colors are CSS strings on Blazor; uses `IAddressSearchProvider` on both hosts |
| Scheduler views        | `<SchedulerCalendarView>`, `<SchedulerAgendaView>`, `<SchedulerCalendarListView>` | Same names |
| `IToaster.ShowAsync(text, cfg => {})` | `IToastService.ShowAsync(text, cfg => {})` | MAUI uses DI-injected `IToaster` (registered by `UseShinyControls()`); Blazor uses DI-injected `IToastService`. Blazor requires `AddShinyToast()` in DI and `<ToastHost />` in layout |

### Binding, events, content

| MAUI                                                 | Blazor                                           |
|------------------------------------------------------|--------------------------------------------------|
| `IsOpen="{Binding IsOpen, Mode=TwoWay}"`             | `@bind-IsOpen="isOpen"`                          |
| `Value="{Binding Pin, Mode=TwoWay}"`                 | `@bind-Value="pin"`                              |
| `Command="{Binding AddCommand}"`                     | `Clicked="OnAdd"` / `OnClick="OnAdd"` (event callback) |
| `FontAttributes="Bold"` (PillView)                   | `Bold="true"`                                    |
| `Color="DodgerBlue"` (MAUI `Color`)                  | `Color="#1E90FF"` (CSS color strings)            |
| `ItemsSource` + `ItemTemplate` (DataTemplate)        | `ItemsSource` + `ItemTemplate` (`RenderFragment<object>`) |
| `<shiny:FloatingPanel>` content is `[ContentProperty]` PanelContent | `<SheetContent>…</SheetContent>` named slot (Blazor SheetView)    |
| `<shiny:FabMenu><shiny:FabMenuItem /></shiny:FabMenu>` | `Items="List<FabMenuItem>"` parameter         |

### Blazor-specific notes

- Use **CSS color strings** (`"#RRGGBB"`, `"rgb(...)"`, named colors) — there is no MAUI `Color` type on Blazor
- `Icon` on `Fab`/`FabMenuItem` is a string — pass an inline SVG, emoji, or single character
- `RenderFragment<object>` is the Blazor equivalent of `DataTemplate` for `ItemsSource`/`ItemTemplate`
- Event handlers take the event arg directly (e.g. `Completed="OnCompleted"` where `OnCompleted(SecurityPinCompletedEventArgs e)`), not ICommand
- Scheduler still uses `ISchedulerEventProvider` — the same interface and models work on both hosts

### Blazor quick examples

**TableView**
```razor
<TableView CellAccentColor="#10B981">
    <TableSection Title="Profile">
        <LabelCell Title="Name" ValueText="Allan Ritchie" />
        <LabelCell Title="Plan" ValueText="Pro" />
    </TableSection>
    <TableSection Title="Danger zone">
        <ButtonCell Title="Delete account"
                    ButtonTextColor="#DC2626"
                    OnClick="@(() => deleted = true)" />
    </TableSection>
</TableView>
```

**SheetView (Blazor only — MAUI uses FloatingPanel+OverlayHost)**
```razor
<button @onclick="() => isOpen = true">Open Sheet</button>

<SheetView @bind-IsOpen="isOpen"
                 Detents="detents"
                 SheetCornerRadius="20">
    <SheetContent>
        <h2>Hello from a sheet</h2>
        <button @onclick="() => isOpen = false">Close</button>
    </SheetContent>
</SheetView>

@code {
    bool isOpen;
    IList<DetentValue> detents = new List<DetentValue>
    {
        DetentValue.Quarter, DetentValue.Half, DetentValue.Full
    };
}
```

**Pill**
```razor
<Pill Text="Success" Type="PillType.Success" />
<Pill Text="Brand" PillColor="#312E81" PillTextColor="#E0E7FF" />
<Pill Text="Bold" Type="PillType.Info" Bold="true" />
```

**Fab / FabMenu**
```razor
<Fab Icon="+" FabBackgroundColor="#EC4899" Clicked="OnAdd" />

<FabMenu Items="items"
         Icon="+"
         FabBackgroundColor="#7C3AED"
         ItemTapped="OnItemTapped" />

@code {
    readonly List<FabMenuItem> items = new()
    {
        new FabMenuItem { Text = "New Note",  Icon = "📝", FabBackgroundColor = "#10B981", Tag = "note"  },
        new FabMenuItem { Text = "New Photo", Icon = "📷", FabBackgroundColor = "#F59E0B", Tag = "photo" }
    };
    void OnItemTapped(FabMenuItem item) { /* ... */ }
    void OnAdd() { /* ... */ }
}
```

**ImageViewer**
```razor
<img src="@url" @onclick="() => Open(url)" />

<ImageViewer Source="@current" @bind-IsOpen="isOpen" MaxZoom="6" />

@code {
    bool isOpen;
    string? current;
    void Open(string url) { current = url; isOpen = true; }
}
```

**ImageEditor**
```razor
<ImageEditor @ref="editor"
             Source="@imageUrl"
             ImageData="@imageData"
             AllowCrop="true"
             AllowDraw="true"
             AllowRotate="true"
             AllowTextAnnotation="true"
             DrawStrokeColor="#ff0000"
             DrawStrokeWidth="3"
             CanUndoChanged="v => canUndo = v"
             CanRedoChanged="v => canRedo = v" />

@code {
    ImageEditor? editor;
    string? imageUrl = "https://example.com/photo.jpg";
    byte[]? imageData;
    bool canUndo, canRedo;

    async Task Export()
    {
        var bytes = await editor!.ExportAsync("png");
    }
}
```

**ChatView**
```razor
@using Shiny.Blazor.Controls.Chat

<div style="height:600px;">
    <ChatView Messages="messages"
              Participants="participants"
              IsMultiPerson="true"
              TypingParticipants="typingParticipants"
              SendCommand="OnSend"
              AttachImageCommand="OnAttach"
              LoadMoreCommand="OnLoadMore"
              MyBubbleColor="#DCF8C6"
              OtherBubbleColor="#FFFFFF" />
</div>

@code {
    List<ChatMessage> messages = new();
    List<ChatParticipant> participants = new();
    List<ChatParticipant> typingParticipants = new();

    Task OnSend(string text)
    {
        messages.Add(new ChatMessage { Text = text, SenderId = "me", IsFromMe = true });
        StateHasChanged();
        return Task.CompletedTask;
    }

    Task OnAttach() => Task.CompletedTask;
    Task OnLoadMore() => Task.CompletedTask;
}
```

**SecurityPin**
```razor
<SecurityPin @bind-Value="pin"
             Length="6"
             HideCharacter="●"
             Completed="OnCompleted" />

@code {
    string pin = "";
    void OnCompleted(SecurityPinCompletedEventArgs e) { /* verify e.Value */ }
}
```

**Markdown**
```razor
<MarkdownView Markdown="@content" />
<MarkdownEditor @bind-Markdown="content" Placeholder="Write markdown…" />
```

---

# Code Generation Instructions

When generating code with Shiny.Maui.Controls:

### 1. Page Structure
- Always add `xmlns:shiny="http://shiny.net/maui/controls"` to the page
- For Markdown controls: add `xmlns:md="http://shiny.net/maui/markdown"` to the page
- For TableView: wrap content in `shiny:TableView > shiny:TableRoot > shiny:TableSection`
- For FloatingPanel (MAUI): use `shiny:ShinyContentPage` as the page base class with `PageContent` for main content and `Panels` for FloatingPanels. Alternatively, place `shiny:OverlayHost` with `shiny:FloatingPanel` children inside a Grid. Supports `Position="Bottom"` (default), `Position="Top"`, or `Position="BottomTabs"` (for use inside Shell TabBar)
- For SheetView (Blazor only): use `<SheetView>` with `<SheetContent>` child
- For ImageViewer: place `shiny:ImageViewer` inside a Grid that fills the page (it overlays on top, same pattern as SheetView)
- For ImageEditor: use `shiny:ImageEditor` with `Source` bound to `byte[]` image data. Set `AllowX` properties to toggle features. Use `CurrentToolMode` (TwoWay) to control the active tool. Use `CanUndo`/`CanRedo` (OneWayToSource) to observe undo state. Call `ExportAsync()` to save.
- For PillView: use inline within any layout
- For Scheduler views: use `shiny:SchedulerCalendarView`, `shiny:SchedulerAgendaView`, or `shiny:SchedulerCalendarListView` and bind `Provider` to an `ISchedulerEventProvider`
- For MarkdownView: use `md:MarkdownView` anywhere you need to render markdown content
- For MarkdownEditor: use `md:MarkdownEditor` for editable markdown with toolbar and preview

### 2. Cell Selection (TableView)
- Use `SwitchCell` for on/off toggles
- Use `CheckboxCell` for accept/agree checkboxes
- Use `SimpleCheckCell` for selection lists (shows/hides checkmark)
- Use `RadioCell` for mutually exclusive choices within a section
- Use `EntryCell` for text input
- Use `CommandCell` for navigation/action items with disclosure arrow
- Use `ButtonCell` for destructive or primary actions
- Use `LabelCell` for read-only display
- Use `DatePickerCell` / `TimePickerCell` for date/time selection
- Use `TextPickerCell` for dropdown selection from a list
- Use `NumberPickerCell` for numeric input with min/max
- Use `PickerCell` for full-page single or multi-select
- Use `CustomCell` for any custom MAUI view

### 3. Binding Patterns
- Always use `Mode=TwoWay` for editable properties (`On`, `Checked`, `ValueText`, `Date`, `Time`, `Number`, `SelectedIndex`, `SelectedItem`, `SelectedItems`, `IsOpen`, `IsViewerOpen`, `IsPreviewVisible`)
- Use `Mode=TwoWay` for `MarkdownEditor.Markdown` (editor content)
- Use `Mode=OneWay` (default) for display-only properties (`Title`, `Description`, `ValueText` on LabelCell, `Text` on PillView, `Source` on ImageViewer, `Markdown` on MarkdownView)
- Commands use default `Mode=OneWay`
- RadioCell selection binds at section level: `shiny:RadioCell.SelectedValue="{Binding Prop, Mode=TwoWay}"`

### 4. FloatingPanel Placement (MAUI)
Use `ShinyContentPage` for the simplest setup:

```xml
<shiny:ShinyContentPage xmlns:shiny="http://shiny.net/maui/controls">
    <shiny:ShinyContentPage.PageContent>
        <ScrollView>
            <VerticalStackLayout>
                <Button Text="Open Panel" Command="{Binding OpenCommand}" />
            </VerticalStackLayout>
        </ScrollView>
    </shiny:ShinyContentPage.PageContent>
    <shiny:ShinyContentPage.Panels>
        <shiny:FloatingPanel IsOpen="{Binding IsOpen, Mode=TwoWay}">
            <Label Text="Panel content" />
        </shiny:FloatingPanel>
    </shiny:ShinyContentPage.Panels>
</shiny:ShinyContentPage>
```

Or use `OverlayHost` manually in a Grid:

```xml
<ContentPage>
    <Grid>
        <ScrollView><!-- page content --></ScrollView>
        <shiny:OverlayHost>
            <shiny:FloatingPanel IsOpen="{Binding IsOpen, Mode=TwoWay}">
                <Label Text="Panel content" />
            </shiny:FloatingPanel>
        </shiny:OverlayHost>
    </Grid>
</ContentPage>
```

### 5. Dark Mode
- Do NOT hardcode colors. Leave color properties as `null` to inherit system defaults.
- Only set explicit colors when the design requires specific brand colors.
- The controls respect `Application.Current.UserAppTheme` automatically.

### 6. Styling Strategy
- Set global styles on `shiny:TableView` for consistent appearance
- Override at section level for section-specific header/footer styling
- Override at cell level only for individual cell emphasis
- Use `CellAccentColor` for switches, checkboxes, and radio buttons globally

## Complete TableView Example

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:shiny="http://shiny.net/maui/controls"
             x:Class="MyApp.SettingsPage"
             Title="Settings">

    <shiny:TableView CellSelectedColor="#E0E0E0" CellAccentColor="#007AFF">
        <shiny:TableRoot>
            <shiny:TableSection Title="General">
                <shiny:SwitchCell Title="Notifications" On="{Binding NotificationsOn, Mode=TwoWay}" />
                <shiny:SwitchCell Title="Sound" On="{Binding SoundOn, Mode=TwoWay}" />
                <shiny:CheckboxCell Title="Accept Analytics" Checked="{Binding AnalyticsAccepted, Mode=TwoWay}" />
            </shiny:TableSection>

            <shiny:TableSection Title="Account">
                <shiny:EntryCell Title="Name" ValueText="{Binding Name, Mode=TwoWay}" Placeholder="Your name" />
                <shiny:EntryCell Title="Email" ValueText="{Binding Email, Mode=TwoWay}" Keyboard="Email" />
                <shiny:CommandCell Title="Change Password" Command="{Binding ChangePasswordCommand}" />
            </shiny:TableSection>

            <shiny:TableSection Title="Theme" shiny:RadioCell.SelectedValue="{Binding Theme, Mode=TwoWay}">
                <shiny:RadioCell Title="Light" Value="Light" />
                <shiny:RadioCell Title="Dark" Value="Dark" />
                <shiny:RadioCell Title="System" Value="System" />
            </shiny:TableSection>

            <shiny:TableSection Title="Preferences">
                <shiny:DatePickerCell Title="Birthday" Date="{Binding Birthday, Mode=TwoWay}" Format="D" />
                <shiny:TimePickerCell Title="Daily Reminder" Time="{Binding ReminderTime, Mode=TwoWay}" />
                <shiny:NumberPickerCell Title="Font Size" Number="{Binding FontSize, Mode=TwoWay}"
                                      Min="10" Max="36" Unit="pt" />
            </shiny:TableSection>

            <shiny:TableSection Title="About">
                <shiny:LabelCell Title="Version" ValueText="1.0.0" />
                <shiny:CommandCell Title="Privacy Policy" Command="{Binding PrivacyCommand}" />
                <shiny:CommandCell Title="Terms of Service" Command="{Binding TermsCommand}" />
            </shiny:TableSection>

            <shiny:TableSection Title="Actions">
                <shiny:ButtonCell Title="Sign Out" Command="{Binding SignOutCommand}" ButtonTextColor="Red" />
            </shiny:TableSection>
        </shiny:TableRoot>
    </shiny:TableView>
</ContentPage>
```

## Complete FloatingPanel + PillView Example

```xml
<shiny:ShinyContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                         xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                         xmlns:shiny="http://shiny.net/maui/controls"
                         x:Class="MyApp.StatusPage"
                         Title="Status">

    <shiny:ShinyContentPage.PageContent>
        <ScrollView>
            <VerticalStackLayout Padding="20" Spacing="10">
                <Label Text="System Status" FontSize="24" FontAttributes="Bold" />

                <HorizontalStackLayout Spacing="8">
                    <shiny:PillView Text="API" Type="Success" />
                    <shiny:PillView Text="Database" Type="Warning" />
                    <shiny:PillView Text="Queue" Type="Critical" />
                </HorizontalStackLayout>

                <Button Text="View Details" Command="{Binding OpenDetailsCommand}" />
            </VerticalStackLayout>
        </ScrollView>
    </shiny:ShinyContentPage.PageContent>

    <shiny:ShinyContentPage.Panels>
        <shiny:FloatingPanel IsOpen="{Binding IsDetailsOpen, Mode=TwoWay}"
                             PanelCornerRadius="20">
            <VerticalStackLayout Padding="20" Spacing="12">
                <Label Text="Service Details" FontSize="18" FontAttributes="Bold" />

                <HorizontalStackLayout Spacing="6">
                    <shiny:PillView Text="Healthy" Type="Success" />
                    <Label Text="API Server" VerticalOptions="Center" />
                </HorizontalStackLayout>

                <HorizontalStackLayout Spacing="6">
                    <shiny:PillView Text="Degraded" Type="Warning" />
                    <Label Text="Database Cluster" VerticalOptions="Center" />
                </HorizontalStackLayout>

                <HorizontalStackLayout Spacing="6">
                    <shiny:PillView Text="Down" Type="Critical" />
                    <Label Text="Message Queue" VerticalOptions="Center" />
                </HorizontalStackLayout>
            </VerticalStackLayout>
        </shiny:FloatingPanel>
    </shiny:ShinyContentPage.Panels>
</shiny:ShinyContentPage>
```

## Best Practices

1. **Group logically** - Put related settings in the same section with clear headers
2. **Use FooterText** - Explain non-obvious settings in section footers
3. **Two-way bind editable values** - Always `Mode=TwoWay` for user-editable properties
4. **Leave colors null for dark mode** - Only set colors when brand-specific styling is needed
5. **Use CellAccentColor globally** - Set once on TableView instead of per-cell AccentColor
6. **Use CommandCell for navigation** - With `ShowArrow="True"` and `KeepSelectedUntilBack="True"`
7. **Use ButtonCell for destructive actions** - Red text, centered, at the bottom of the page
8. **Use RadioCell for exclusive choices** - Bind `SelectedValue` at the section level
9. **Use PickerCell for long lists** - Full-page picker is better than inline for more than 4-5 items
10. **Use ItemTemplate for dynamic content** - Bind `ItemsSource` on sections for data-driven cells
11. **Use ShinyContentPage or OverlayHost for FloatingPanels** - Use `ShinyContentPage` as the page base class, or place `OverlayHost` in a Grid for overlay panels. Place ImageViewer in a Grid as before
12. **Use PillView for status indicators** - Prefer preset types for consistency; use custom colors for brand-specific needs
13. **Use AOT-safe bindings for scheduler templates** - Always use `static (T item) => item.Property` lambda bindings, never string-based
14. **Leave MarkdownView/MarkdownEditor Theme as null** - It auto-resolves Light/Dark based on the app theme
15. **Use MarkdownView for read-only content** - Documentation, notes, changelogs; use MarkdownEditor only when the user needs to edit
16. **ImageViewer Source is set before IsOpen** - Always set the image source before opening the viewer. When Source is null, the viewer is automatically InputTransparent so it won't block touches. Use `OpenViewerOnTap="False"` when controlling the viewer programmatically
