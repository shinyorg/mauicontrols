namespace Shiny.Maui.Controls;

public class CountryPicker : ContentView
{
    static readonly bool showFlags = DeviceInfo.Platform != DevicePlatform.WinUI;

    readonly AutoCompleteEntry autoComplete;
    readonly Label selectedFlagLabel;
    readonly Grid rootGrid;

    public CountryPicker()
    {
        autoComplete = new AutoCompleteEntry
        {
            Placeholder = "Search countries...",
            ItemsSource = CountryData.All.ToList(),
            TextMemberPath = nameof(Country.Name),
            HorizontalOptions = LayoutOptions.Fill,
            ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Auto),
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    },
                    ColumnSpacing = 8,
                    Padding = new Thickness(8, 6)
                };

                var flag = new Label
                {
                    FontSize = 20,
                    VerticalTextAlignment = TextAlignment.Center,
                    IsVisible = showFlags
                };
                flag.SetBinding(Label.TextProperty, nameof(Country.FlagEmoji));

                var name = new Label
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 14
                };
                name.SetBinding(Label.TextProperty, nameof(Country.Name));

                var dial = new Label
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 12,
                    TextColor = Colors.Grey
                };
                dial.SetBinding(Label.TextProperty, nameof(Country.DialCode));

                grid.Add(flag, 0, 0);
                grid.Add(name, 1, 0);
                grid.Add(dial, 2, 0);
                return grid;
            })
        };
        autoComplete.ItemSelected += OnItemSelected;

        selectedFlagLabel = new Label
        {
            FontSize = 24,
            VerticalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest = showFlags ? 32 : 0,
            IsVisible = false
        };

        rootGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(showFlags ? new GridLength(36) : new GridLength(0)),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = showFlags ? 4 : 0
        };
        rootGrid.Add(selectedFlagLabel, 0, 0);
        rootGrid.Add(autoComplete, 1, 0);

        Content = rootGrid;
    }

    public static readonly BindableProperty SelectedCountryProperty = BindableProperty.Create(
        nameof(SelectedCountry),
        typeof(Country),
        typeof(CountryPicker),
        null,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((CountryPicker)b).OnSelectedCountryChanged(n as Country));
    public Country? SelectedCountry
    {
        get => (Country?)GetValue(SelectedCountryProperty);
        set => SetValue(SelectedCountryProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(CountryPicker),
        "Search countries...",
        propertyChanged: (b, _, n) => ((CountryPicker)b).autoComplete.Placeholder = (string)n);
    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(CountryPicker),
        null,
        propertyChanged: (b, _, n) => ((CountryPicker)b).autoComplete.TextColor = n as Color);
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor),
        typeof(Color),
        typeof(CountryPicker),
        null,
        propertyChanged: (b, _, n) => ((CountryPicker)b).autoComplete.PlaceholderColor = n as Color);
    public Color? PlaceholderColor
    {
        get => (Color?)GetValue(PlaceholderColorProperty);
        set => SetValue(PlaceholderColorProperty, value);
    }

    public static readonly BindableProperty DropDownBackgroundColorProperty = BindableProperty.Create(
        nameof(DropDownBackgroundColor),
        typeof(Color),
        typeof(CountryPicker),
        null,
        propertyChanged: (b, _, n) => { if (n is Color c) ((CountryPicker)b).autoComplete.DropDownBackgroundColor = c; });
    public Color? DropDownBackgroundColor
    {
        get => (Color?)GetValue(DropDownBackgroundColorProperty);
        set => SetValue(DropDownBackgroundColorProperty, value);
    }

    public static readonly BindableProperty MaxDropDownHeightProperty = BindableProperty.Create(
        nameof(MaxDropDownHeight),
        typeof(double),
        typeof(CountryPicker),
        200d,
        propertyChanged: (b, _, n) => ((CountryPicker)b).autoComplete.MaxDropDownHeight = (double)n);
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(CountryPicker),
        14d,
        propertyChanged: (b, _, n) => ((CountryPicker)b).autoComplete.FontSize = (double)n);
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(CountryPicker),
        null,
        propertyChanged: (b, _, n) => ((CountryPicker)b).autoComplete.FontFamily = n as string);
    public string? FontFamily
    {
        get => (string?)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(CountryPicker),
        4d,
        propertyChanged: (b, _, n) => ((CountryPicker)b).autoComplete.CornerRadius = (double)n);
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public event EventHandler<Country>? CountrySelected;

    void OnItemSelected(object? sender, object? e)
    {
        if (e is Country country)
        {
            SelectedCountry = country;
            CountrySelected?.Invoke(this, country);
        }
    }

    void OnSelectedCountryChanged(Country? country)
    {
        if (country != null)
        {
            autoComplete.Text = country.Name;
            autoComplete.SelectedItem = country;
            if (showFlags)
            {
                selectedFlagLabel.Text = country.FlagEmoji;
                selectedFlagLabel.IsVisible = true;
            }
        }
        else
        {
            autoComplete.Text = string.Empty;
            autoComplete.SelectedItem = null;
            selectedFlagLabel.IsVisible = false;
        }
    }
}
