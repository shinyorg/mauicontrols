using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sample.Features.AutoComplete;

public record ProgrammingLanguage(string Name, int Year, string Paradigm);

public partial class AutoCompleteViewModel : ObservableObject
{
    public List<string> Fruits { get; } =
    [
        "Apple", "Apricot", "Avocado", "Banana", "Blackberry", "Blueberry",
        "Cherry", "Coconut", "Cranberry", "Date", "Dragon Fruit", "Fig",
        "Grape", "Grapefruit", "Guava", "Kiwi", "Lemon", "Lime",
        "Lychee", "Mango", "Melon", "Nectarine", "Orange", "Papaya",
        "Passion Fruit", "Peach", "Pear", "Pineapple", "Plum", "Pomegranate",
        "Raspberry", "Starfruit", "Strawberry", "Tangerine", "Watermelon"
    ];

    public List<ProgrammingLanguage> Languages { get; } =
    [
        new("C", 1972, "Procedural"),
        new("C++", 1985, "Multi-paradigm"),
        new("C#", 2000, "Object-oriented"),
        new("Dart", 2011, "Object-oriented"),
        new("Elixir", 2011, "Functional"),
        new("F#", 2005, "Functional"),
        new("Go", 2009, "Concurrent"),
        new("Haskell", 1990, "Functional"),
        new("Java", 1995, "Object-oriented"),
        new("JavaScript", 1995, "Multi-paradigm"),
        new("Kotlin", 2011, "Multi-paradigm"),
        new("Lua", 1993, "Multi-paradigm"),
        new("Objective-C", 1984, "Object-oriented"),
        new("PHP", 1995, "Multi-paradigm"),
        new("Python", 1991, "Multi-paradigm"),
        new("Ruby", 1995, "Object-oriented"),
        new("Rust", 2010, "Multi-paradigm"),
        new("Scala", 2004, "Multi-paradigm"),
        new("Swift", 2014, "Multi-paradigm"),
        new("TypeScript", 2012, "Multi-paradigm")
    ];

    static readonly List<ProgrammingLanguage> AllCities =
    [
        new("New York", 1624, "Americas"),
        new("Los Angeles", 1781, "Americas"),
        new("London", 43, "Europe"),
        new("Paris", 250, "Europe"),
        new("Tokyo", 1457, "Asia"),
        new("Toronto", 1793, "Americas"),
        new("Sydney", 1788, "Oceania"),
        new("Singapore", 1819, "Asia"),
        new("Berlin", 1237, "Europe"),
        new("Mumbai", 1507, "Asia"),
        new("São Paulo", 1554, "Americas"),
        new("Dubai", 1833, "Middle East"),
        new("Seoul", 18, "Asia"),
        new("Mexico City", 1325, "Americas"),
        new("Stockholm", 1252, "Europe"),
        new("Amsterdam", 1275, "Europe"),
        new("Barcelona", 230, "Europe"),
        new("Rome", -753, "Europe"),
        new("San Francisco", 1776, "Americas"),
        new("Vancouver", 1886, "Americas")
    ];

    [ObservableProperty]
    object? selectedFruit;

    [ObservableProperty]
    object? selectedLanguage;

    [ObservableProperty]
    object? selectedLanguage2;

    [ObservableProperty]
    object? selectedCity;

    [ObservableProperty]
    ObservableCollection<ProgrammingLanguage>? cityResults;

    [ObservableProperty]
    bool isSearchingCities;

    public string? SelectedLanguageInfo => SelectedLanguage is ProgrammingLanguage lang
        ? $"Selected: {lang.Name} ({lang.Year}) - {lang.Paradigm}"
        : null;

    public string? SelectedCityInfo => SelectedCity is ProgrammingLanguage city
        ? $"Selected: {city.Name} (founded {city.Year}) - {city.Paradigm}"
        : null;

    partial void OnSelectedLanguageChanged(object? value) => OnPropertyChanged(nameof(SelectedLanguageInfo));
    partial void OnSelectedCityChanged(object? value) => OnPropertyChanged(nameof(SelectedCityInfo));

    [RelayCommand]
    async Task SearchCities(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return;

        IsSearchingCities = true;

        // Simulate network delay
        await Task.Delay(800);

        var filtered = AllCities
            .Where(c => c.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        CityResults = new ObservableCollection<ProgrammingLanguage>(filtered);
        IsSearchingCities = false;
    }
}
