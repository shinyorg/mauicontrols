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

Place the `Fab` / `FabMenu` inside a `Grid` that fills the page — same pattern as `ImageViewer`:

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
| `UseFeedback` | `bool` | `true` | OneWay | Feedback on tap |

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
| `UseFeedback` | `bool` | `true` | OneWay | Feedback on toggle |

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
| `UseFeedback` | `bool` | `true` | Feedback on tap |

Events: `Clicked`.

## Behavior & Animation

- Tapping the main Fab of a `FabMenu` toggles `IsOpen`
- Opening the menu fades in the backdrop and animates each `FabMenuItem` up (fade + translate) with a small stagger; closing reverses it
- `IsOpen` is fully two-way bindable — setting it from a ViewModel animates in/out
- Child items' own animations never conflict with the main Fab — it stays fixed
- Tapping a menu item executes its `Command`, raises `ItemTapped` on the menu, and closes the menu when `CloseOnItemTap` is true (default)

## Code Generation Guidance

- Use `Fab` for a single primary action (e.g., "Add") and `FabMenu` for multiple related actions (speed dial)
- Always place `Fab` / `FabMenu` inside a Grid that fills the page so the FabMenu backdrop overlays everything (or use `ShinyContentPage` with `OverlayHost`)
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
