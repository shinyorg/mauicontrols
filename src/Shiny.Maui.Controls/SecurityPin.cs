using Shiny.Maui.Controls.Infrastructure;

namespace Shiny.Maui.Controls;

public class SecurityPin : ContentView
{
    readonly Grid rootGrid;
    readonly HorizontalStackLayout cellsLayout;
    readonly Entry hiddenEntry;
    readonly List<Border> cellBorders = new();
    readonly List<Label> cellLabels = new();

    bool isUpdatingValue;

    public SecurityPin()
    {
        hiddenEntry = new Entry
        {
            Opacity = 0,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.Transparent,
            HeightRequest = 1,
            WidthRequest = 1,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            IsSpellCheckEnabled = false,
            IsTextPredictionEnabled = false,
            Keyboard = Keyboard.Numeric
        };
        hiddenEntry.TextChanged += OnHiddenEntryTextChanged;
        hiddenEntry.Completed += OnHiddenEntryCompleted;

        cellsLayout = new HorizontalStackLayout
        {
            Spacing = DefaultCellSpacing,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => hiddenEntry.Focus();
        cellsLayout.GestureRecognizers.Add(tap);

        rootGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        rootGrid.Children.Add(hiddenEntry);
        rootGrid.Children.Add(cellsLayout);

        Content = rootGrid;
        BuildCells();
    }

    const double DefaultCellSize = 50;
    const double DefaultCellSpacing = 8;
    const double DefaultCornerRadius = 8;
    const double DefaultFontSize = 24;
    const int DefaultLength = 4;


    public event EventHandler<SecurityPinCompletedEventArgs>? Completed;


    public static readonly BindableProperty LengthProperty = BindableProperty.Create(
        nameof(Length),
        typeof(int),
        typeof(SecurityPin),
        DefaultLength,
        propertyChanged: (b, _, _) => ((SecurityPin)b).BuildCells()
    );
    public int Length
    {
        get => (int)GetValue(LengthProperty);
        set => SetValue(LengthProperty, value);
    }

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(string),
        typeof(SecurityPin),
        string.Empty,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((SecurityPin)b).OnValueChangedExternally((string?)n)
    );
    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard),
        typeof(Keyboard),
        typeof(SecurityPin),
        Keyboard.Numeric,
        propertyChanged: (b, _, n) => ((SecurityPin)b).hiddenEntry.Keyboard = (Keyboard)n
    );
    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public static readonly BindableProperty HideCharacterProperty = BindableProperty.Create(
        nameof(HideCharacter),
        typeof(string),
        typeof(SecurityPin),
        null,
        propertyChanged: (b, _, _) => ((SecurityPin)b).RefreshCells()
    );
    public string? HideCharacter
    {
        get => (string?)GetValue(HideCharacterProperty);
        set => SetValue(HideCharacterProperty, value);
    }

    public static readonly BindableProperty CellSizeProperty = BindableProperty.Create(
        nameof(CellSize),
        typeof(double),
        typeof(SecurityPin),
        DefaultCellSize,
        propertyChanged: (b, _, _) => ((SecurityPin)b).ApplyCellStyle()
    );
    public double CellSize
    {
        get => (double)GetValue(CellSizeProperty);
        set => SetValue(CellSizeProperty, value);
    }

    public static readonly BindableProperty CellSpacingProperty = BindableProperty.Create(
        nameof(CellSpacing),
        typeof(double),
        typeof(SecurityPin),
        DefaultCellSpacing,
        propertyChanged: (b, _, n) => ((SecurityPin)b).cellsLayout.Spacing = (double)n
    );
    public double CellSpacing
    {
        get => (double)GetValue(CellSpacingProperty);
        set => SetValue(CellSpacingProperty, value);
    }

    public static readonly BindableProperty CellCornerRadiusProperty = BindableProperty.Create(
        nameof(CellCornerRadius),
        typeof(double),
        typeof(SecurityPin),
        DefaultCornerRadius,
        propertyChanged: (b, _, _) => ((SecurityPin)b).ApplyCellStyle()
    );
    public double CellCornerRadius
    {
        get => (double)GetValue(CellCornerRadiusProperty);
        set => SetValue(CellCornerRadiusProperty, value);
    }

    public static readonly BindableProperty CellBorderColorProperty = BindableProperty.Create(
        nameof(CellBorderColor),
        typeof(Color),
        typeof(SecurityPin),
        null,
        propertyChanged: (b, _, _) => ((SecurityPin)b).ApplyCellStyle()
    );
    public Color? CellBorderColor
    {
        get => (Color?)GetValue(CellBorderColorProperty);
        set => SetValue(CellBorderColorProperty, value);
    }

    public static readonly BindableProperty CellFocusedBorderColorProperty = BindableProperty.Create(
        nameof(CellFocusedBorderColor),
        typeof(Color),
        typeof(SecurityPin),
        null,
        propertyChanged: (b, _, _) => ((SecurityPin)b).ApplyCellStyle()
    );
    public Color? CellFocusedBorderColor
    {
        get => (Color?)GetValue(CellFocusedBorderColorProperty);
        set => SetValue(CellFocusedBorderColorProperty, value);
    }

    public static readonly BindableProperty CellBackgroundColorProperty = BindableProperty.Create(
        nameof(CellBackgroundColor),
        typeof(Color),
        typeof(SecurityPin),
        null,
        propertyChanged: (b, _, _) => ((SecurityPin)b).ApplyCellStyle()
    );
    public Color? CellBackgroundColor
    {
        get => (Color?)GetValue(CellBackgroundColorProperty);
        set => SetValue(CellBackgroundColorProperty, value);
    }

    public static readonly BindableProperty CellFocusedBackgroundColorProperty = BindableProperty.Create(
        nameof(CellFocusedBackgroundColor),
        typeof(Color),
        typeof(SecurityPin),
        null,
        propertyChanged: (b, _, _) => ((SecurityPin)b).ApplyCellStyle()
    );
    public Color? CellFocusedBackgroundColor
    {
        get => (Color?)GetValue(CellFocusedBackgroundColorProperty);
        set => SetValue(CellFocusedBackgroundColorProperty, value);
    }

    public static readonly BindableProperty CellTextColorProperty = BindableProperty.Create(
        nameof(CellTextColor),
        typeof(Color),
        typeof(SecurityPin),
        null,
        propertyChanged: (b, _, n) =>
        {
            var pin = (SecurityPin)b;
            if (n is Color c)
            {
                foreach (var label in pin.cellLabels)
                    label.TextColor = c;
            }
        }
    );
    public Color? CellTextColor
    {
        get => (Color?)GetValue(CellTextColorProperty);
        set => SetValue(CellTextColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(SecurityPin),
        DefaultFontSize,
        propertyChanged: (b, _, n) =>
        {
            var pin = (SecurityPin)b;
            foreach (var label in pin.cellLabels)
                label.FontSize = (double)n;
        }
    );
    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }


    public static readonly BindableProperty UseFeedbackProperty = BindableProperty.Create(
        nameof(UseFeedback),
        typeof(bool),
        typeof(SecurityPin),
        true);
    public bool UseFeedback
    {
        get => (bool)GetValue(UseFeedbackProperty);
        set => SetValue(UseFeedbackProperty, value);
    }


    public new void Focus() => hiddenEntry.Focus();
    public new void Unfocus() => hiddenEntry.Unfocus();
    public void Clear() => UpdateValue(string.Empty);


    void BuildCells()
    {
        cellsLayout.Clear();
        cellBorders.Clear();
        cellLabels.Clear();

        var count = Math.Max(1, Length);
        for (var i = 0; i < count; i++)
        {
            var label = new Label
            {
                FontSize = FontSize,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            if (CellTextColor is Color textColor)
                label.TextColor = textColor;

            var border = new Border
            {
                WidthRequest = CellSize,
                HeightRequest = CellSize,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
                {
                    CornerRadius = new CornerRadius(CellCornerRadius)
                },
                StrokeThickness = 1,
                Content = label
            };

            cellLabels.Add(label);
            cellBorders.Add(border);
            cellsLayout.Add(border);
        }

        ApplyCellStyle();
        RefreshCells();

        // Keep the Entry's MaxLength in sync with pin length
        hiddenEntry.MaxLength = count;

        // If current value is longer than new length, truncate
        if (!string.IsNullOrEmpty(Value) && Value.Length > count)
            UpdateValue(Value.Substring(0, count));
    }

    void ApplyCellStyle()
    {
        for (var i = 0; i < cellBorders.Count; i++)
        {
            var border = cellBorders[i];
            border.WidthRequest = CellSize;
            border.HeightRequest = CellSize;
            border.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = new CornerRadius(CellCornerRadius)
            };

            ApplyActiveCellStyle(border, i);
        }
    }

    void ApplyActiveCellStyle(Border border, int index)
    {
        var isCurrent = index == CurrentIndex && hiddenEntry.IsFocused;

        if (isCurrent && CellFocusedBorderColor is Color focusedBorder)
            border.Stroke = focusedBorder;
        else if (CellBorderColor is Color normal)
            border.Stroke = normal;

        if (isCurrent && CellFocusedBackgroundColor is Color focusedBg)
            border.BackgroundColor = focusedBg;
        else if (CellBackgroundColor is Color bg)
            border.BackgroundColor = bg;
    }

    int CurrentIndex => Math.Min(Value?.Length ?? 0, cellBorders.Count - 1);

    void RefreshCells()
    {
        var value = Value ?? string.Empty;
        var hide = HideCharacter;
        var shouldHide = !string.IsNullOrEmpty(hide);

        for (var i = 0; i < cellLabels.Count; i++)
        {
            if (i < value.Length)
                cellLabels[i].Text = shouldHide ? hide : value[i].ToString();
            else
                cellLabels[i].Text = string.Empty;
        }

        for (var i = 0; i < cellBorders.Count; i++)
            ApplyActiveCellStyle(cellBorders[i], i);
    }

    void OnHiddenEntryTextChanged(object? sender, TextChangedEventArgs e)
    {
        var newText = e.NewTextValue ?? string.Empty;
        var max = Math.Max(1, Length);
        if (newText.Length > max)
            newText = newText.Substring(0, max);

        UpdateValue(newText);

        if (newText.Length >= max)
        {
            if (UseFeedback)
                FeedbackHelper.Execute(typeof(SecurityPin), nameof(Completed), "LongPress");

            Completed?.Invoke(this, new SecurityPinCompletedEventArgs(newText));
        }
        else if (newText.Length > (e.OldTextValue?.Length ?? 0))
        {
            if (UseFeedback)
                FeedbackHelper.Execute(typeof(SecurityPin), "DigitEntered");
        }
    }

    void OnHiddenEntryCompleted(object? sender, EventArgs e)
    {
        if ((Value?.Length ?? 0) >= Length)
            Completed?.Invoke(this, new SecurityPinCompletedEventArgs(Value ?? string.Empty));
    }

    void OnValueChangedExternally(string? newValue)
    {
        if (isUpdatingValue)
            return;

        var value = newValue ?? string.Empty;
        var max = Math.Max(1, Length);
        if (value.Length > max)
            value = value.Substring(0, max);

        if (hiddenEntry.Text != value)
        {
            isUpdatingValue = true;
            hiddenEntry.Text = value;
            isUpdatingValue = false;
        }

        RefreshCells();
    }

    void UpdateValue(string value)
    {
        isUpdatingValue = true;
        if (hiddenEntry.Text != value)
            hiddenEntry.Text = value;
        if (Value != value)
            Value = value;
        isUpdatingValue = false;

        RefreshCells();
    }
}

public class SecurityPinCompletedEventArgs(string value) : EventArgs
{
    public string Value { get; } = value;
}
