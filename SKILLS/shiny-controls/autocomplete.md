# AutoCompleteEntry

A text input with debounced search, dropdown suggestions, busy indicator, and custom item templates. Supports both local filtering (set `ItemsSource` without `SearchCommand`) and remote search (set `SearchCommand` to trigger async search). Available on both MAUI and Blazor with full styling control.

## MAUI Usage

```xml
<!-- Local filtering -->
<shiny:AutoCompleteEntry
    Text="{Binding SearchText}"
    Placeholder="Search fruits..."
    ItemsSource="{Binding AllFruits}"
    SelectedItem="{Binding SelectedFruit}"
    TextMemberPath="Name"
    FontSize="16"
    CornerRadius="8" />

<!-- Remote search -->
<shiny:AutoCompleteEntry
    Text="{Binding SearchText}"
    Placeholder="Search..."
    ItemsSource="{Binding Results}"
    SelectedItem="{Binding SelectedResult}"
    SearchCommand="{Binding SearchCommand}"
    IsBusy="{Binding IsSearching}"
    TextMemberPath="Name"
    DebounceInterval="400"
    Threshold="2"
    MaxDropDownHeight="300"
    FontSize="16"
    FontFamily="OpenSans"
    TextColor="Black"
    DropDownBackgroundColor="White"
    DropDownBorderColor="LightGray"
    SpinnerColor="DodgerBlue"
    CornerRadius="8" />
```

### MAUI Custom Item Template

```csharp
var autoComplete = new AutoCompleteEntry
{
    Placeholder = "Search products...",
    TextMemberPath = nameof(Product.Name),
    FontSize = 16,
    CornerRadius = 8,
    ItemTemplate = new DataTemplate(() =>
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            Padding = new Thickness(8, 6)
        };

        var name = new Label { FontSize = 14, VerticalTextAlignment = TextAlignment.Center };
        name.SetBinding(Label.TextProperty, nameof(Product.Name));

        var price = new Label { FontSize = 12, TextColor = Colors.Grey };
        price.SetBinding(Label.TextProperty, nameof(Product.PriceDisplay));

        grid.Add(name, 0, 0);
        grid.Add(price, 1, 0);
        return grid;
    })
};
```

## Blazor Usage

```razor
<AutoCompleteEntry
    Text="@searchText"
    TextChanged="v => searchText = v"
    Placeholder="Search..."
    ItemsSource="@results"
    SelectedItemChanged="OnSelected"
    SearchCommand="OnSearch"
    IsBusy="@isBusy"
    TextMemberPath="Name"
    DebounceInterval="400"
    Threshold="2"
    FontSize="16"
    TextColor="#333"
    DropDownBackgroundColor="#fff"
    DropDownBorderColor="#ccc"
    SpinnerColor="#3B82F6"
    InputClass="my-input"
    DropDownClass="my-dropdown">
    <ItemTemplate Context="item">
        @{ var p = (Product)item; }
        <div style="display:flex;justify-content:space-between;">
            <span>@p.Name</span>
            <span style="color:#888;">@p.PriceDisplay</span>
        </div>
    </ItemTemplate>
</AutoCompleteEntry>

@code {
    string searchText = "";
    bool isBusy;
    IList? results;

    async Task OnSearch(string query)
    {
        isBusy = true;
        results = await MyApi.SearchAsync(query);
        isBusy = false;
    }

    void OnSelected(object? item)
    {
        if (item is Product p) { /* handle selection */ }
    }
}
```

### Blazor CSS Custom Properties

Override these on a parent element or the component itself to theme without parameters:

| Variable | Default | Controls |
|---|---|---|
| `--shiny-ac-text` | inherit | Input text color |
| `--shiny-ac-ph` | #9CA3AF | Placeholder color |
| `--shiny-ac-dd-bg` | #fff | Dropdown background |
| `--shiny-ac-dd-border` | #D1D5DB | Dropdown border |
| `--shiny-ac-spinner` | #9CA3AF | Spinner color |
| `--shiny-ac-font-size` | inherit | Input font size |
| `--shiny-ac-dd-max-h` | 200px | Dropdown max height |

Example: theme a whole page section without touching each component:

```css
.dark-theme {
    --shiny-ac-text: #f0f0f0;
    --shiny-ac-ph: #888;
    --shiny-ac-dd-bg: #1e1e1e;
    --shiny-ac-dd-border: #444;
    --shiny-ac-spinner: #60A5FA;
}
```

## AutoCompleteEntry Properties

| Property | Type | Default | Host | Description |
|---|---|---|---|---|
| `Text` | `string` | `""` | Both | Current text value (TwoWay) |
| `Placeholder` | `string?` | `null` | Both | Placeholder text |
| `PlaceholderColor` | Color/string | `null` | Both | Placeholder color |
| `ItemsSource` | `IList` | `null` | Both | Suggestion items |
| `SelectedItem` | `object?` | `null` | Both | Selected item (TwoWay) |
| `SearchCommand` | ICommand/EventCallback | `null` | Both | Async search trigger |
| `TextMemberPath` | `string?` | `null` | Both | Display property name |
| `ItemTemplate` | DataTemplate/RenderFragment | `null` | Both | Custom item template |
| `IsBusy` | `bool` | `false` | Both | Show spinner (TwoWay) |
| `DebounceInterval` | `int` | `300` | Both | Debounce delay (ms) |
| `Threshold` | `int` | `1` | Both | Min chars before search |
| `MaxDropDownHeight` | `double` | `200` | Both | Max dropdown height |
| `TextColor` | Color/string | `null` | Both | Text color |
| `FontSize` | `double` | `14` | Both | Font size |
| `FontFamily` | `string?` | `null` | MAUI | Font family |
| `FontAttributes` | `FontAttributes` | `None` | MAUI | Bold/italic |
| `DropDownBackgroundColor` | Color/string | White | Both | Dropdown background |
| `DropDownBorderColor` | Color/string | LightGray | Both | Dropdown border |
| `CornerRadius` | `double` | `4` | MAUI | Dropdown corner radius |
| `SpinnerColor` | Color/string | Grey | Both | Spinner color |
| `CssClass` | `string?` | `null` | Blazor | Root CSS class |
| `InputClass` | `string?` | `null` | Blazor | Input CSS class |
| `DropDownClass` | `string?` | `null` | Blazor | Dropdown CSS class |
| `AdditionalAttributes` | `IDictionary` | `null` | Blazor | Unmatched HTML attributes |

Events: `ItemSelected` (MAUI: `EventHandler<object?>`, Blazor: `EventCallback<object?>`)
