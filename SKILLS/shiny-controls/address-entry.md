# AddressEntry

An address search control built on `AutoCompleteEntry`. Queries a geocoding provider (Nominatim/OpenStreetMap by default) and returns structured address data with coordinates. Supports custom providers via `IAddressSearchProvider`.

## MAUI Usage

```xml
<shiny:AddressEntry SelectedAddress="{Binding Address}"
                    Placeholder="Search address..."
                    CountryCodes="us,ca"
                    FontSize="16"
                    CornerRadius="8" />
```

## Blazor Usage

```razor
<AddressEntry SelectedAddress="@selectedAddress"
              SelectedAddressChanged="OnAddressChanged"
              Placeholder="Search address..."
              CountryCodes="us,ca"
              FontSize="16"
              TextColor="#333" />

@code {
    Address? selectedAddress;

    void OnAddressChanged(Address? address)
    {
        selectedAddress = address;
    }
}
```

## Custom Search Provider

```csharp
public class GoogleGeoProvider : IAddressSearchProvider
{
    public async Task<IList<Address>> SearchAsync(string query, string? countryCodes, CancellationToken ct)
    {
        // Call Google Geocoding API and map results to Address records
        return results;
    }
}
```

MAUI:
```xml
<shiny:AddressEntry SearchProvider="{Binding MyGeoProvider}" />
```

Blazor:
```razor
<AddressEntry SearchProvider="myGeoProvider" />
```

## AddressEntry Properties

| Property | Type | Default | Host | Description |
|---|---|---|---|---|
| `SelectedAddress` | `Address` | `null` | Both | Selected address (TwoWay) |
| `SearchProvider` | `IAddressSearchProvider?` | `null` | Both | Custom geocoding provider |
| `CountryCodes` | `string?` | `null` | Both | Comma-separated ISO codes to filter |
| `Placeholder` | `string` | `"Search address..."` | Both | Placeholder text |
| `MaxDropDownHeight` | `double` | `250` | Both | Max dropdown height |
| `TextColor` | Color/string | `null` | Both | Text color |
| `PlaceholderColor` | Color/string | `null` | Both | Placeholder color |
| `DropDownBackgroundColor` | Color/string | `null` | Both | Dropdown background |
| `DropDownBorderColor` | Color/string | `null` | Both | Dropdown border |
| `FontSize` | `double` | `14` | Both | Font size |
| `FontFamily` | `string?` | `null` | MAUI | Font family |
| `CornerRadius` | `double` | `4` | MAUI | Dropdown corner radius |
| `InputClass` | `string?` | `null` | Blazor | Input CSS class |
| `DropDownClass` | `string?` | `null` | Blazor | Dropdown CSS class |

Events: `AddressSelected` (MAUI: `EventHandler<Address>`, Blazor: `EventCallback<Address>`)

### Address Model

| Property | Type | Description |
|---|---|---|
| `DisplayName` | `string` | Full formatted address |
| `HouseNumber` | `string?` | House/building number |
| `Street` | `string?` | Street name |
| `City` | `string?` | City/town/village |
| `State` | `string?` | State/province |
| `PostalCode` | `string?` | Postal/ZIP code |
| `Country` | `string?` | Country name |
| `CountryCode` | `string?` | ISO country code |
| `Latitude` | `double` | Latitude coordinate |
| `Longitude` | `double` | Longitude coordinate |

### IAddressSearchProvider

```csharp
public interface IAddressSearchProvider
{
    Task<IList<Address>> SearchAsync(string query, string? countryCodes, CancellationToken ct);
}
```

The default provider (`NominatimAddressSearchProvider`) queries OpenStreetMap's Nominatim API with a User-Agent header. For production use, consider a provider with higher rate limits.
