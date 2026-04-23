using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Shiny.Blazor.Controls;

public record Address(
    string DisplayName,
    string? HouseNumber,
    string? Street,
    string? City,
    string? State,
    string? PostalCode,
    string? Country,
    string? CountryCode,
    double Latitude,
    double Longitude
)
{
    public string ShortDisplay => string.Join(", ", new[] { HouseNumber, Street, City, State }.Where(s => !string.IsNullOrWhiteSpace(s)));
    public override string ToString() => DisplayName;
}

public interface IAddressSearchProvider
{
    Task<IList<Address>> SearchAsync(string query, string? countryCodes, CancellationToken ct);
}

public class NominatimAddressSearchProvider : IAddressSearchProvider
{
    readonly HttpClient httpClient;

    public NominatimAddressSearchProvider(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IList<Address>> SearchAsync(string query, string? countryCodes, CancellationToken ct)
    {
        var encoded = Uri.EscapeDataString(query);
        var url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=jsonv2&addressdetails=1&limit=5";

        if (!string.IsNullOrWhiteSpace(countryCodes))
            url += $"&countrycodes={Uri.EscapeDataString(countryCodes)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("User-Agent", "Shiny.Blazor.Controls/1.0");
        request.Headers.Add("Accept-Language", "en");

        var response = await httpClient.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var results = await response.Content.ReadFromJsonAsync(
            BlazorNominatimJsonContext.Default.ListNominatimResult, ct);

        return results?
            .Select(r => new Address(
                r.DisplayName,
                r.Address?.HouseNumber,
                r.Address?.Road,
                r.Address?.City ?? r.Address?.Town ?? r.Address?.Village,
                r.Address?.State,
                r.Address?.Postcode,
                r.Address?.Country,
                r.Address?.CountryCode,
                double.TryParse(r.Lat, System.Globalization.CultureInfo.InvariantCulture, out var lat) ? lat : 0,
                double.TryParse(r.Lon, System.Globalization.CultureInfo.InvariantCulture, out var lon) ? lon : 0
            ))
            .ToList<Address>() ?? [];
    }
}

[JsonSerializable(typeof(List<NominatimResult>))]
internal partial class BlazorNominatimJsonContext : JsonSerializerContext;

internal class NominatimResult
{
    [JsonPropertyName("display_name")]
    public string DisplayName { get; set; } = "";

    [JsonPropertyName("lat")]
    public string? Lat { get; set; }

    [JsonPropertyName("lon")]
    public string? Lon { get; set; }

    [JsonPropertyName("address")]
    public NominatimAddress? Address { get; set; }
}

internal class NominatimAddress
{
    [JsonPropertyName("house_number")]
    public string? HouseNumber { get; set; }

    [JsonPropertyName("road")]
    public string? Road { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("town")]
    public string? Town { get; set; }

    [JsonPropertyName("village")]
    public string? Village { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("postcode")]
    public string? Postcode { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }
}
