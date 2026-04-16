---
name: shiny-controls
description: Generate .NET MAUI UI using Shiny.Maui.Controls - includes TableView with 14 cell types, BottomSheetView with detents, PillView status badges, ImageViewer with pinch/pan/double-tap zoom, SecurityPin entry, Fab and FabMenu (floating action button and expanding action menu), Scheduler views (calendar grid, agenda timeline, event list), and Markdown controls (MarkdownView renderer, MarkdownEditor with toolbar)
auto_invoke: true
triggers:
  - tableview
  - table view
  - settings page
  - settings view
  - settingsview
  - bottom sheet
  - bottomsheet
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
---

# Shiny.Maui.Controls Skill

You are an expert in Shiny.Maui.Controls, a .NET MAUI controls library containing:
- **TableView**: A pure MAUI settings-style TableView with 14 cell types, cascading styles, sections, drag-sort reordering, and full MVVM/binding support
- **BottomSheetView**: A draggable bottom sheet overlay with configurable detents, backdrop, animations, and keyboard handling
- **PillView**: A status badge/label control with 6 preset themes, custom colors, and WCAG-accessible contrast
- **ImageViewer**: A full-screen image overlay with pinch-to-zoom, pan when zoomed, double-tap to toggle zoom, animated open/close, and a close button
- **SecurityPin**: A PIN/OTP entry control with individual cells, configurable length, keyboard, and optional character masking
- **Fab**: A Material-style floating action button with Icon, Text, Command, custom colors, border, and shadow
- **FabMenu**: A floating action menu with an expanding, animated child `FabMenuItem` stack and two-way `IsOpen`
- **SchedulerCalendarView**: Monthly calendar grid with swipe navigation, event display, and pinch-to-zoom
- **SchedulerAgendaView**: Day/multi-day timeline with overlap detection, Apple Calendar-style date picker, and timezone support
- **SchedulerCalendarListView**: Vertically scrolling event list grouped by day with infinite scroll
- **MarkdownView**: A read-only markdown renderer that converts markdown text to native MAUI controls with theming and link handling
- **MarkdownEditor**: A markdown editor with formatting toolbar, live preview toggle, and customizable toolbar items

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
- Add a bottom sheet / sliding panel to a page
- Show status badges, tags, or labels (pill views)
- Display categorized status indicators (success, warning, critical, etc.)
- Add a zoomable image viewer / photo viewer overlay
- Display full-screen images with pinch-to-zoom, pan, and double-tap zoom
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

## Library Overview

**Namespace**: `Shiny.Maui.Controls`
**XAML Namespace**: `http://shiny.net/maui/controls` (prefix: `shiny`)

## Setup

### 1. Install NuGet Package
```bash
dotnet add package Shiny.Maui.Controls
```

### 2. Configure in MauiProgram.cs
```csharp
using Shiny;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseShinyControls();
```

### 3. Add XAML Namespace
```xml
xmlns:shiny="http://shiny.net/maui/controls"
```

---

# TableView

## Architecture

The control hierarchy is:
```
shiny:TableView (ContentView > ScrollView > VerticalStackLayout)
  shiny:TableRoot
    shiny:TableSection (Title, FooterText, cells)
      shiny:LabelCell / shiny:SwitchCell / shiny:CommandCell / etc.
```

**Style cascade**: TableView globals -> Section overrides -> Cell overrides. All properties are `BindableProperty` with full MVVM support.

## Cell Types (14 Total)

### LabelCell
Displays title with optional value text on the right.

```xml
<shiny:LabelCell Title="Version" ValueText="1.0.0" Description="Latest release" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `ValueText` | `string` | `""` | Right-side text |
| `ValueTextColor` | `Color?` | `null` | Value color |
| `ValueTextFontSize` | `double` | `-1` | Value font size |
| `ValueTextFontFamily` | `string?` | `null` | Value font family |
| `ValueTextFontAttributes` | `FontAttributes?` | `null` | Value styling |

### SwitchCell
Toggle switch.

```xml
<shiny:SwitchCell Title="Wi-Fi" On="{Binding WifiEnabled, Mode=TwoWay}" OnColor="#34C759" />
```

| Property | Type | Default |
|---|---|---|
| `On` | `bool` | `false` |
| `OnColor` | `Color?` | `null` |

### CheckboxCell
Native checkbox control.

```xml
<shiny:CheckboxCell Title="Accept Terms" Checked="{Binding Accepted, Mode=TwoWay}" AccentColor="Green" />
```

| Property | Type | Default |
|---|---|---|
| `Checked` | `bool` | `false` |
| `AccentColor` | `Color?` | `null` |

### SimpleCheckCell
Checkmark toggle (no native checkbox, just a checkmark character).

```xml
<shiny:SimpleCheckCell Title="Option A" Checked="{Binding OptionA, Mode=TwoWay}" />
```

| Property | Type | Default |
|---|---|---|
| `Checked` | `bool` | `false` |
| `Value` | `object?` | `null` |
| `AccentColor` | `Color?` | `null` |

### RadioCell
Radio button selection. Use `RadioCell.SelectedValue` attached property on the section.

```xml
<shiny:TableSection Title="Theme" shiny:RadioCell.SelectedValue="{Binding SelectedTheme, Mode=TwoWay}">
    <shiny:RadioCell Title="Light" Value="Light" />
    <shiny:RadioCell Title="Dark" Value="Dark" />
    <shiny:RadioCell Title="System" Value="System" />
</shiny:TableSection>
```

| Property | Type | Default |
|---|---|---|
| `Value` | `object?` | `null` |
| `AccentColor` | `Color?` | `null` |

### CommandCell
Tappable cell with disclosure arrow. Inherits from LabelCell.

```xml
<shiny:CommandCell Title="About" ValueText="Learn more"
                 Command="{Binding AboutCommand}"
                 KeepSelectedUntilBack="True" />
```

| Property | Type | Default |
|---|---|---|
| `Command` | `ICommand?` | `null` |
| `CommandParameter` | `object?` | `null` |
| `ShowArrow` | `bool` | `true` |
| `KeepSelectedUntilBack` | `bool` | `false` |

### ButtonCell
Full-width button-style cell.

```xml
<shiny:ButtonCell Title="Sign Out" Command="{Binding SignOutCommand}" ButtonTextColor="Red" />
```

| Property | Type | Default |
|---|---|---|
| `Command` | `ICommand?` | `null` |
| `CommandParameter` | `object?` | `null` |
| `ButtonTextColor` | `Color?` | `null` |
| `TitleAlignment` | `TextAlignment` | `Center` |

### EntryCell
Inline text input.

```xml
<shiny:EntryCell Title="Email" ValueText="{Binding Email, Mode=TwoWay}"
               Placeholder="user@example.com" Keyboard="Email" />
```

| Property | Type | Default |
|---|---|---|
| `ValueText` | `string` | `""` |
| `Placeholder` | `string` | `""` |
| `PlaceholderColor` | `Color?` | `null` |
| `Keyboard` | `Keyboard` | `Default` |
| `IsPassword` | `bool` | `false` |
| `MaxLength` | `int` | `-1` |
| `TextAlignment` | `TextAlignment` | `End` |
| `CompletedCommand` | `ICommand?` | `null` |
| `ValueTextColor` | `Color?` | `null` |

### DatePickerCell
Opens native date picker dialog on tap.

```xml
<shiny:DatePickerCell Title="Birthday" Date="{Binding BirthDate, Mode=TwoWay}" Format="D" />
```

| Property | Type | Default |
|---|---|---|
| `Date` | `DateTime?` | `null` |
| `InitialDate` | `DateTime` | `2000-01-01` |
| `MinimumDate` | `DateTime` | `1900-01-01` |
| `MaximumDate` | `DateTime` | `2100-12-31` |
| `Format` | `string` | `"d"` |
| `ValueTextColor` | `Color?` | `null` |

### TimePickerCell
Opens native time picker dialog on tap.

```xml
<shiny:TimePickerCell Title="Alarm" Time="{Binding AlarmTime, Mode=TwoWay}" Format="T" />
```

| Property | Type | Default |
|---|---|---|
| `Time` | `TimeSpan` | `00:00:00` |
| `Format` | `string` | `"t"` |
| `ValueTextColor` | `Color?` | `null` |

### TextPickerCell
Opens native dropdown/spinner picker on tap.

```xml
<shiny:TextPickerCell Title="Color" ItemsSource="{Binding Colors}"
                    SelectedIndex="{Binding SelectedColorIndex, Mode=TwoWay}" />
```

| Property | Type | Default |
|---|---|---|
| `ItemsSource` | `IList?` | `null` |
| `SelectedIndex` | `int` | `-1` |
| `SelectedItem` | `object?` | `null` |
| `DisplayMember` | `string?` | `null` |
| `PickerTitle` | `string?` | `null` |
| `SelectedCommand` | `ICommand?` | `null` |
| `ValueTextColor` | `Color?` | `null` |

### NumberPickerCell
Opens a prompt dialog for numeric input.

```xml
<shiny:NumberPickerCell Title="Font Size" Number="{Binding FontSize, Mode=TwoWay}"
                      Min="8" Max="72" Unit="pt" />
```

| Property | Type | Default |
|---|---|---|
| `Number` | `int?` | `null` |
| `Min` | `int` | `0` |
| `Max` | `int` | `9999` |
| `Unit` | `string` | `""` |
| `PickerTitle` | `string` | `"Enter a number"` |
| `SelectedCommand` | `ICommand?` | `null` |
| `ValueTextColor` | `Color?` | `null` |

### PickerCell
Full-page picker for single or multi-select. Navigates to a selection page.

```xml
<!-- Single select -->
<shiny:PickerCell Title="Country" ItemsSource="{Binding Countries}"
                SelectionMode="Single" SelectedItem="{Binding SelectedCountry, Mode=TwoWay}"
                PageTitle="Select Country" />

<!-- Multi select -->
<shiny:PickerCell Title="Hobbies" ItemsSource="{Binding Hobbies}"
                SelectionMode="Multiple" MaxSelectedNumber="3"
                SelectedItems="{Binding SelectedHobbies, Mode=TwoWay}" />
```

| Property | Type | Default |
|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` |
| `SelectedItem` | `object?` | `null` |
| `SelectedItems` | `IList?` | `null` |
| `SelectionMode` | `SelectionMode` | `Single` |
| `MaxSelectedNumber` | `int` | `0` (unlimited) |
| `UsePickToClose` | `bool` | `false` |
| `UseAutoValueText` | `bool` | `true` |
| `DisplayMember` | `string?` | `null` |
| `SubDisplayMember` | `string?` | `null` |
| `PageTitle` | `string` | `"Select"` |
| `ShowArrow` | `bool` | `true` |
| `KeepSelectedUntilBack` | `bool` | `false` |
| `SelectedCommand` | `ICommand?` | `null` |
| `AccentColor` | `Color?` | `null` |

### CustomCell
Hosts any custom MAUI view.

```xml
<shiny:CustomCell Title="Progress">
    <shiny:CustomCell.CustomContent>
        <ProgressBar Progress="0.75" />
    </shiny:CustomCell.CustomContent>
</shiny:CustomCell>
```

| Property | Type | Default |
|---|---|---|
| `CustomContent` | `View?` | `null` |
| `UseFullSize` | `bool` | `false` |
| `Command` | `ICommand?` | `null` |
| `LongCommand` | `ICommand?` | `null` |
| `ShowArrow` | `bool` | `false` |
| `KeepSelectedUntilBack` | `bool` | `false` |

## Common Cell Properties (CellBase)

All cells inherit these properties:

| Property | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Primary text |
| `TitleColor` | `Color?` | `null` | Title color |
| `TitleFontSize` | `double` | `-1` | Title font size |
| `TitleFontFamily` | `string?` | `null` | Title font family |
| `TitleFontAttributes` | `FontAttributes?` | `null` | Bold, Italic, None |
| `Description` | `string` | `""` | Subtitle below title |
| `DescriptionColor` | `Color?` | `null` | Description color |
| `DescriptionFontSize` | `double` | `-1` | Description font size |
| `HintText` | `string` | `""` | Right of title hint |
| `HintTextColor` | `Color?` | `null` | Hint color |
| `IconSource` | `ImageSource?` | `null` | Left icon |
| `IconSize` | `double` | `-1` | Icon dimensions |
| `IconRadius` | `double` | `-1` | Icon corner radius |
| `CellBackgroundColor` | `Color?` | `null` | Background color |
| `SelectedColor` | `Color?` | `null` | Tap highlight color |
| `IsSelectable` | `bool` | `true` | Responds to taps |
| `CellHeight` | `double` | `-1` | Fixed height |
| `BorderColor` | `Color?` | `null` | Border color |
| `BorderWidth` | `double` | `-1` | Border width |
| `BorderRadius` | `double` | `-1` | Border corner radius |

## TableSection Properties

```xml
<shiny:TableSection Title="GENERAL" FooterText="These settings apply globally"
                  HeaderBackgroundColor="#F2F2F7" HeaderTextColor="#666666"
                  UseDragSort="False">
    <!-- cells -->
</shiny:TableSection>
```

| Property | Type | Default |
|---|---|---|
| `Title` | `string` | `""` |
| `FooterText` | `string` | `""` |
| `HeaderView` | `View?` | `null` |
| `FooterView` | `View?` | `null` |
| `IsVisible` | `bool` | `true` |
| `FooterVisible` | `bool` | `true` |
| `HeaderBackgroundColor` | `Color?` | `null` |
| `HeaderTextColor` | `Color?` | `null` |
| `HeaderFontSize` | `double` | `-1` |
| `HeaderFontFamily` | `string?` | `null` |
| `HeaderFontAttributes` | `FontAttributes?` | `null` |
| `HeaderHeight` | `double` | `-1` |
| `FooterTextColor` | `Color?` | `null` |
| `FooterFontSize` | `double` | `-1` |
| `FooterBackgroundColor` | `Color?` | `null` |
| `UseDragSort` | `bool` | `false` |
| `ItemsSource` | `IEnumerable?` | `null` |
| `ItemTemplate` | `DataTemplate?` | `null` |
| `TemplateStartIndex` | `int` | `0` |

## Dynamic Cells with ItemTemplate

Generate cells from a data source:

```xml
<shiny:TableSection Title="Items" ItemsSource="{Binding Items}">
    <shiny:TableSection.ItemTemplate>
        <DataTemplate>
            <shiny:LabelCell Title="{Binding Name}" ValueText="{Binding Value}" />
        </DataTemplate>
    </shiny:TableSection.ItemTemplate>
</shiny:TableSection>
```

The `ItemsSource` supports `INotifyCollectionChanged` for live updates.

## Global Styling (TableView Properties)

Apply styles at the TableView level. Individual cell/section properties override globals.

```xml
<shiny:TableView CellTitleColor="#333333"
              CellTitleFontSize="17"
              CellDescriptionColor="#888888"
              CellValueTextColor="#007AFF"
              CellBackgroundColor="White"
              CellSelectedColor="#EFEFEF"
              CellAccentColor="#007AFF"
              CellIconSize="28"
              HeaderTextColor="#666666"
              HeaderFontSize="13"
              HeaderBackgroundColor="#F2F2F7"
              FooterTextColor="#8E8E93"
              SeparatorColor="#C6C6C8"
              SeparatorPadding="16"
              SectionSeparatorHeight="12">
```

### Cell Global Styles

| Property | Type | Description |
|---|---|---|
| `CellTitleColor` | `Color?` | Title color for all cells |
| `CellTitleFontSize` | `double` | Title font size |
| `CellTitleFontFamily` | `string?` | Title font family |
| `CellTitleFontAttributes` | `FontAttributes?` | Title styling |
| `CellDescriptionColor` | `Color?` | Description color |
| `CellDescriptionFontSize` | `double` | Description font size |
| `CellHintTextColor` | `Color?` | Hint text color |
| `CellHintTextFontSize` | `double` | Hint font size |
| `CellValueTextColor` | `Color?` | Value text color |
| `CellValueTextFontSize` | `double` | Value font size |
| `CellBackgroundColor` | `Color?` | Cell background |
| `CellSelectedColor` | `Color?` | Tap highlight color |
| `CellAccentColor` | `Color?` | Switches, checkboxes, radios |
| `CellIconSize` | `double` | Icon dimensions |
| `CellIconRadius` | `double` | Icon corner radius |
| `CellPadding` | `Thickness?` | Cell content padding |
| `CellBorderColor` | `Color?` | Cell border color |
| `CellBorderWidth` | `double` | Cell border width |
| `CellBorderRadius` | `double` | Cell border corner radius |

### Header/Footer Global Styles

| Property | Type | Default |
|---|---|---|
| `HeaderBackgroundColor` | `Color?` | `null` |
| `HeaderTextColor` | `Color?` | `null` |
| `HeaderFontSize` | `double` | `-1` |
| `HeaderFontFamily` | `string?` | `null` |
| `HeaderFontAttributes` | `FontAttributes` | `Bold` |
| `HeaderPadding` | `Thickness` | `14,8,8,8` |
| `HeaderHeight` | `double` | `-1` |
| `HeaderTextVerticalAlign` | `LayoutAlignment` | `End` |
| `FooterTextColor` | `Color?` | `null` |
| `FooterFontSize` | `double` | `-1` |
| `FooterFontAttributes` | `FontAttributes` | `None` |
| `FooterPadding` | `Thickness` | `14,8,8,8` |
| `FooterBackgroundColor` | `Color?` | `null` |

### Separator/Section Styles

| Property | Type | Default |
|---|---|---|
| `SeparatorColor` | `Color?` | `null` |
| `SeparatorHeight` | `double` | `0.5` |
| `SeparatorPadding` | `double` | `16` |
| `ShowSectionSeparator` | `bool` | `true` |
| `SectionSeparatorHeight` | `double` | `8` |
| `SectionSeparatorColor` | `Color?` | `null` |

## Drag & Sort

Enable reorder controls (up/down arrows) on a section:

```xml
<shiny:TableView ItemDroppedCommand="{Binding ItemDroppedCommand}">
    <shiny:TableRoot>
        <shiny:TableSection Title="Reorder" UseDragSort="True">
            <shiny:LabelCell Title="First" ValueText="1" />
            <shiny:LabelCell Title="Second" ValueText="2" />
            <shiny:LabelCell Title="Third" ValueText="3" />
        </shiny:TableSection>
    </shiny:TableRoot>
</shiny:TableView>
```

The `ItemDroppedCommand` receives `ItemDroppedEventArgs` with `Section`, `Cell`, `FromIndex`, `ToIndex`.

## Scroll Control

```xml
<shiny:TableView ScrollToTop="{Binding ShouldScrollTop}" ScrollToBottom="{Binding ShouldScrollBottom}" />
```

```csharp
await tableView.ScrollToTopAsync();
await tableView.ScrollToBottomAsync();
```

## TableView Events

| Event | Args | Description |
|---|---|---|
| `ItemDropped` | `ItemDroppedEventArgs` | Cell reordered via drag sort |
| `ModelChanged` | `EventArgs` | Root/sections/cells changed |
| `CellPropertyChanged` | `CellPropertyChangedEventArgs` | Cell property changed |

---

# BottomSheetView

A draggable bottom sheet overlay that slides up from the bottom of the page. Supports multiple snap points (detents), backdrop dimming, pan gestures, and automatic keyboard handling.

## Basic Usage

```xml
<shiny:BottomSheetView IsOpen="{Binding IsSheetOpen, Mode=TwoWay}">
    <VerticalStackLayout Padding="20" Spacing="10">
        <Label Text="Sheet Content" FontSize="18" FontAttributes="Bold" />
        <Label Text="Drag the handle or tap the backdrop to close." />
        <Button Text="Close" Command="{Binding CloseSheetCommand}" />
    </VerticalStackLayout>
</shiny:BottomSheetView>
```

## BottomSheetView Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `IsOpen` | `bool` | `false` | Opens/closes the sheet (two-way bindable) |
| `SheetContent` | `View?` | `null` | The content displayed inside the sheet (ContentProperty) |
| `Detents` | `ObservableCollection<DetentValue>` | Quarter, Half, Full | Snap points as ratios of available height |
| `SheetBackgroundColor` | `Color` | `White` | Background color of the sheet panel |
| `HandleColor` | `Color` | `Grey` | Color of the drag handle indicator |
| `SheetCornerRadius` | `double` | `16` | Top corner radius of the sheet |
| `HasBackdrop` | `bool` | `true` | Shows a dimming backdrop behind the sheet |
| `CloseOnBackdropTap` | `bool` | `true` | Tapping the backdrop closes the sheet |
| `AnimationDuration` | `double` | `250` | Animation duration in milliseconds |
| `ExpandOnInputFocus` | `bool` | `true` | Auto-expands to highest detent when an input is focused |

## DetentValue

Predefined snap points (or create custom ones):

| Static Property | Ratio | Description |
|---|---|---|
| `DetentValue.Quarter` | `0.25` | 25% of available height |
| `DetentValue.Half` | `0.50` | 50% of available height |
| `DetentValue.ThreeQuarters` | `0.75` | 75% of available height |
| `DetentValue.Full` | `1.0` | Full available height |

Custom detent: `new DetentValue(0.33)` for 33% height.

## BottomSheetView Events

| Event | Args | Description |
|---|---|---|
| `Opened` | `EventArgs` | Sheet finished opening animation |
| `Closed` | `EventArgs` | Sheet finished closing animation |
| `DetentChanged` | `DetentValue` | Sheet snapped to a different detent |

## Public Methods

| Method | Description |
|---|---|
| `AnimateToDetentAsync(DetentValue)` | Programmatically animate to a specific detent |

## BottomSheetView Features

- **Pan gesture**: Drag the sheet up/down between detents; swipe down past lowest detent to close
- **Keyboard handling**: Automatically expands when an Entry/Editor is focused (Android AdjustResize), restores when keyboard dismissed
- **ScrollView integration**: Scroll is enabled only at the highest detent; disabled at lower detents to allow pan gestures
- **Backdrop**: Semi-transparent overlay that dims proportionally to sheet position

## BottomSheetView Example with Custom Detents

```xml
<shiny:BottomSheetView IsOpen="{Binding IsOpen, Mode=TwoWay}"
                       SheetBackgroundColor="#1E1E1E"
                       HandleColor="#888888"
                       SheetCornerRadius="24"
                       HasBackdrop="True"
                       CloseOnBackdropTap="True"
                       AnimationDuration="300">
    <shiny:BottomSheetView.Detents>
        <shiny:DetentValue Ratio="0.33" />
        <shiny:DetentValue Ratio="0.66" />
        <shiny:DetentValue Ratio="1.0" />
    </shiny:BottomSheetView.Detents>
    <VerticalStackLayout Padding="20">
        <Label Text="Custom Sheet" TextColor="White" />
    </VerticalStackLayout>
</shiny:BottomSheetView>
```

---

# PillView

A status badge/label control with preset color themes and custom color support. Uses WCAG luminance calculations for accessible text contrast.

## Basic Usage

```xml
<!-- Preset types -->
<shiny:PillView Text="Active" Type="Success" />
<shiny:PillView Text="Pending" Type="Warning" />
<shiny:PillView Text="Error" Type="Critical" />
<shiny:PillView Text="Info" Type="Info" />
<shiny:PillView Text="Notice" Type="Caution" />

<!-- Custom color (auto-calculates text and border) -->
<shiny:PillView Text="Custom" PillColor="#6366F1" />

<!-- Full override -->
<shiny:PillView Text="Manual" PillColor="#1E1E1E" PillTextColor="White" PillBorderColor="#444" />
```

## PillView Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Text` | `string` | `""` | The text displayed in the pill |
| `Type` | `PillType` | `None` | Preset theme: None, Success, Info, Warning, Caution, Critical |
| `PillColor` | `Color?` | `null` | Custom background color (overrides Type when set) |
| `PillTextColor` | `Color?` | `null` | Text color override (auto-calculated if null) |
| `PillBorderColor` | `Color?` | `null` | Border color override (auto-calculated if null) |
| `FontSize` | `double` | `12` | Text font size |
| `CornerRadius` | `double` | `12` | Border corner radius |
| `FontAttributes` | `FontAttributes` | `None` | Bold, Italic, None |

## PillType Preset Colors

| Type | Background | Text | Border |
|---|---|---|---|
| `None` | `#F3F4F6` | `#374151` | `#D1D5DB` |
| `Success` | `#DCFCE7` | `#166534` | `#86EFAC` |
| `Info` | `#DBEAFE` | `#1E40AF` | `#93C5FD` |
| `Warning` | `#FEF9C3` | `#854D0E` | `#FDE047` |
| `Caution` | `#FFEDD5` | `#9A3412` | `#FDBA74` |
| `Critical` | `#FEE2E2` | `#991B1B` | `#FCA5A5` |

## PillView Color Behavior

- Setting `PillColor` auto-calculates contrast text color (WCAG luminance) and a darkened border color
- Setting `PillTextColor` overrides auto-calculated text color
- Setting `PillBorderColor` overrides auto-calculated border color
- Setting `Type` applies the preset theme; `PillTextColor`/`PillBorderColor` still override if set

---

# ImageViewer

A full-screen image overlay with pinch-to-zoom, pan (when zoomed), double-tap to toggle zoom, animated fade open/close, and a close button. Designed to overlay page content like BottomSheetView.

## Basic Usage

```xml
<Grid>
    <!-- Main page content -->
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

## ImageViewer Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Source` | `ImageSource?` | `null` | OneWay | The image to display |
| `IsOpen` | `bool` | `false` | TwoWay | Opens/closes the viewer with fade animation |
| `MaxZoom` | `double` | `5.0` | OneWay | Maximum pinch zoom scale |

## ImageViewer Features

- **Pinch-to-zoom**: Two-finger pinch gesture scales around the pinch origin, clamped between 1x and MaxZoom
- **Pan when zoomed**: One-finger pan is enabled after zooming in, with translation clamped to image bounds
- **Double-tap to zoom**: Double-tap zooms to 2.5x centered on the tap point; double-tap again resets to 1x
- **Animated open/close**: Backdrop, image, and close button fade in/out together (250ms)
- **Close button**: "✕" button in the top-right corner to close the viewer
- **Backdrop**: Black overlay that swallows touches so nothing falls through to the page behind

## ImageViewer Placement

Like BottomSheetView, ImageViewer must be placed inside a Grid that fills the page so it overlays correctly:

```xml
<ContentPage>
    <Grid>
        <!-- Main page content -->
        <ScrollView>
            <!-- ... -->
        </ScrollView>

        <!-- ImageViewer overlays on top -->
        <shiny:ImageViewer Source="{Binding SelectedImage}"
                           IsOpen="{Binding IsViewerOpen}" />
    </Grid>
</ContentPage>
```

## ImageViewer ViewModel Pattern

```csharp
public partial class ImageViewerViewModel : ObservableObject
{
    [ObservableProperty] ImageSource? selectedImage;
    [ObservableProperty] bool isViewerOpen;

    [RelayCommand]
    void OpenViewer(string imageSource)
    {
        SelectedImage = imageSource;
        IsViewerOpen = true;
    }
}
```

---

# SecurityPin

A PIN/OTP entry control with individually rendered cells. Input is captured by a hidden `Entry` so the native keyboard appears when any cell is tapped. Digits are visible by default and can optionally be masked with any character for password-style entry.

## Basic Usage

```xml
<!-- 4-digit visible numeric PIN -->
<shiny:SecurityPin Length="4"
                   Value="{Binding Pin}"
                   Keyboard="Numeric" />

<!-- 6-digit masked PIN -->
<shiny:SecurityPin Length="6"
                   HideCharacter="*"
                   Value="{Binding Pin}"
                   Keyboard="Numeric" />
```

## SecurityPin Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Length` | `int` | `4` | OneWay | Number of PIN cells |
| `Value` | `string` | `""` | TwoWay | Current entered value |
| `Keyboard` | `Keyboard` | `Numeric` | OneWay | Keyboard type applied to the hidden Entry |
| `HideCharacter` | `string?` | `null` | OneWay | When set, masks entered characters; when null/empty, shows the actual value |
| `CellSize` | `double` | `50` | OneWay | Width and height of each cell |
| `CellSpacing` | `double` | `8` | OneWay | Horizontal spacing between cells |
| `CellCornerRadius` | `double` | `8` | OneWay | Corner radius of the cell border |
| `CellBorderColor` | `Color?` | `null` | OneWay | Border stroke color |
| `CellFocusedBorderColor` | `Color?` | `null` | OneWay | Border stroke color for the currently active cell |
| `CellBackgroundColor` | `Color?` | `null` | OneWay | Cell fill color |
| `CellTextColor` | `Color?` | `null` | OneWay | Color for the rendered digit/character |
| `FontSize` | `double` | `24` | OneWay | Font size for the rendered character |

## Events

| Event | Args | Description |
|---|---|---|
| `Completed` | `SecurityPinCompletedEventArgs(string Value)` | Fires when the entered value reaches `Length` |

## Methods

| Method | Description |
|---|---|
| `Focus()` | Focus the hidden Entry and show the keyboard |
| `Unfocus()` | Dismiss the keyboard |
| `Clear()` | Reset `Value` to an empty string |

## Code Generation Guidance

- When the user asks for a PIN/OTP/passcode entry field, use `SecurityPin` — do not stitch together multiple `Entry` controls
- Default `Length` to 4 unless the user specifies otherwise (common alternatives: 6 for OTP codes)
- Keep `Keyboard="Numeric"` for numeric PINs; only switch to `Default` when the user asks for alphanumeric entry
- Set `HideCharacter="*"` (or `"●"`) only when the user wants masked/hidden entry — leave null for visible digits
- Wire `Completed` when the caller wants to react automatically after the last digit is entered
- Bind `Value` TwoWay to the ViewModel so the PIN is observable and `Clear()` / reassignment works as expected

## SecurityPin ViewModel Pattern

```csharp
public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    string pin = string.Empty;

    [RelayCommand]
    void ClearPin() => Pin = string.Empty;
}
```

```xml
<shiny:SecurityPin Length="4"
                   HideCharacter="*"
                   Value="{Binding Pin}"
                   Completed="OnPinCompleted" />
```

```csharp
void OnPinCompleted(object? sender, SecurityPinCompletedEventArgs e)
{
    // e.Value contains the full entered PIN
    _ = ((LoginViewModel)BindingContext).VerifyAsync(e.Value);
}
```

## Styled Example

```xml
<shiny:SecurityPin Length="5"
                   HideCharacter="●"
                   CellSize="48"
                   CellSpacing="10"
                   CellCornerRadius="12"
                   CellBorderColor="LightGray"
                   CellFocusedBorderColor="DodgerBlue"
                   CellBackgroundColor="#F5F5F5"
                   CellTextColor="Black"
                   FontSize="22"
                   Value="{Binding Pin}" />
```

## Behavior Notes

- Tapping any cell focuses the hidden Entry; the native keyboard appears
- The hidden Entry's `MaxLength` is synced with `Length`
- Changing `Length` rebuilds the cells; a longer `Value` is truncated to fit
- When `HideCharacter` is null or empty, the entered character is shown verbatim
- `Completed` fires whenever the value length reaches `Length` — including via programmatic assignment

---

# Fab & FabMenu

Material-style floating action button (`Fab`) and an expanding multi-action menu (`FabMenu`) that animates `FabMenuItem` children up from the main FAB with a staggered reveal.

## Basic Usage

```xml
<!-- Single icon-only Fab pinned to bottom-right -->
<shiny:Fab Icon="add.png"
           FabBackgroundColor="#E91E63"
           Command="{Binding AddCommand}"
           HorizontalOptions="End"
           VerticalOptions="End"
           Margin="24" />

<!-- Extended Fab (icon + text) -->
<shiny:Fab Icon="add.png"
           Text="Add Item"
           FabBackgroundColor="#4CAF50"
           TextColor="White"
           Command="{Binding AddCommand}" />

<!-- FabMenu (speed dial) -->
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

## Placement (important)

Place the `Fab` / `FabMenu` inside a `Grid` that fills the page — same pattern as `BottomSheetView` / `ImageViewer`:

```xml
<ContentPage>
    <Grid>
        <ScrollView>
            <!-- page content -->
        </ScrollView>

        <shiny:FabMenu IsOpen="{Binding IsMenuOpen}"
                       Icon="plus.png"
                       HorizontalOptions="End"
                       VerticalOptions="End"
                       Margin="24">
            <!-- items -->
        </shiny:FabMenu>
    </Grid>
</ContentPage>
```

## Fab Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Icon` | `ImageSource?` | `null` | OneWay | Icon shown inside the button |
| `Text` | `string?` | `null` | OneWay | Optional label; when null the Fab is a perfect circle |
| `Command` | `ICommand?` | `null` | OneWay | Executed on tap |
| `CommandParameter` | `object?` | `null` | OneWay | Parameter forwarded to the Command |
| `FabBackgroundColor` | `Color` | `#2196F3` | OneWay | Fill color |
| `BorderColor` | `Color?` | `null` | OneWay | Outline stroke color |
| `BorderThickness` | `double` | `0` | OneWay | Outline stroke thickness |
| `TextColor` | `Color` | `White` | OneWay | Label text color |
| `FontSize` | `double` | `14` | OneWay | Label font size |
| `FontAttributes` | `FontAttributes` | `None` | OneWay | Label font attributes |
| `Size` | `double` | `56` | OneWay | Height (diameter when circular) |
| `IconSize` | `double` | `24` | OneWay | Icon image size |
| `HasShadow` | `bool` | `true` | OneWay | Drop shadow on/off |

Events: `Clicked`.

## FabMenu Properties

In addition to the main-Fab pass-throughs (`Icon`, `Text`, `FabBackgroundColor`, `BorderColor`, `BorderThickness`, `TextColor`):

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `IsOpen` | `bool` | `false` | TwoWay | Opens/closes the menu with animation |
| `Items` | `IList<FabMenuItem>` | empty | — | Content property; place items directly inside `<shiny:FabMenu>` |
| `HasBackdrop` | `bool` | `true` | OneWay | Show dim backdrop while open |
| `BackdropColor` | `Color` | `Black` | OneWay | Backdrop color |
| `BackdropOpacity` | `double` | `0.4` | OneWay | Backdrop peak opacity |
| `CloseOnBackdropTap` | `bool` | `true` | OneWay | Close when backdrop tapped |
| `CloseOnItemTap` | `bool` | `true` | OneWay | Close after item tap |
| `AnimationDuration` | `uint` | `200` | OneWay | Open/close animation duration (ms) |

Events: `ItemTapped(FabMenuItem)`.
Methods: `Open()`, `Close()`, `Toggle()`.

## FabMenuItem Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `Icon` | `ImageSource?` | `null` | Icon rendered inside the circular button |
| `Text` | `string?` | `null` | Side label |
| `Command` | `ICommand?` | `null` | Invoked on tap |
| `CommandParameter` | `object?` | `null` | Parameter forwarded to the Command |
| `FabBackgroundColor` | `Color` | `#2196F3` | Icon button fill |
| `BorderColor` | `Color?` | `null` | Icon button outline |
| `BorderThickness` | `double` | `0` | Icon button outline thickness |
| `TextColor` | `Color` | `Black` | Side-label text color |
| `LabelBackgroundColor` | `Color` | `White` | Side-label fill color |
| `FontSize` | `double` | `13` | Side-label font size |
| `Size` | `double` | `44` | Icon button diameter |
| `IconSize` | `double` | `20` | Icon image size |

Events: `Clicked`.

## Behavior & Animation

- Tapping the main Fab of a `FabMenu` toggles `IsOpen`
- Opening the menu fades in the backdrop and animates each `FabMenuItem` up (fade + translate) with a small stagger; closing reverses it
- `IsOpen` is fully two-way bindable — setting it from a ViewModel animates in/out
- Child items' own animations never conflict with the main Fab — it stays fixed
- Tapping a menu item executes its `Command`, raises `ItemTapped` on the menu, and closes the menu when `CloseOnItemTap` is true (default)

## Code Generation Guidance

- Use `Fab` for a single primary action (e.g., "Add") and `FabMenu` for multiple related actions (speed dial)
- Always place `Fab` / `FabMenu` inside a Grid that fills the page so the FabMenu backdrop overlays everything
- Default to `HorizontalOptions="End"` + `VerticalOptions="End"` + `Margin="24"` for the canonical Material bottom-right placement
- Bind `IsOpen` TwoWay when the ViewModel needs to drive the menu state; otherwise omit it and let taps control it
- Keep `FabMenuItem` icons monochrome/filled for best visual contrast against the colored circles
- Use `Icon` on every item when possible; `Text` is optional but strongly recommended for accessibility

## ViewModel Pattern

```csharp
public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty] bool isMenuOpen;

    [RelayCommand] void Add()    { /* ... */ }
    [RelayCommand] void Share()  { /* ... */ }
    [RelayCommand] void Edit()   { /* ... */ }
    [RelayCommand] void Delete() { /* ... */ }
}
```

---

# Markdown Controls

Two controls for rendering and editing markdown content. Uses Markdig for parsing. Separate NuGet package from the main controls library.

**NuGet Package**: `Shiny.Maui.Controls.Markdown`
**Namespace**: `Shiny.Maui.Controls.Markdown`
**XAML Namespace**: `http://shiny.net/maui/markdown` (prefix: `md`)

```xml
xmlns:md="http://shiny.net/maui/markdown"
```

## MarkdownView

A read-only markdown renderer that converts markdown text to native MAUI controls (Labels, Grids, BoxViews, Borders) with theming and link handling.

```xml
<md:MarkdownView Markdown="{Binding Markdown}" Padding="16" />
```

### MarkdownView Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Markdown` | `string` | `""` | OneWay | The markdown content to render |
| `Theme` | `MarkdownTheme?` | `null` | OneWay | Rendering theme; auto-resolves Light/Dark based on app theme if null |
| `IsScrollEnabled` | `bool` | `true` | OneWay | Enable/disable scrolling of the content |

### MarkdownView Events

| Event | EventArgs | Description |
|---|---|---|
| `LinkTapped` | `LinkTappedEventArgs` | Fired when a link is tapped; set `Handled = true` to prevent default browser launch |

### LinkTappedEventArgs

| Property | Type | Description |
|---|---|---|
| `Url` | `string` | The URL of the tapped link |
| `Handled` | `bool` | Set to `true` to prevent default browser launch |

## MarkdownEditor

A markdown editor with formatting toolbar, live preview toggle, and customizable toolbar items.

```xml
<md:MarkdownEditor Markdown="{Binding Markdown, Mode=TwoWay}"
                   Placeholder="Start writing markdown..."
                   Padding="8" />
```

### MarkdownEditor Properties

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `Markdown` | `string` | `""` | TwoWay | The markdown content being edited |
| `Theme` | `MarkdownTheme?` | `null` | OneWay | Theme for preview rendering |
| `Placeholder` | `string` | `"Write markdown here..."` | OneWay | Placeholder text in the editor |
| `ToolbarItems` | `IReadOnlyList<MarkdownToolbarItem>?` | `MarkdownToolbarItems.Default` | OneWay | Formatting toolbar buttons |
| `IsPreviewVisible` | `bool` | `false` | TwoWay | Show/hide the preview pane |
| `ToolbarBackgroundColor` | `Color?` | `null` | OneWay | Toolbar background color |
| `EditorBackgroundColor` | `Color?` | `null` | OneWay | Editor text area background color |

### MarkdownEditor Events

| Event | EventArgs | Description |
|---|---|---|
| `LinkTapped` | `LinkTappedEventArgs` | Forwarded from preview link taps |
| `TextChanged` | `TextChangedEventArgs` | Fired when editor text changes |

### MarkdownEditor Features

- **Formatting toolbar**: Buttons for bold, italic, headings, lists, code, links, etc.
- **Live preview**: Toggle between edit and preview modes with eye/pencil icon button
- **Auto-growing editor**: Editor height grows as you type
- **Custom toolbar**: Replace default toolbar items with a custom set

## MarkdownTheme

Comprehensive theming for rendered markdown appearance. Auto-resolves Light or Dark based on `Application.Current?.RequestedTheme` when set to null.

### Static Themes

```csharp
MarkdownTheme.Light    // Light color scheme
MarkdownTheme.Dark     // Dark color scheme
```

### Theme Color Properties

| Property | Light Default | Dark Default | Description |
|---|---|---|---|
| `TextColor` | Black | `#E5E7EB` | Main text color |
| `MutedTextColor` | `#6B7280` | `#9CA3AF` | Dimmed text |
| `LinkColor` | `#2563EB` | `#60A5FA` | Hyperlink color |
| `CodeBackgroundColor` | `#F3F4F6` | `#374151` | Inline code background |
| `CodeTextColor` | `#D946EF` | `#F472B6` | Inline code text |
| `CodeBlockBackgroundColor` | `#1F2937` | `#111827` | Code block background |
| `CodeBlockTextColor` | `#E5E7EB` | `#D1D5DB` | Code block text |
| `BlockquoteBorderColor` | `#D1D5DB` | `#4B5563` | Blockquote left border |
| `BlockquoteBackgroundColor` | `#F9FAFB` | `#1F2937` | Blockquote background |
| `HorizontalRuleColor` | `#E5E7EB` | `#374151` | Divider color |
| `TableBorderColor` | `#E5E7EB` | `#374151` | Table cell borders |
| `TableHeaderBackgroundColor` | `#F3F4F6` | `#1F2937` | Table header background |

### Theme Font/Spacing Properties

| Property | Default | Description |
|---|---|---|
| `BaseFontSize` | `16` | Default text size |
| `H1FontSize` | `32` | Heading 1 size |
| `H2FontSize` | `24` | Heading 2 size |
| `H3FontSize` | `20` | Heading 3 size |
| `H4FontSize` | `18` | Heading 4 size |
| `H5FontSize` | `16` | Heading 5 size |
| `H6FontSize` | `14` | Heading 6 size |
| `CodeFontSize` | `14` | Code font size |
| `BlockSpacing` | `12` | Vertical spacing between block elements |
| `ListIndent` | `24` | Left indent for list items |
| `CodeFontFamily` | `""` | Monospace font family |

## MarkdownToolbarItem

```csharp
public record MarkdownToolbarItem(
    string Label,      // Display name
    string Icon,       // Emoji/text icon
    string Prefix,     // Text before selection
    string Suffix,     // Text after selection
    bool IsBlockLevel = false
);
```

### Pre-defined Toolbar Items

Available via `MarkdownToolbarItems.*`:

| Item | Icon | Description |
|---|---|---|
| `Bold` | 🗒 | **Bold text** |
| `Italic` | 𝘐 | *Italic text* |
| `Strikethrough` | S̶ | ~~Strikethrough~~ |
| `InlineCode` | `</>` | `inline code` |
| `Link` | 🔗 | Hyperlink |
| `Image` | 🖼 | Image |
| `H1` | H1 | Heading 1 |
| `H2` | H2 | Heading 2 |
| `H3` | H3 | Heading 3 |
| `BulletList` | • | Unordered list |
| `NumberedList` | 1. | Ordered list |
| `TaskList` | ☑ | Task list |
| `Quote` | " | Blockquote |
| `CodeBlock` | `{}` | Code block |
| `HorizontalRule` | — | Horizontal divider |

### Toolbar Collections

```csharp
MarkdownToolbarItems.Default  // Recommended set: Bold, Italic, InlineCode, H1-H3, Lists, Link, Quote, CodeBlock
MarkdownToolbarItems.All      // All 15 items
```

## Supported Markdown Elements

Bold, italic, strikethrough, H1-H6 headings, unordered/ordered/task lists, inline code, fenced code blocks, links (with LinkTapped event), images (as placeholder text), blockquotes, tables, horizontal rules, line breaks.

## Complete Markdown Example

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:md="http://shiny.net/maui/markdown"
             x:Class="MyApp.DocsPage"
             Title="Documentation">

    <md:MarkdownView Markdown="{Binding DocumentContent}" Padding="16" />
</ContentPage>
```

## Complete Markdown Editor Example

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:md="http://shiny.net/maui/markdown"
             x:Class="MyApp.NotesPage"
             Title="Notes">

    <md:MarkdownEditor Markdown="{Binding NoteContent, Mode=TwoWay}"
                       Placeholder="Write your notes..."
                       Padding="8" />
</ContentPage>
```

---

# Code Generation Instructions

When generating code with Shiny.Maui.Controls:

### 1. Page Structure
- Always add `xmlns:shiny="http://shiny.net/maui/controls"` to the page
- For Markdown controls: add `xmlns:md="http://shiny.net/maui/markdown"` to the page
- For TableView: wrap content in `shiny:TableView > shiny:TableRoot > shiny:TableSection`
- For BottomSheet: place `shiny:BottomSheetView` inside a Grid that fills the page (it overlays on top)
- For ImageViewer: place `shiny:ImageViewer` inside a Grid that fills the page (it overlays on top, same pattern as BottomSheet)
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

### 4. BottomSheet Placement
The BottomSheetView must be placed inside a Grid that fills the page so it can overlay correctly:

```xml
<ContentPage>
    <Grid>
        <!-- Main page content -->
        <ScrollView>
            <VerticalStackLayout>
                <Button Text="Open Sheet" Command="{Binding OpenCommand}" />
            </VerticalStackLayout>
        </ScrollView>

        <!-- Bottom sheet overlays on top -->
        <shiny:BottomSheetView IsOpen="{Binding IsOpen, Mode=TwoWay}">
            <Label Text="Sheet content" />
        </shiny:BottomSheetView>
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

## Complete BottomSheet + PillView Example

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:shiny="http://shiny.net/maui/controls"
             x:Class="MyApp.StatusPage"
             Title="Status">

    <Grid>
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

        <shiny:BottomSheetView IsOpen="{Binding IsDetailsOpen, Mode=TwoWay}"
                               SheetCornerRadius="20">
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
        </shiny:BottomSheetView>
    </Grid>
</ContentPage>
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
11. **Place BottomSheetView and ImageViewer in a Grid** - They must overlay page content, so use a Grid as the page root
12. **Use PillView for status indicators** - Prefer preset types for consistency; use custom colors for brand-specific needs
13. **Use AOT-safe bindings for scheduler templates** - Always use `static (T item) => item.Property` lambda bindings, never string-based
14. **Leave MarkdownView/MarkdownEditor Theme as null** - It auto-resolves Light/Dark based on the app theme
15. **Use MarkdownView for read-only content** - Documentation, notes, changelogs; use MarkdownEditor only when the user needs to edit
16. **ImageViewer Source is set before IsOpen** - Always set the image source before opening the viewer
14. **Register scheduler pages and ViewModels in DI** - Use `AddTransient` for pages and ViewModels, `AddSingleton` for the event provider
15. **Use `SelectedDate` with TwoWay binding** - All scheduler views share this property for coordination
16. **Implement all `ISchedulerEventProvider` methods** - Even if some are no-ops, all must be implemented

---

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
builder.Services.AddTransient<MySchedulerViewModel>();
builder.Services.AddTransient<MyCalendarPage>();
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
- `SchedulerAgendaView` uses an Apple Calendar-style day picker by default; override with `DayPickerItemTemplate` using `DatePickerItemContext`
- `SchedulerAgendaView` supports 24-hour and 12-hour (AM/PM) time via `Use24HourTime`
- `SchedulerAgendaView` supports multiple timezone columns via `AdditionalTimezones` + `ShowAdditionalTimezones`
- All-day events always sort to top within their day group
- Custom templates must use AOT-safe static lambda bindings, never string-based bindings
- `AdditionalTimezones` is an `IList<TimeZoneInfo>` — add timezones in code-behind
