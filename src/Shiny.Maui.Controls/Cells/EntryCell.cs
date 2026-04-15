using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Shiny.Maui.Controls;

namespace Shiny.Maui.Controls.Cells;

public class EntryCell : CellBase
{
    static readonly Style CleanEntryStyle = new(typeof(BorderlessEntry))
    {
        Setters =
        {
            new Setter { Property = Entry.BackgroundColorProperty, Value = Colors.Transparent },
            new Setter { Property = VisualElement.HeightRequestProperty, Value = 40d },
        }
    };

    BorderlessEntry entry = default!;

    public static readonly BindableProperty ValueTextProperty = BindableProperty.Create(
        nameof(ValueText), typeof(string), typeof(EntryCell), string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((EntryCell)b).OnValueTextChanged((string)n));

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(EntryCell), null,
        propertyChanged: (b, o, n) => ((EntryCell)b).UpdateEntryColor());

    public static readonly BindableProperty ValueTextFontSizeProperty = BindableProperty.Create(
        nameof(ValueTextFontSize), typeof(double), typeof(EntryCell), -1d,
        propertyChanged: (b, o, n) => ((EntryCell)b).UpdateEntryFontSize());

    public static readonly BindableProperty ValueTextFontFamilyProperty = BindableProperty.Create(
        nameof(ValueTextFontFamily), typeof(string), typeof(EntryCell), null,
        propertyChanged: (b, o, n) => ((EntryCell)b).UpdateEntryFontFamily());

    public static readonly BindableProperty ValueTextFontAttributesProperty = BindableProperty.Create(
        nameof(ValueTextFontAttributes), typeof(FontAttributes?), typeof(EntryCell), null,
        propertyChanged: (b, o, n) => ((EntryCell)b).UpdateEntryFontAttributes());

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(EntryCell), string.Empty,
        propertyChanged: (b, o, n) => ((EntryCell)b).entry.Placeholder = (string)n);

    public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
        nameof(PlaceholderColor), typeof(Color), typeof(EntryCell), null,
        propertyChanged: (b, o, n) => { if (n is Color c) ((EntryCell)b).entry.PlaceholderColor = c; });

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard), typeof(Keyboard), typeof(EntryCell), Keyboard.Default,
        propertyChanged: (b, o, n) => ((EntryCell)b).entry.Keyboard = (Keyboard)n);

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPassword), typeof(bool), typeof(EntryCell), false,
        propertyChanged: (b, o, n) => ((EntryCell)b).entry.IsPassword = (bool)n);

    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
        nameof(MaxLength), typeof(int), typeof(EntryCell), -1,
        propertyChanged: (b, o, n) =>
        {
            var cell = (EntryCell)b;
            cell.entry.MaxLength = (int)n > 0 ? (int)n : int.MaxValue;
        });

    public static readonly BindableProperty TextAlignmentProperty = BindableProperty.Create(
        nameof(TextAlignment), typeof(TextAlignment), typeof(EntryCell), TextAlignment.End,
        propertyChanged: (b, o, n) => ((EntryCell)b).entry.HorizontalTextAlignment = (TextAlignment)n);

    public static readonly BindableProperty CompletedCommandProperty = BindableProperty.Create(
        nameof(CompletedCommand), typeof(ICommand), typeof(EntryCell), null);

    public string ValueText
    {
        get => (string)GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    public Color? ValueTextColor
    {
        get => (Color?)GetValue(ValueTextColorProperty);
        set => SetValue(ValueTextColorProperty, value);
    }

    public double ValueTextFontSize
    {
        get => (double)GetValue(ValueTextFontSizeProperty);
        set => SetValue(ValueTextFontSizeProperty, value);
    }

    public string? ValueTextFontFamily
    {
        get => (string?)GetValue(ValueTextFontFamilyProperty);
        set => SetValue(ValueTextFontFamilyProperty, value);
    }

    public FontAttributes? ValueTextFontAttributes
    {
        get => (FontAttributes?)GetValue(ValueTextFontAttributesProperty);
        set => SetValue(ValueTextFontAttributesProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public Color? PlaceholderColor
    {
        get => (Color?)GetValue(PlaceholderColorProperty);
        set => SetValue(PlaceholderColorProperty, value);
    }

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    public TextAlignment TextAlignment
    {
        get => (TextAlignment)GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    public ICommand? CompletedCommand
    {
        get => (ICommand?)GetValue(CompletedCommandProperty);
        set => SetValue(CompletedCommandProperty, value);
    }

    public event EventHandler? Completed;

    protected override View? CreateAccessoryView()
    {
        entry = new BorderlessEntry
        {
            Style = CleanEntryStyle,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Fill,
            HorizontalTextAlignment = TextAlignment.End,
            MinimumWidthRequest = 120
        };
        entry.TextChanged += (s, e) =>
        {
            if (ValueText != e.NewTextValue)
                ValueText = e.NewTextValue;
        };
        entry.Completed += (s, e) =>
        {
            Completed?.Invoke(this, EventArgs.Empty);
            if (CompletedCommand?.CanExecute(ValueText) == true)
                CompletedCommand.Execute(ValueText);
        };
        return entry;
    }

    public void SetFocus() => entry?.Focus();

    void OnValueTextChanged(string newValue)
    {
        if (entry != null && entry.Text != newValue)
            entry.Text = newValue;
    }

    void UpdateEntryColor()
    {
        var color = ValueTextColor ?? ParentTableView?.CellValueTextColor;
        if (color != null)
            entry.TextColor = color;
        else
            entry.ClearValue(Entry.TextColorProperty);
    }

    void UpdateEntryFontSize()
        => entry.FontSize = ResolveDouble(ValueTextFontSize, ParentTableView?.CellValueTextFontSize ?? -1, 16);

    void UpdateEntryFontFamily()
        => entry.FontFamily = ResolveFontFamily(ValueTextFontFamily, ParentTableView?.CellValueTextFontFamily);

    void UpdateEntryFontAttributes()
        => entry.FontAttributes = ResolveFontAttributes(ValueTextFontAttributes, ParentTableView?.CellValueTextFontAttributes);

    protected override void OnTapped()
    {
        entry.Focus();
    }
}