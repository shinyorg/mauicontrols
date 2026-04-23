using CommunityToolkit.Mvvm.ComponentModel;
using Shiny.Maui.Controls;

namespace Sample.Features.CountryAddress;

public partial class CountryAddressViewModel : ObservableObject
{
    [ObservableProperty]
    Country? selectedCountry;

    [ObservableProperty]
    Address? selectedAddress;

    public bool HasCountry => SelectedCountry is not null;
    public bool HasAddress => SelectedAddress is not null;

    public string? CountryCodeDisplay => SelectedCountry is { } c
        ? $"{c.Code2} / {c.Code3}"
        : null;

    public string? CountryCodeFilter => SelectedCountry?.Code2.ToLowerInvariant();

    public string? StreetDisplay => SelectedAddress is { } a
        ? string.IsNullOrEmpty(a.HouseNumber)
            ? a.Street
            : $"{a.HouseNumber} {a.Street}"
        : null;

    public string? CoordsDisplay => SelectedAddress is { } a
        ? $"{a.Latitude:F5}, {a.Longitude:F5}"
        : null;

    partial void OnSelectedCountryChanged(Country? value)
    {
        OnPropertyChanged(nameof(HasCountry));
        OnPropertyChanged(nameof(CountryCodeDisplay));
        OnPropertyChanged(nameof(CountryCodeFilter));
    }

    partial void OnSelectedAddressChanged(Address? value)
    {
        OnPropertyChanged(nameof(HasAddress));
        OnPropertyChanged(nameof(StreetDisplay));
        OnPropertyChanged(nameof(CoordsDisplay));
    }
}
