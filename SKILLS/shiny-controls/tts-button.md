# TextToSpeechButton

A button that speaks bound text using the platform's text-to-speech engine. Fully customizable visually like a normal button, but instead of a `Command` binding, tapping plays the bound `SpeechText`. Tapping again while speaking cancels playback.

Available on both MAUI (native TTS via `TextToSpeech.Default`) and Blazor (browser Web Speech API via JS interop).

## Basic Usage

```xml
<!-- Simple text-to-speech button -->
<shiny:TextToSpeechButton Text="Listen"
                          SpeechText="{Binding ArticleText}"
                          ButtonBackgroundColor="#2196F3"
                          TextColor="White" />

<!-- With icon -->
<shiny:TextToSpeechButton Text="Play"
                          Icon="speaker.png"
                          SpeechText="{Binding SomeText}"
                          ButtonBackgroundColor="#4CAF50"
                          TextColor="White" />

<!-- Outlined style -->
<shiny:TextToSpeechButton Text="Speak"
                          SpeechText="Hello World"
                          ButtonBackgroundColor="Transparent"
                          BorderColor="#E91E63"
                          BorderThickness="2"
                          TextColor="#E91E63"
                          CornerRadius="20" />

<!-- With speech options -->
<shiny:TextToSpeechButton Text="Slow and deep"
                          SpeechText="This is spoken slowly with a low pitch."
                          Pitch="0.5"
                          Volume="1.0"
                          ButtonBackgroundColor="#FF9800"
                          TextColor="White" />
```

### Blazor Usage

```razor
<TextToSpeechButton Text="Listen"
                    SpeechText="@articleText"
                    ButtonBackgroundColor="#2196F3" />

<!-- With two-way IsSpeaking binding -->
<TextToSpeechButton Text="@(isSpeaking ? "Stop" : "Listen")"
                    SpeechText="@longText"
                    @bind-IsSpeaking="isSpeaking"
                    ButtonBackgroundColor="@(isSpeaking ? "#F44336" : "#2196F3")" />

<!-- With speech options -->
<TextToSpeechButton Text="Custom voice"
                    SpeechText="@text"
                    Pitch="0.8f"
                    Rate="1.2f"
                    Volume="0.7f"
                    Locale="en-GB" />
```

## TextToSpeechButton Properties (MAUI)

| Property | Type | Default | Binding Mode | Description |
|---|---|---|---|---|
| `SpeechText` | `string?` | `null` | OneWay | The text to speak when tapped |
| `Text` | `string?` | `null` | OneWay | Button label |
| `Icon` | `ImageSource?` | `null` | OneWay | Button icon |
| `ButtonBackgroundColor` | `Color` | `#2196F3` | OneWay | Fill color |
| `TextColor` | `Color` | `White` | OneWay | Label text color |
| `FontSize` | `double` | `14` | OneWay | Label font size |
| `FontAttributes` | `FontAttributes` | `None` | OneWay | Label font attributes |
| `CornerRadius` | `double` | `8` | OneWay | Border corner radius |
| `BorderColor` | `Color?` | `null` | OneWay | Outline stroke color |
| `BorderThickness` | `double` | `0` | OneWay | Outline stroke thickness |
| `IconSize` | `double` | `24` | OneWay | Icon dimensions |
| `IsSpeaking` | `bool` | `false` | OneWayToSource | Whether speech is currently playing |
| `Pitch` | `float` | `1.0` | OneWay | Speech pitch (0.0-2.0) |
| `Volume` | `float` | `1.0` | OneWay | Speech volume (0.0-1.0) |
| `Locale` | `Locale?` | `null` | OneWay | Speech locale/language |
| `UseHapticFeedback` | `bool` | `true` | OneWay | Haptic feedback on tap |
| `HasShadow` | `bool` | `false` | OneWay | Drop shadow on/off |

## TextToSpeechButton Properties (Blazor)

| Parameter | Type | Default | Description |
|---|---|---|---|
| `SpeechText` | `string?` | `null` | The text to speak when tapped |
| `Text` | `string?` | `null` | Button label |
| `Icon` | `string?` | `null` | Inline SVG or image URL |
| `ButtonBackgroundColor` | `string` | `"#2196F3"` | CSS background color |
| `TextColor` | `string` | `"#FFFFFF"` | CSS text color |
| `FontSize` | `double` | `14` | Font size in pixels |
| `CornerRadius` | `double` | `8` | Border radius in pixels |
| `BorderColor` | `string?` | `null` | CSS border color |
| `BorderThickness` | `double` | `0` | Border width in pixels |
| `IconSize` | `double` | `24` | Icon dimensions in pixels |
| `IsSpeaking` | `bool` | `false` | Two-way bindable via `@bind-IsSpeaking` |
| `Pitch` | `float` | `1.0` | Speech pitch |
| `Rate` | `float` | `1.0` | Speech rate (Blazor-specific) |
| `Volume` | `float` | `1.0` | Speech volume |
| `Locale` | `string?` | `null` | BCP-47 language tag (e.g., `"en-GB"`) |
| `HasShadow` | `bool` | `false` | Drop shadow on/off |
| `Disabled` | `bool` | `false` | Disable button interactions |

## Events

| Event | Args | Description |
|---|---|---|
| `Clicked` | `EventArgs` (MAUI) / `EventCallback` (Blazor) | Fires on every tap, before speak/cancel |

## Methods (MAUI only)

| Method | Description |
|---|---|
| `Cancel()` | Programmatically stop speech playback |

## Behavior

- Tapping the button while not speaking starts speaking the `SpeechText`
- Tapping again while speaking cancels the current speech
- `IsSpeaking` reflects the current state — bind to it for dynamic UI (e.g., changing button text/color)
- If `SpeechText` is null or empty, tapping does nothing (but `Clicked` still fires)
- The MAUI version uses `TextToSpeech.Default.SpeakAsync` (native platform TTS)
- The Blazor version uses the browser `SpeechSynthesis` API via JS interop
- Disposing the Blazor component automatically cancels any in-progress speech

## Code Generation Guidance

- Use `TextToSpeechButton` whenever the user wants a button that reads text aloud — do not manually wire `TextToSpeech.Default` to a regular Button
- Bind `SpeechText` to any string property — article content, label text, accessibility descriptions, etc.
- Use `IsSpeaking` to change the button appearance while speaking (e.g., swap text to "Stop", change color to red)
- Default speech options (Pitch=1, Volume=1) are fine for most use cases; only expose sliders when the user explicitly wants control
- In Blazor, use `@bind-IsSpeaking` for two-way binding; in MAUI, `IsSpeaking` is OneWayToSource (read from ViewModel, not set)
- The control handles cancellation automatically — no need to track CancellationTokenSource in ViewModels

## ViewModel Pattern (MAUI)

```csharp
public partial class ArticleViewModel : ObservableObject
{
    [ObservableProperty]
    string articleText = "Welcome to Shiny Controls. This text will be spoken aloud.";

    [ObservableProperty]
    bool isSpeaking;
}
```

```xml
<shiny:TextToSpeechButton Text="{Binding IsSpeaking, Converter={StaticResource BoolToTextConverter}}"
                          SpeechText="{Binding ArticleText}"
                          IsSpeaking="{Binding IsSpeaking}"
                          ButtonBackgroundColor="#2196F3"
                          TextColor="White" />
```

## Blazor Pattern

```razor
<TextToSpeechButton Text="@(isSpeaking ? "Stop" : "Listen")"
                    SpeechText="@article"
                    @bind-IsSpeaking="isSpeaking"
                    ButtonBackgroundColor="@(isSpeaking ? "#F44336" : "#2196F3")" />

@code {
    string article = "Welcome to Shiny Controls.";
    bool isSpeaking;
}
```
