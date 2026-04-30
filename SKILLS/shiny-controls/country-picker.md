# CountryPicker

A country search control built on `AutoCompleteEntry`. Displays flag emoji, country name, and dial code in the dropdown. Wraps the full ISO 3166-1 country list. All styling properties are forwarded to the inner `AutoCompleteEntry`.

## MAUI Usage

```xml
<shiny:CountryPicker SelectedCountry="{Binding Country}"
                     Placeholder="Select country..."
                     FontSize="16"
                     TextColor="Black"
                     CornerRadius="8" />
```

## Blazor Usage

```razor
<CountryPicker SelectedCountry="@selectedCountry"
               SelectedCountryChanged="OnCountryChanged"
               Placeholder="Select country..."
               FontSize="16"
               TextColor="#333"
               InputClass="my-input" />

@code {
    Country? selectedCountry;

    void OnCountryChanged(Country? country)
    {
        selectedCountry = country;
    }
}
```

## CountryPicker Properties

| Property | Type | Default | Host | Description |
|---|---|---|---|---|
| `SelectedCountry` | `Country` | `null` | Both | Selected country (TwoWay) |
| `Placeholder` | `string` | `"Search countries..."` | Both | Placeholder text |
| `MaxDropDownHeight` | `double` | `200` | Both | Max dropdown height |
| `TextColor` | Color/string | `null` | Both | Text color |
| `PlaceholderColor` | Color/string | `null` | Both | Placeholder color |
| `DropDownBackgroundColor` | Color/string | `null` | Both | Dropdown background |
| `DropDownBorderColor` | Color/string | `null` | Both | Dropdown border |
| `FontSize` | `double` | `14` | Both | Font size |
| `FontFamily` | `string?` | `null` | MAUI | Font family |
| `CornerRadius` | `double` | `4` | MAUI | Dropdown corner radius |
| `InputClass` | `string?` | `null` | Blazor | Input CSS class |
| `DropDownClass` | `string?` | `null` | Blazor | Dropdown CSS class |

Events: `CountrySelected` (MAUI: `EventHandler<Country>`, Blazor: `EventCallback<Country>`)

### Country Model

| Property | Type | Description |
|---|---|---|
| `Name` | `string` | Country name (e.g. "United States") |
| `Iso2` | `string` | ISO 3166-1 alpha-2 code (e.g. "US") |
| `Iso3` | `string` | ISO 3166-1 alpha-3 code (e.g. "USA") |
| `DialCode` | `string` | International dial code (e.g. "+1") |
| `FlagEmoji` | `string` | Flag emoji (e.g. "🇺🇸") |

Flag emoji display is automatically disabled on Windows (not supported by the platform font).
