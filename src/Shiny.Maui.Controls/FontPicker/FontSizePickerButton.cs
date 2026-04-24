using System.Globalization;
using System.Windows.Input;

namespace Shiny.Maui.Controls.FontPicker;

public class FontSizePickerButton : ContentView
{
    readonly Border buttonBorder;
    readonly Label buttonLabel;
    readonly Border popupBorder;
    readonly FontSizePicker picker;
    readonly Grid overlay;

    bool isOpen;

    public FontSizePickerButton()
    {
        buttonLabel = new Label
        {
            FontSize = 13,
            TextColor = Colors.White,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        buttonBorder = new Border
        {
            StrokeThickness = 2,
            Stroke = Colors.LightGray,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(12, 6),
            MinimumWidthRequest = 60,
            MinimumHeightRequest = 36,
            BackgroundColor = Color.FromArgb("#2C2C2E"),
            Content = buttonLabel,
            HorizontalOptions = LayoutOptions.Start
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnButtonTapped;
        buttonBorder.GestureRecognizers.Add(tap);

        picker = new FontSizePicker
        {
            HeightRequest = 320,
            WidthRequest = 240
        };
        picker.FontSizeChanged += OnPickerFontSizeChanged;

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
        UpdateButtonLabel(SelectedFontSize);
    }

    public static readonly BindableProperty AvailableFontSizesProperty = BindableProperty.Create(
        nameof(AvailableFontSizes),
        typeof(IList<double>),
        typeof(FontSizePickerButton),
        null,
        propertyChanged: (b, _, n) => ((FontSizePickerButton)b).picker.AvailableFontSizes = n as IList<double>);

    public IList<double>? AvailableFontSizes
    {
        get => (IList<double>?)GetValue(AvailableFontSizesProperty);
        set => SetValue(AvailableFontSizesProperty, value);
    }

    public static readonly BindableProperty SelectedFontSizeProperty = BindableProperty.Create(
        nameof(SelectedFontSize),
        typeof(double),
        typeof(FontSizePickerButton),
        16.0,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((FontSizePickerButton)b).OnSelectedFontSizeChanged((double)n));

    public double SelectedFontSize
    {
        get => (double)GetValue(SelectedFontSizeProperty);
        set => SetValue(SelectedFontSizeProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(int),
        typeof(FontSizePickerButton),
        8,
        propertyChanged: (b, _, n) =>
            ((FontSizePickerButton)b).buttonBorder.StrokeShape =
                new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = (int)n });

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty FontSizeChangedCommandProperty = BindableProperty.Create(
        nameof(FontSizeChangedCommand),
        typeof(ICommand),
        typeof(FontSizePickerButton));

    public ICommand? FontSizeChangedCommand
    {
        get => (ICommand?)GetValue(FontSizeChangedCommandProperty);
        set => SetValue(FontSizeChangedCommandProperty, value);
    }

    public event EventHandler<double>? FontSizeChanged;

    void OnButtonTapped(object? sender, TappedEventArgs e)
    {
        if (isOpen) { Close(); return; }
        Open();
    }

    void Open()
    {
        if (isOpen) return;
        isOpen = true;

        picker.SelectedFontSize = SelectedFontSize;

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

    void OnPickerFontSizeChanged(object? sender, double size)
    {
        if (isUpdating) return;
        isUpdating = true;

        SetValue(SelectedFontSizeProperty, size);
        UpdateButtonLabel(size);

        FontSizeChanged?.Invoke(this, size);
        if (FontSizeChangedCommand?.CanExecute(size) == true)
            FontSizeChangedCommand.Execute(size);

        isUpdating = false;
    }

    void OnSelectedFontSizeChanged(double size)
    {
        if (isUpdating) return;
        isUpdating = true;

        UpdateButtonLabel(size);
        picker.SelectedFontSize = size;

        FontSizeChanged?.Invoke(this, size);
        if (FontSizeChangedCommand?.CanExecute(size) == true)
            FontSizeChangedCommand.Execute(size);

        isUpdating = false;
    }

    void UpdateButtonLabel(double size)
    {
        buttonLabel.Text = $"{size.ToString(CultureInfo.InvariantCulture)} pt";
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
