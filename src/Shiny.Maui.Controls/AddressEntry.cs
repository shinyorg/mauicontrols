using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Shiny.Maui.Controls;

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
    static readonly Lazy<HttpClient> lazyHttpClient = new(() =>
    {
        var client = new HttpClient();
        try
        {
            var name = AppInfo.PackageName;
            var version = AppInfo.Version;
            client.DefaultRequestHeaders.Add("User-Agent", $"{name}/{version}");
        }
        catch
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Shiny.Maui.Controls/1.0");
        }
        client.DefaultRequestHeaders.Add("Accept-Language", "en");
        return client;
    });
    static HttpClient httpClient => lazyHttpClient.Value;

    public async Task<IList<Address>> SearchAsync(string query, string? countryCodes, CancellationToken ct)
    {
        var encoded = Uri.EscapeDataString(query);
        var url = $"https://nominatim.openstreetmap.org/search?q={encoded}&format=jsonv2&addressdetails=1&limit=5";

        if (!string.IsNullOrWhiteSpace(countryCodes))
            url += $"&countrycodes={Uri.EscapeDataString(countryCodes)}";

        var results = await httpClient.GetFromJsonAsync(url, NominatimJsonContext.Default.ListNominatimResult, ct);

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

public class AddressEntry : ContentView
{
    static readonly IAddressSearchProvider defaultProvider = new NominatimAddressSearchProvider();

    readonly AutoCompleteEntry autoComplete;
    CancellationTokenSource? searchCts;

    public AddressEntry()
    {
        autoComplete = new AutoCompleteEntry
        {
            Placeholder = "Search address...",
            TextMemberPath = nameof(Address.DisplayName),
            DebounceInterval = 400,
            Threshold = 3,
            MaxDropDownHeight = 250,
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    FontSize = 13,
                    LineBreakMode = LineBreakMode.TailTruncation,
                    MaxLines = 1,
                    Padding = new Thickness(8, 6)
                };
                label.SetBinding(Label.TextProperty, nameof(Address.ShortDisplay));
                return label;
            }),
            HorizontalOptions = LayoutOptions.Fill
        };
        autoComplete.SetBinding(
            AutoCompleteEntry.SearchCommandProperty,
            new Binding(nameof(InternalSearchCommand), source: this)
        );
        autoComplete.ItemSelected += OnItemSelected;

        Content = autoComplete;
    }

    public Command<string> InternalSearchCommand => new(async searchText => await SearchAsync(searchText));

    public static readonly BindableProperty SelectedAddressProperty = BindableProperty.Create(
        nameof(SelectedAddress),
        typeof(Address),
        typeof(AddressEntry),
        null,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((AddressEntry)b).OnSelectedAddressChanged(n as Address));
    public Address? SelectedAddress
    {
        get => (Address?)GetValue(SelectedAddressProperty);
        set => SetValue(SelectedAddressProperty, value);
    }

    public static readonly BindableProperty SearchProviderProperty = BindableProperty.Create(
        nameof(SearchProvider),
        typeof(IAddressSearchProvider),
        typeof(AddressEntry),
        null);
    public IAddressSearchProvider? SearchProvider
    {
        get => (IAddressSearchProvider?)GetValue(SearchProviderProperty);
        set => SetValue(SearchProviderProperty, value);
    }

    public static readonly BindableProperty CountryCodesProperty = BindableProperty.Create(
        nameof(CountryCodes),
        typeof(string),
        typeof(AddressEntry),
        null);
    public string? CountryCodes
    {
        get => (string?)GetValue(CountryCodesProperty);
        set => SetValue(CountryCodesProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(AddressEntry),
        "Search address...",
        propertyChanged: (b, _, n) => ((AddressEntry)b).autoComplete.Placeholder = (string)n);
    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(AddressEntry),
        null,
        propertyChanged: (b, _, n) => ((AddressEntry)b).autoComplete.TextColor = n as Color);
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor),
        typeof(Color),
        typeof(AddressEntry),
        null,
        propertyChanged: (b, _, n) => ((AddressEntry)b).autoComplete.PlaceholderColor = n as Color);
    public Color? PlaceholderColor
    {
        get => (Color?)GetValue(PlaceholderColorProperty);
        set => SetValue(PlaceholderColorProperty, value);
    }

    public static readonly BindableProperty DropDownBackgroundColorProperty = BindableProperty.Create(
        nameof(DropDownBackgroundColor),
        typeof(Color),
        typeof(AddressEntry),
        null,
        propertyChanged: (b, _, n) => { if (n is Color c) ((AddressEntry)b).autoComplete.DropDownBackgroundColor = c; });
    public Color? DropDownBackgroundColor
    {
        get => (Color?)GetValue(DropDownBackgroundColorProperty);
        set => SetValue(DropDownBackgroundColorProperty, value);
    }

    public static readonly BindableProperty DropDownBorderColorProperty = BindableProperty.Create(
        nameof(DropDownBorderColor),
        typeof(Color),
        typeof(AddressEntry),
        null,
        propertyChanged: (b, _, n) => { if (n is Color c) ((AddressEntry)b).autoComplete.DropDownBorderColor = c; });
    public Color? DropDownBorderColor
    {
        get => (Color?)GetValue(DropDownBorderColorProperty);
        set => SetValue(DropDownBorderColorProperty, value);
    }

    public static readonly BindableProperty SpinnerColorProperty = BindableProperty.Create(
        nameof(SpinnerColor),
        typeof(Color),
        typeof(AddressEntry),
        null,
        propertyChanged: (b, _, n) => { if (n is Color c) ((AddressEntry)b).autoComplete.SpinnerColor = c; });
    public Color? SpinnerColor
    {
        get => (Color?)GetValue(SpinnerColorProperty);
        set => SetValue(SpinnerColorProperty, value);
    }

    public static readonly BindableProperty MaxDropDownHeightProperty = BindableProperty.Create(
        nameof(MaxDropDownHeight),
        typeof(double),
        typeof(AddressEntry),
        250d,
        propertyChanged: (b, _, n) => ((AddressEntry)b).autoComplete.MaxDropDownHeight = (double)n);
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(AddressEntry),
        14d,
        propertyChanged: (b, _, n) => ((AddressEntry)b).autoComplete.FontSize = (double)n);
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(AddressEntry),
        null,
        propertyChanged: (b, _, n) => ((AddressEntry)b).autoComplete.FontFamily = n as string);
    public string? FontFamily
    {
        get => (string?)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(AddressEntry),
        4d,
        propertyChanged: (b, _, n) => ((AddressEntry)b).autoComplete.CornerRadius = (double)n);
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public event EventHandler<Address>? AddressSelected;

    async Task SearchAsync(string query)
    {
        searchCts?.Cancel();
        searchCts = new CancellationTokenSource();
        var token = searchCts.Token;

        autoComplete.IsBusy = true;
        try
        {
            var provider = SearchProvider ?? defaultProvider;
            var results = await provider.SearchAsync(query, CountryCodes, token);

            if (!token.IsCancellationRequested)
                autoComplete.ItemsSource = results as System.Collections.IList ?? results.ToList();
        }
        catch (TaskCanceledException) { }
        catch (Exception)
        {
            autoComplete.ItemsSource = null;
        }
        finally
        {
            autoComplete.IsBusy = false;
        }
    }

    void OnItemSelected(object? sender, object? e)
    {
        if (e is Address address)
        {
            SelectedAddress = address;
            AddressSelected?.Invoke(this, address);
        }
    }

    void OnSelectedAddressChanged(Address? address)
    {
        if (address != null)
        {
            autoComplete.Text = address.DisplayName;
            autoComplete.SelectedItem = address;
        }
        else
        {
            autoComplete.Text = string.Empty;
            autoComplete.SelectedItem = null;
        }
    }
}

[JsonSerializable(typeof(List<NominatimResult>))]
internal partial class NominatimJsonContext : JsonSerializerContext;

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
