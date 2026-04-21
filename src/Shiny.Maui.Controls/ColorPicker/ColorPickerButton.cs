using System.Windows.Input;

namespace Shiny.Maui.Controls.ColorPicker;

public class ColorPickerButton : ContentView
{
    readonly Border buttonBorder;
    readonly Label buttonLabel;
    readonly Border popupBorder;
    readonly ColorPicker picker;
    readonly Grid overlay;

    bool isOpen;

    public ColorPickerButton()
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
            MinimumWidthRequest = 44,
            MinimumHeightRequest = 36,
            BackgroundColor = Colors.Red,
            Content = buttonLabel,
            HorizontalOptions = LayoutOptions.Start
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += OnButtonTapped;
        buttonBorder.GestureRecognizers.Add(tap);

        picker = new ColorPicker();
        picker.ColorChanged += OnPickerColorChanged;

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

        // Backdrop to close on outside tap
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

        UpdateButtonColor(SelectedColor);
    }

    // Properties

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
        nameof(SelectedColor),
        typeof(Color),
        typeof(ColorPickerButton),
        Colors.Red,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, _, n) => ((ColorPickerButton)b).OnSelectedColorChanged((Color)n));

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(ColorPickerButton),
        null,
        propertyChanged: (b, _, n) => ((ColorPickerButton)b).buttonLabel.Text = (string?)n);

    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty ShowOpacityProperty = BindableProperty.Create(
        nameof(ShowOpacity),
        typeof(bool),
        typeof(ColorPickerButton),
        false,
        propertyChanged: (b, _, n) => ((ColorPickerButton)b).picker.ShowOpacity = (bool)n);

    public bool ShowOpacity
    {
        get => (bool)GetValue(ShowOpacityProperty);
        set => SetValue(ShowOpacityProperty, value);
    }

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(int),
        typeof(ColorPickerButton),
        8,
        propertyChanged: (b, _, n) =>
            ((ColorPickerButton)b).buttonBorder.StrokeShape =
                new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = (int)n });

    public int CornerRadius
    {
        get => (int)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static readonly BindableProperty ColorChangedCommandProperty = BindableProperty.Create(
        nameof(ColorChangedCommand),
        typeof(ICommand),
        typeof(ColorPickerButton));

    public ICommand? ColorChangedCommand
    {
        get => (ICommand?)GetValue(ColorChangedCommandProperty);
        set => SetValue(ColorChangedCommandProperty, value);
    }

    public event EventHandler<Color>? ColorChanged;

    // Methods

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

        picker.SelectedColor = SelectedColor;

        // Add overlay to the nearest ContentPage's layout
        var page = GetParentPage();
        if (page is ContentPage cp)
        {
            if (cp.Content is Grid grid && !grid.Children.Contains(overlay))
                grid.Children.Add(overlay);
            else if (cp.Content is not Grid)
            {
                // Wrap existing content in a Grid to overlay
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

        overlay.IsVisible = false;
        popupBorder.IsVisible = false;
        foreach (var child in overlay.Children)
            ((View)child).IsVisible = false;
    }

    void OnPickerColorChanged(object? sender, Color color)
    {
        SelectedColor = color;
    }

    void OnSelectedColorChanged(Color color)
    {
        UpdateButtonColor(color);
        picker.SelectedColor = color;

        ColorChanged?.Invoke(this, color);
        if (ColorChangedCommand?.CanExecute(color) == true)
            ColorChangedCommand.Execute(color);
    }

    void UpdateButtonColor(Color color)
    {
        buttonBorder.BackgroundColor = color;

        // Auto contrast: use white text on dark colors, black on light
        var luminance = 0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue;
        buttonLabel.TextColor = luminance < 0.5 ? Colors.White : Colors.Black;
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
