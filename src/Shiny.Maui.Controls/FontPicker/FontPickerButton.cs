using System.Windows.Input;

namespace Shiny.Maui.Controls.FontPicker;

public class FontPickerButton : ContentView
{
    readonly Border buttonBorder;
    readonly Label buttonLabel;
    readonly Border popupBorder;
    readonly FontPicker picker;
    readonly Grid overlay;

    bool isOpen;

    public FontPickerButton()
    {
        buttonLabel = new Label
        {
            FontSize = 13,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.TailTruncation
        };

        buttonBorder = new Border
        {
            StrokeThickness = 2,
            Stroke = Colors.LightGray,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(12, 6),
            MinimumWidthRequest = 80,
            MinimumHeightRequest = 36,
            BackgroundColor = Color.FromArgb("#2C2C2E"),
            Content = buttonLabel,
            HorizontalOptions = LayoutOptions.Start
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnButtonTapped;
        buttonBorder.GestureRecognizers.Add(tap);

        picker = new FontPicker
        {
            HeightRequest = 320,
            WidthRequest = 320
        };
        picker.FontChanged += OnPickerFontChanged;

        var doneButton = new Button
        {
            Text = "Done",
            FontSize = 14,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#007AFF"),
            CornerRadius = 8,
            HeightRequest = 36,
            HorizontalOptions = LayoutOptions.Fill,
            Margin = new Thickness(12, 0, 12, 12)
        };
        doneButton.Clicked += (_, _) => Close();

        var pickerWithDone = new VerticalStackLayout
        {
            Children = { picker, doneButton }
        };

        popupBorder = new Border
        {
            StrokeThickness = 1,
            Stroke = Colors.LightGray,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            BackgroundColor = Colors.White,
            Padding = 0,
            Content = pickerWithDone,
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Colors.Black),
                Offset = new Point(0, 4),
                Radius = 12,
                Opacity = 0.25f
            },
            IsVisible = false,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var backdrop = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0.3,
            IsVisible = false,
            InputTransparent = false
        };
        var backdropTap = new TapGestureRecognizer();
        backdropTap.Tapped += (_, _) => Close();
        backdrop.GestureRecognizers.Add(backdropTap);

        overlay = new Grid
        {
            IsVisible = false,
            InputTransparent = false,
            Children = { backdrop, popupBorder }
        };

        Content = buttonBorder;

        UpdateButtonLabel(SelectedFont);
    }

    public static readonly BindableProperty AvailableFontsProperty = BindableProperty.Create(
        nameof(AvailableFonts),
        typeof(IList<string>),
        typeof(FontPickerButton),
        null,
        propertyChanged: (b, _, n) => ((FontPickerButton)b).picker.AvailableFonts = n as IList<string>);

    public IList<string>? AvailableFonts
    {
        get => (IList<string>?)GetValue(AvailableFontsProperty);
        set => SetValue(AvailableFontsProperty, value);
    }

    public static readonly BindableProperty SelectedFontProperty = BindableProperty.Create(
        nameof(SelectedFont),
        typeof(string),
        typeof(FontPickerButton),
        null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((FontPickerButton)b).OnSelectedFontChanged(n as string));

    public string? SelectedFont
    {
        get => (string?)GetValue(SelectedFontProperty);
        set => SetValue(SelectedFontProperty, value);
    }

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder),
        typeof(string),
        typeof(FontPickerButton),
        "Font",
        propertyChanged: (b, _, _) => ((FontPickerButton)b).UpdateButtonLabel(((FontPickerButton)b).SelectedFont));

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(int),
        typeof(FontPickerButton),
        8,
        propertyChanged: (b, _, n) =>
            ((FontPickerButton)b).buttonBorder.StrokeShape =
                new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = (int)n });

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty FontChangedCommandProperty = BindableProperty.Create(
        nameof(FontChangedCommand),
        typeof(ICommand),
        typeof(FontPickerButton));

    public ICommand? FontChangedCommand
    {
        get => (ICommand?)GetValue(FontChangedCommandProperty);
        set => SetValue(FontChangedCommandProperty, value);
    }

    public event EventHandler<string>? FontChanged;

    void OnButtonTapped(object? sender, TappedEventArgs e)
    {
        if (isOpen)
        {
            Close();
            return;
        }
        Open();
    }

    void Open()
    {
        if (isOpen) return;
        isOpen = true;

        picker.SelectedFont = SelectedFont;

        var page = GetParentPage();
        if (page is ContentPage cp)
        {
            if (cp.Content is Grid grid)
            {
                if (!grid.Children.Contains(overlay))
                    grid.Children.Add(overlay);
            }
            else
            {
                var existing = cp.Content;
                var wrapper = new Grid { Children = { existing, overlay } };
                cp.Content = wrapper;
            }
        }

        overlay.IsVisible = true;
        foreach (var child in overlay.Children)
            ((View)child).IsVisible = true;
        popupBorder.IsVisible = true;
    }

    void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        if (overlay.Parent is Layout parent)
            parent.Remove(overlay);

        overlay.IsVisible = false;
        popupBorder.IsVisible = false;
        foreach (var child in overlay.Children)
            ((View)child).IsVisible = false;
    }

    bool isUpdating;

    void OnPickerFontChanged(object? sender, string font)
    {
        if (isUpdating) return;
        isUpdating = true;

        SetValue(SelectedFontProperty, font);
        UpdateButtonLabel(font);

        FontChanged?.Invoke(this, font);
        if (FontChangedCommand?.CanExecute(font) == true)
            FontChangedCommand.Execute(font);

        isUpdating = false;
    }

    void OnSelectedFontChanged(string? font)
    {
        if (isUpdating) return;
        isUpdating = true;

        UpdateButtonLabel(font);
        picker.SelectedFont = font;

        if (font is not null)
        {
            FontChanged?.Invoke(this, font);
            if (FontChangedCommand?.CanExecute(font) == true)
                FontChangedCommand.Execute(font);
        }

        isUpdating = false;
    }

    void UpdateButtonLabel(string? font)
    {
        if (string.IsNullOrEmpty(font))
        {
            buttonLabel.Text = Placeholder;
            buttonLabel.FontFamily = null;
        }
        else
        {
            buttonLabel.Text = font;
            buttonLabel.FontFamily = font;
        }
    }

    ContentPage? GetParentPage()
    {
        Element? current = this;
        while (current is not null)
        {
            if (current is ContentPage page)
                return page;
            current = current.Parent;
        }
        return null;
    }
}
