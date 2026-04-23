using System.Collections;
using System.Windows.Input;

namespace Shiny.Maui.Controls;

public class AutoCompleteEntry : ContentView
{
    const int DefaultDebounceMs = 300;
    const double DefaultMaxDropDownHeight = 200;
    const double DefaultFontSize = 14;
    const double DefaultCornerRadius = 4;

    readonly BorderlessEntry entry;
    readonly ActivityIndicator spinner;
    readonly CollectionView suggestionList;
    readonly Border dropDownBorder;
    readonly Microsoft.Maui.Controls.Shapes.RoundRectangle dropDownShape;
    readonly Grid rootGrid;

    CancellationTokenSource? debounceCts;
    bool suppressTextChanged;

    public AutoCompleteEntry()
    {
        entry = new BorderlessEntry
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Center,
            FontSize = DefaultFontSize
        };
        entry.TextChanged += OnEntryTextChanged;
        entry.Focused += (_, _) => ShowDropDown();
        entry.Unfocused += (_, _) =>
        {
            // delay to allow tap on suggestion to register
            Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(200), HideDropDown);
        };

        spinner = new ActivityIndicator
        {
            IsRunning = false,
            IsVisible = false,
            WidthRequest = 20,
            HeightRequest = 20,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            Color = Colors.Grey
        };

        var entryGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            },
            VerticalOptions = LayoutOptions.Center
        };
        entryGrid.Add(entry, 0, 0);
        entryGrid.Add(spinner, 1, 0);

        suggestionList = new CollectionView
        {
            SelectionMode = SelectionMode.Single,
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.Fill
        };
        suggestionList.SelectionChanged += OnSuggestionSelected;

        dropDownShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(DefaultCornerRadius)
        };

        dropDownBorder = new Border
        {
            IsVisible = false,
            StrokeThickness = 1,
            Stroke = Colors.LightGray,
            BackgroundColor = Colors.White,
            Padding = 0,
            StrokeShape = dropDownShape,
            MaximumHeightRequest = DefaultMaxDropDownHeight,
            Content = suggestionList,
            Shadow = new Shadow
            {
                Brush = Brush.Black,
                Opacity = 0.15f,
                Radius = 6,
                Offset = new Point(0, 3)
            }
        };

        rootGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            }
        };
        rootGrid.Add(entryGrid, 0, 0);
        rootGrid.Add(dropDownBorder, 0, 1);

        Content = rootGrid;
    }


    // --- Bindable Properties ---

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(AutoCompleteEntry),
        string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) =>
        {
            var ctrl = (AutoCompleteEntry)b;
            if (ctrl.entry.Text != (string)n)
            {
                ctrl.suppressTextChanged = true;
                ctrl.entry.Text = (string)n;
                ctrl.suppressTextChanged = false;
            }
        });
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(AutoCompleteEntry),
        string.Empty,
        propertyChanged: (b, _, n) => ((AutoCompleteEntry)b).entry.Placeholder = (string)n);
    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor),
        typeof(Color),
        typeof(AutoCompleteEntry),
        null,
        propertyChanged: (b, _, n) => { if (n is Color c) ((AutoCompleteEntry)b).entry.PlaceholderColor = c; });
    public Color? PlaceholderColor
    {
        get => (Color?)GetValue(PlaceholderColorProperty);
        set => SetValue(PlaceholderColorProperty, value);
    }

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IList),
        typeof(AutoCompleteEntry),
        null);
    public IList? ItemsSource
    {
        get => (IList?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty SearchCommandProperty = BindableProperty.Create(
        nameof(SearchCommand),
        typeof(ICommand),
        typeof(AutoCompleteEntry),
        null);
    public ICommand? SearchCommand
    {
        get => (ICommand?)GetValue(SearchCommandProperty);
        set => SetValue(SearchCommandProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(object),
        typeof(AutoCompleteEntry),
        null,
        BindingMode.TwoWay);
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(
        nameof(IsBusy),
        typeof(bool),
        typeof(AutoCompleteEntry),
        false,
        BindingMode.TwoWay,
        propertyChanged: (b, _, n) =>
        {
            var ctrl = (AutoCompleteEntry)b;
            var busy = (bool)n;
            ctrl.spinner.IsRunning = busy;
            ctrl.spinner.IsVisible = busy;
        });
    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(AutoCompleteEntry),
        null,
        propertyChanged: (b, _, n) => ((AutoCompleteEntry)b).suggestionList.ItemTemplate = n as DataTemplate);
    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly BindableProperty TextMemberPathProperty = BindableProperty.Create(
        nameof(TextMemberPath),
        typeof(string),
        typeof(AutoCompleteEntry),
        null);
    public string? TextMemberPath
    {
        get => (string?)GetValue(TextMemberPathProperty);
        set => SetValue(TextMemberPathProperty, value);
    }

    public static readonly BindableProperty DebounceIntervalProperty = BindableProperty.Create(
        nameof(DebounceInterval),
        typeof(int),
        typeof(AutoCompleteEntry),
        DefaultDebounceMs);
    public int DebounceInterval
    {
        get => (int)GetValue(DebounceIntervalProperty);
        set => SetValue(DebounceIntervalProperty, value);
    }

    public static readonly BindableProperty ThresholdProperty = BindableProperty.Create(
        nameof(Threshold),
        typeof(int),
        typeof(AutoCompleteEntry),
        1);
    public int Threshold
    {
        get => (int)GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    public static readonly BindableProperty MaxDropDownHeightProperty = BindableProperty.Create(
        nameof(MaxDropDownHeight),
        typeof(double),
        typeof(AutoCompleteEntry),
        DefaultMaxDropDownHeight,
        propertyChanged: (b, _, n) => ((AutoCompleteEntry)b).dropDownBorder.MaximumHeightRequest = (double)n);
    public double MaxDropDownHeight
    {
        get => (double)GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(AutoCompleteEntry),
        null,
        propertyChanged: (b, _, n) => { if (n is Color c) ((AutoCompleteEntry)b).entry.TextColor = c; });
    public Color? TextColor
    {
        get => (Color?)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty DropDownBackgroundColorProperty = BindableProperty.Create(
        nameof(DropDownBackgroundColor),
        typeof(Color),
        typeof(AutoCompleteEntry),
        Colors.White,
        propertyChanged: (b, _, n) => { if (n is Color c) ((AutoCompleteEntry)b).dropDownBorder.BackgroundColor = c; });
    public Color? DropDownBackgroundColor
    {
        get => (Color?)GetValue(DropDownBackgroundColorProperty);
        set => SetValue(DropDownBackgroundColorProperty, value);
    }

    public static readonly BindableProperty DropDownBorderColorProperty = BindableProperty.Create(
        nameof(DropDownBorderColor),
        typeof(Color),
        typeof(AutoCompleteEntry),
        Colors.LightGray,
        propertyChanged: (b, _, n) => { if (n is Color c) ((AutoCompleteEntry)b).dropDownBorder.Stroke = c; });
    public Color? DropDownBorderColor
    {
        get => (Color?)GetValue(DropDownBorderColorProperty);
        set => SetValue(DropDownBorderColorProperty, value);
    }

    public static readonly BindableProperty SpinnerColorProperty = BindableProperty.Create(
        nameof(SpinnerColor),
        typeof(Color),
        typeof(AutoCompleteEntry),
        Colors.Grey,
        propertyChanged: (b, _, n) => { if (n is Color c) ((AutoCompleteEntry)b).spinner.Color = c; });
    public Color? SpinnerColor
    {
        get => (Color?)GetValue(SpinnerColorProperty);
        set => SetValue(SpinnerColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(AutoCompleteEntry),
        DefaultFontSize,
        propertyChanged: (b, _, n) => ((AutoCompleteEntry)b).entry.FontSize = (double)n);
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        nameof(FontFamily),
        typeof(string),
        typeof(AutoCompleteEntry),
        null,
        propertyChanged: (b, _, n) => ((AutoCompleteEntry)b).entry.FontFamily = n as string);
    public string? FontFamily
    {
        get => (string?)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(
        nameof(FontAttributes),
        typeof(FontAttributes),
        typeof(AutoCompleteEntry),
        Microsoft.Maui.Controls.FontAttributes.None,
        propertyChanged: (b, _, n) => ((AutoCompleteEntry)b).entry.FontAttributes = (FontAttributes)n);
    public FontAttributes FontAttributes
    {
        get => (FontAttributes)GetValue(FontAttributesProperty);
        set => SetValue(FontAttributesProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(AutoCompleteEntry),
        DefaultCornerRadius,
        propertyChanged: (b, _, n) => ((AutoCompleteEntry)b).dropDownShape.CornerRadius = new CornerRadius((double)n));
    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }


    // --- Events ---

    public event EventHandler<object?>? ItemSelected;


    // --- Private Methods ---

    void OnEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (suppressTextChanged)
            return;

        Text = e.NewTextValue;
        SelectedItem = null;

        debounceCts?.Cancel();

        var searchText = e.NewTextValue ?? string.Empty;
        if (searchText.Length < Threshold)
        {
            HideDropDown();
            return;
        }

        debounceCts = new CancellationTokenSource();
        var token = debounceCts.Token;
        var debounce = DebounceInterval;

        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(debounce), () =>
        {
            if (token.IsCancellationRequested)
                return;

            PerformSearch(searchText);
        });
    }

    void PerformSearch(string searchText)
    {
        if (SearchCommand != null)
        {
            if (SearchCommand.CanExecute(searchText))
                SearchCommand.Execute(searchText);

            ShowDropDown();
        }
        else
        {
            FilterLocalItems(searchText);
        }
    }

    void FilterLocalItems(string searchText)
    {
        var source = ItemsSource;
        if (source == null || source.Count == 0)
        {
            HideDropDown();
            return;
        }

        var filtered = new List<object>();
        var comparison = StringComparison.OrdinalIgnoreCase;

        foreach (var item in source)
        {
            var text = GetDisplayText(item);
            if (text.Contains(searchText, comparison))
                filtered.Add(item);
        }

        if (filtered.Count > 0)
        {
            suggestionList.ItemsSource = filtered;
            ShowDropDown();
        }
        else
        {
            HideDropDown();
        }
    }

    string GetDisplayText(object item)
    {
        if (item is string s)
            return s;

        if (!string.IsNullOrEmpty(TextMemberPath))
        {
            var prop = item.GetType().GetProperty(TextMemberPath);
            if (prop != null)
                return prop.GetValue(item)?.ToString() ?? string.Empty;
        }

        return item.ToString() ?? string.Empty;
    }

    void OnSuggestionSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0)
            return;

        var selected = e.CurrentSelection[0];
        SelectedItem = selected;

        suppressTextChanged = true;
        var displayText = GetDisplayText(selected);
        entry.Text = displayText;
        Text = displayText;
        suppressTextChanged = false;

        HideDropDown();
        suggestionList.SelectedItem = null;

        ItemSelected?.Invoke(this, selected);
    }

    void ShowDropDown()
    {
        if (suggestionList.ItemsSource is IList list && list.Count > 0)
            dropDownBorder.IsVisible = true;
        else if (suggestionList.ItemsSource is IEnumerable enumerable && enumerable.Cast<object>().Any())
            dropDownBorder.IsVisible = true;
    }

    void HideDropDown()
    {
        dropDownBorder.IsVisible = false;
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        // When ItemsSource changes externally (e.g. from SearchCommand callback), update dropdown
        if (propertyName == nameof(ItemsSource))
        {
            suggestionList.ItemsSource = ItemsSource;
            if (entry.IsFocused && !string.IsNullOrEmpty(Text) && Text.Length >= Threshold)
                ShowDropDown();
        }
    }
}
