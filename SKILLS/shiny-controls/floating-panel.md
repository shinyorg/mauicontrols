# FloatingPanel + OverlayHost (MAUI)

A floating panel overlay system for .NET MAUI. Panels slide in from the bottom or top edge of the page with configurable detents, header peek when closed, backdrop dimming, drag-handle gestures, keyboard handling, and haptic feedback. Multiple panels can coexist on the same page without blocking touches on content underneath.

**Architecture:**
- **OverlayHost**: A transparent `Grid` layer with `InputTransparent=true, CascadeInputTransparent=false` — touches pass through to content underneath, but panels and backdrop still receive input on their visible areas. Manages a shared backdrop.
- **FloatingPanel**: A `ContentView` that lives inside an `OverlayHost`. Animates height (not translation) so the panel only occupies the space it needs. Pan gesture is restricted to the drag handle only — no scroll conflicts with content.
- **ShinyContentPage**: A `ContentPage` with a built-in `OverlayHost`. Set page content via `PageContent` and add panels via `Panels`.

> **Blazor note:** Blazor does not use FloatingPanel/OverlayHost. It retains `SheetView` which uses CSS `position: fixed`, `z-index`, and `pointer-events: none` for overlay behavior natively.

## Basic Usage with ShinyContentPage

```xml
<shiny:ShinyContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
                         xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
                         xmlns:shiny="http://shiny.net/maui/controls"
                         x:Class="MyApp.MainPage">
    <shiny:ShinyContentPage.PageContent>
        <ScrollView>
            <VerticalStackLayout Padding="20" Spacing="10">
                <Button Text="Open Panel" Command="{Binding OpenCommand}" />
            </VerticalStackLayout>
        </ScrollView>
    </shiny:ShinyContentPage.PageContent>
    <shiny:ShinyContentPage.Panels>
        <shiny:FloatingPanel IsOpen="{Binding IsOpen, Mode=TwoWay}">
            <VerticalStackLayout Padding="20" Spacing="10">
                <Label Text="Panel Content" FontSize="18" FontAttributes="Bold" />
                <Button Text="Close" Command="{Binding CloseCommand}" />
            </VerticalStackLayout>
        </shiny:FloatingPanel>
    </shiny:ShinyContentPage.Panels>
</shiny:ShinyContentPage>
```

## Basic Usage with OverlayHost (manual)

```xml
<ContentPage>
    <Grid>
        <ScrollView>
            <!-- page content -->
        </ScrollView>

        <shiny:OverlayHost>
            <shiny:FloatingPanel IsOpen="{Binding IsOpen, Mode=TwoWay}">
                <!-- panel content -->
            </shiny:FloatingPanel>
        </shiny:OverlayHost>
    </Grid>
</ContentPage>
```

## FloatingPanel Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `IsOpen` | `bool` | `false` | Opens/closes the panel (two-way bindable) |
| `Position` | `FloatingPanelPosition` | `Bottom` | Where the panel slides from (`Bottom`, `BottomTabs`, or `Top`). Use `BottomTabs` when inside a Shell TabBar to clip the panel above the tab bar |
| `PanelContent` | `View?` | `null` | The content displayed inside the panel (`[ContentProperty]`) |
| `HeaderTemplate` | `View?` | `null` | Optional header view at the screen edge; shown as a peek bar when closed |
| `ShowHeaderWhenClosed` | `bool` | `false` | When true, the header peeks from the edge when the panel is closed |
| `Detents` | `ObservableCollection<DetentValue>` | Quarter, Half, Full | Snap points as ratios of available height |
| `PanelBackgroundColor` | `Color` | `White` | Background color of the panel |
| `HandleColor` | `Color` | `Grey` | Color of the drag handle indicator |
| `ShowHandle` | `bool` | `true` | Show/hide the drag handle bar |
| `PanelCornerRadius` | `double` | `16` | Corner radius of the panel |
| `HasBackdrop` | `bool` | `true` | Shows a dimming backdrop behind the panel |
| `CloseOnBackdropTap` | `bool` | `true` | Tapping the backdrop closes the panel |
| `AnimationDuration` | `double` | `250` | Animation duration in milliseconds |
| `ExpandOnInputFocus` | `bool` | `true` | Auto-expands to highest detent when an input is focused |
| `IsLocked` | `bool` | `false` | Prevents all user dismissal (drag, header tap close, backdrop tap); panel can only be closed via code. Header tap still opens the panel |
| `FitContent` | `bool` | `false` | Measures content and auto-computes a single detent to fit it (ignores Detents when true) |
| `UseHapticFeedback` | `bool` | `true` | Haptic feedback on open, close, and detent snap |

## OverlayHost Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `BackdropColor` | `Color` | `Black` | Backdrop color |
| `BackdropMaxOpacity` | `double` | `0.5` | Maximum backdrop opacity |

## ShinyContentPage Properties

| Property | Type | Default | Description |
|---|---|---|---|
| `PageContent` | `View?` | `null` | Main page content (`[ContentProperty]`) |
| `Panels` | `IList<IView>` | — | Collection of FloatingPanels |
| `OverlayHost` | `OverlayHost` | — | The internal OverlayHost (read-only) |
| `BackdropColor` | `Color` | `Black` | Forwarded to internal OverlayHost |
| `BackdropMaxOpacity` | `double` | `0.5` | Forwarded to internal OverlayHost |

## DetentValue

Predefined snap points (or create custom ones):

| Static Property | Ratio | Description |
|---|---|---|
| `DetentValue.Quarter` | `0.25` | 25% of available height |
| `DetentValue.Half` | `0.50` | 50% of available height |
| `DetentValue.ThreeQuarters` | `0.75` | 75% of available height |
| `DetentValue.Full` | `1.0` | Full available height |

Custom detent: `new DetentValue(0.33)` for 33% height.

## FloatingPanel Events

| Event | Args | Description |
|---|---|---|
| `Opened` | `EventArgs` | Panel finished opening animation |
| `Closed` | `EventArgs` | Panel finished closing animation |
| `DetentChanged` | `DetentValue` | Panel snapped to a different detent |

## Public Methods

| Method | Description |
|---|---|
| `AnimateToDetentAsync(DetentValue)` | Programmatically animate to a specific detent |

## FloatingPanel Features

- **Drag handle gesture**: Pan gesture restricted to the drag handle only — content scrolls normally with no gesture conflicts
- **Height animation**: Panel animates `HeightRequest` instead of `TranslationY`, so it only occupies the space it needs — no phantom touch areas
- **Keyboard handling**: Automatically expands when an Entry/Editor is focused, restores when keyboard dismissed
- **Backdrop**: Shared backdrop managed by OverlayHost; dims proportionally to panel position
- **Multiple panels**: Multiple FloatingPanels can coexist in the same OverlayHost without blocking each other or content underneath
- **Locked mode**: When `IsLocked="True"`, all user-initiated dismissal is blocked (drag, header tap to close, backdrop tap to close) — the panel can only be closed via code. Header tap still opens the panel
- **Fit content**: When `FitContent="True"`, the panel measures its content and auto-computes a single detent to fit it
- **Position**: Slides from bottom (`Position="Bottom"`, default), top (`Position="Top"`), or bottom with tabs (`Position="BottomTabs"` — clips above the tab bar)
- **Header peek**: Set `ShowHeaderWhenClosed="True"` with a `HeaderTemplate` to show a persistent header bar when the panel is closed — tapping it opens the panel
- **Haptic feedback**: Subtle haptic on open, close, and detent snap; disable with `UseHapticFeedback="False"`

## FloatingPanel Locked Example

```xml
<shiny:ShinyContentPage>
    <shiny:ShinyContentPage.PageContent>
        <!-- page content -->
    </shiny:ShinyContentPage.PageContent>
    <shiny:ShinyContentPage.Panels>
        <!-- Signature capture: locked + auto-sized -->
        <shiny:FloatingPanel IsOpen="{Binding IsSignatureOpen}"
                             IsLocked="True"
                             FitContent="True"
                             HasBackdrop="True"
                             PanelCornerRadius="20">
            <VerticalStackLayout Padding="20" Spacing="15">
                <Label Text="Draw your signature" FontSize="18" FontAttributes="Bold" />
                <Button Text="Done" Command="{Binding DoneCommand}" />
            </VerticalStackLayout>
        </shiny:FloatingPanel>

        <!-- Selector: locked with explicit detent -->
        <shiny:FloatingPanel IsOpen="{Binding IsSelectorOpen}"
                             IsLocked="True"
                             HasBackdrop="True"
                             PanelCornerRadius="20">
            <shiny:FloatingPanel.Detents>
                <shiny:DetentValue Ratio="0.5" />
            </shiny:FloatingPanel.Detents>
            <CollectionView ItemsSource="{Binding Items}" />
        </shiny:FloatingPanel>
    </shiny:ShinyContentPage.Panels>
</shiny:ShinyContentPage>
```

## FloatingPanel with Custom Detents

```xml
<shiny:FloatingPanel IsOpen="{Binding IsOpen, Mode=TwoWay}"
                     PanelBackgroundColor="#1E1E1E"
                     HandleColor="#888888"
                     PanelCornerRadius="24"
                     HasBackdrop="True"
                     CloseOnBackdropTap="True"
                     AnimationDuration="300">
    <shiny:FloatingPanel.Detents>
        <shiny:DetentValue Ratio="0.33" />
        <shiny:DetentValue Ratio="0.66" />
        <shiny:DetentValue Ratio="1.0" />
    </shiny:FloatingPanel.Detents>
    <VerticalStackLayout Padding="20">
        <Label Text="Custom Panel" TextColor="White" />
    </VerticalStackLayout>
</shiny:FloatingPanel>
```
