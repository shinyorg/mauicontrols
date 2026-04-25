using System.Windows.Input;

namespace Shiny.Maui.Controls.ColorPicker;

public class ColorPickerButton : ContentView
{
    readonly Border buttonBorder;
    readonly Label buttonLabel;
    readonly ColorPicker picker;
    readonly Grid overlayGrid;

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

        var popupBorder = new Border
        {
            StrokeThickness = 1,
            Stroke = Colors.LightGray,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 12 },
            BackgroundColor = Colors.White,
            Padding = 0,
            Content = new VerticalStackLayout
            {
                Children = { picker, doneButton }
            },
            Shadow = new Shadow
            {
                Brush = new SolidColorBrush(Colors.Black),
                Offset = new Point(0, 4),
                Radius = 12,
                Opacity = 0.25f
            },
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var backdrop = new BoxView
        {
            Color = Colors.Black,
            Opacity = 0.3,
            InputTransparent = false
        };
        var backdropTap = new TapGestureRecognizer();
        backdropTap.Tapped += (_, _) => Close();
        backdrop.GestureRecognizers.Add(backdropTap);

        overlayGrid = new Grid
        {
            InputTransparent = false,
            CascadeInputTransparent = false,
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

        // Inject the overlay into the page's content
        var page = FindPage();
        if (page is not ContentPage cp) return;

        if (cp.Content is Grid grid)
        {
            grid.Children.Add(overlayGrid);
        }
        else
        {
            var original = cp.Content;
            var wrapper = new Grid();
            cp.Content = wrapper;
            if (original != null)
                wrapper.Children.Add(original);
            wrapper.Children.Add(overlayGrid);
        }
    }

    void Close()
    {
        if (!isOpen) return;
        isOpen = false;

        // Remove the overlay — clean break, no stale state
        if (overlayGrid.Parent is Layout parent)
        {
            parent.Children.Remove(overlayGrid);

            // Unwrap if we created a wrapper Grid
            if (parent is Grid wrapper && parent != overlayGrid
                && wrapper.Children.Count == 1
                && FindPage() is ContentPage cp && cp.Content == wrapper)
            {
                var original = wrapper.Children[0];
                wrapper.Children.Clear();
                cp.Content = (View)original;
            }
        }
    }

    bool isUpdatingColor;

    void OnPickerColorChanged(object? sender, Color color)
    {
        if (isUpdatingColor) return;
        isUpdatingColor = true;

        SetValue(SelectedColorProperty, color);
        UpdateButtonColor(color);

        ColorChanged?.Invoke(this, color);
        if (ColorChangedCommand?.CanExecute(color) == true)
            ColorChangedCommand.Execute(color);

        isUpdatingColor = false;
    }

    void OnSelectedColorChanged(Color color)
    {
        if (isUpdatingColor) return;
        isUpdatingColor = true;

        UpdateButtonColor(color);
        picker.SelectedColor = color;

        ColorChanged?.Invoke(this, color);
        if (ColorChangedCommand?.CanExecute(color) == true)
            ColorChangedCommand.Execute(color);

        isUpdatingColor = false;
    }

    void UpdateButtonColor(Color color)
    {
        buttonBorder.BackgroundColor = color;

        var luminance = 0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue;
        buttonLabel.TextColor = luminance < 0.5 ? Colors.White : Colors.Black;
    }

    Page? FindPage()
    {
        Element? current = this;
        while (current is not null)
        {
            if (current is Page page)
                return page;
            current = current.Parent;
        }
        return null;
    }
}
