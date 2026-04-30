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

## PillView Style Overrides

Each `PillType` maps to a well-known style key. If the application's `ResourceDictionary` contains a `Style` with the matching key and `TargetType="PillView"`, it is applied instead of the built-in defaults.

| PillType | Style Key |
|---|---|
| `None` | `ShinyPillNoneStyle` |
| `Success` | `ShinyPillSuccessStyle` |
| `Info` | `ShinyPillInfoStyle` |
| `Warning` | `ShinyPillWarningStyle` |
| `Caution` | `ShinyPillCautionStyle` |
| `Critical` | `ShinyPillCriticalStyle` |

Example override in `App.xaml`:

```xml
<Application.Resources>
    <Style x:Key="ShinyPillSuccessStyle" TargetType="shiny:PillView">
        <Setter Property="PillColor" Value="#22C55E" />
        <Setter Property="PillTextColor" Value="White" />
        <Setter Property="PillBorderColor" Value="#16A34A" />
    </Style>
</Application.Resources>
```
