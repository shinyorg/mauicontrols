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
| `CellFocusedBackgroundColor` | `Color?` | `null` | OneWay | Background color for the currently active cell |
| `CellTextColor` | `Color?` | `null` | OneWay | Color for the rendered digit/character |
| `FontSize` | `double` | `24` | OneWay | Font size for the rendered character |
| `UseHapticFeedback` | `bool` | `true` | OneWay | Haptic click on digit entry, long press on completion |

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
                   CellFocusedBackgroundColor="#E0EDFF"
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
