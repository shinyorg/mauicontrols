using System.Collections.ObjectModel;
using Shiny.Maui.Controls.FloatingPanel;

namespace Shiny.Maui.Controls.SignaturePad;

public partial class SignaturePad
{
    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
        nameof(IsOpen),
        typeof(bool),
        typeof(SignaturePad),
        false,
        BindingMode.TwoWay,
        propertyChanged: OnIsOpenChanged);

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public static readonly BindableProperty PositionProperty = BindableProperty.Create(
        nameof(Position),
        typeof(FloatingPanelPosition),
        typeof(SignaturePad),
        FloatingPanelPosition.Bottom,
        propertyChanged: (b, _, n) => ((SignaturePad)b).floatingPanel.Position = (FloatingPanelPosition)n);

    public FloatingPanelPosition Position
    {
        get => (FloatingPanelPosition)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public static readonly BindableProperty IsLockedProperty = BindableProperty.Create(
        nameof(IsLocked),
        typeof(bool),
        typeof(SignaturePad),
        true,
        propertyChanged: (b, _, n) => ((SignaturePad)b).floatingPanel.IsLocked = (bool)n);

    public bool IsLocked
    {
        get => (bool)GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }

    public static readonly BindableProperty DetentProperty = BindableProperty.Create(
        nameof(Detent),
        typeof(DetentValue),
        typeof(SignaturePad),
        DetentValue.Half,
        propertyChanged: (b, _, n) =>
        {
            var pad = (SignaturePad)b;
            pad.floatingPanel.Detents = new ObservableCollection<DetentValue> { (DetentValue)n };
        });

    public DetentValue Detent
    {
        get => (DetentValue)GetValue(DetentProperty);
        set => SetValue(DetentProperty, value);
    }

    public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(
        nameof(StrokeColor),
        typeof(Color),
        typeof(SignaturePad),
        Colors.Black,
        propertyChanged: (b, _, n) =>
        {
            var pad = (SignaturePad)b;
            pad.drawable.StrokeColor = (Color)n;
            pad.graphicsView.Invalidate();
        });

    public Color StrokeColor
    {
        get => (Color)GetValue(StrokeColorProperty);
        set => SetValue(StrokeColorProperty, value);
    }

    public static readonly BindableProperty SignatureBackgroundColorProperty = BindableProperty.Create(
        nameof(SignatureBackgroundColor),
        typeof(Color),
        typeof(SignaturePad),
        Colors.White,
        propertyChanged: (b, _, n) =>
        {
            var pad = (SignaturePad)b;
            pad.drawable.BackgroundColor = (Color)n;
            pad.graphicsView.Invalidate();
        });

    public Color SignatureBackgroundColor
    {
        get => (Color)GetValue(SignatureBackgroundColorProperty);
        set => SetValue(SignatureBackgroundColorProperty, value);
    }

    public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(
        nameof(StrokeWidth),
        typeof(double),
        typeof(SignaturePad),
        3.0,
        propertyChanged: (b, _, n) =>
        {
            var pad = (SignaturePad)b;
            pad.drawable.StrokeWidth = (float)(double)n;
            pad.graphicsView.Invalidate();
        });

    public double StrokeWidth
    {
        get => (double)GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }

    public static readonly BindableProperty SignButtonTextProperty = BindableProperty.Create(
        nameof(SignButtonText),
        typeof(string),
        typeof(SignaturePad),
        "Sign",
        propertyChanged: (b, _, n) => ((SignaturePad)b).signButton.Text = (string)n);

    public string SignButtonText
    {
        get => (string)GetValue(SignButtonTextProperty);
        set => SetValue(SignButtonTextProperty, value);
    }

    public static readonly BindableProperty CancelButtonTextProperty = BindableProperty.Create(
        nameof(CancelButtonText),
        typeof(string),
        typeof(SignaturePad),
        "Cancel",
        propertyChanged: (b, _, n) => ((SignaturePad)b).cancelButton.Text = (string)n);

    public string CancelButtonText
    {
        get => (string)GetValue(CancelButtonTextProperty);
        set => SetValue(CancelButtonTextProperty, value);
    }

    public static readonly BindableProperty SignButtonColorProperty = BindableProperty.Create(
        nameof(SignButtonColor),
        typeof(Color),
        typeof(SignaturePad),
        Colors.Blue,
        propertyChanged: (b, _, n) => ((SignaturePad)b).signButton.BackgroundColor = (Color)n);

    public Color SignButtonColor
    {
        get => (Color)GetValue(SignButtonColorProperty);
        set => SetValue(SignButtonColorProperty, value);
    }

    public static readonly BindableProperty CancelButtonColorProperty = BindableProperty.Create(
        nameof(CancelButtonColor),
        typeof(Color),
        typeof(SignaturePad),
        Colors.Gray,
        propertyChanged: (b, _, n) => ((SignaturePad)b).cancelButton.BackgroundColor = (Color)n);

    public Color CancelButtonColor
    {
        get => (Color)GetValue(CancelButtonColorProperty);
        set => SetValue(CancelButtonColorProperty, value);
    }

    public static readonly BindableProperty ShowCancelButtonProperty = BindableProperty.Create(
        nameof(ShowCancelButton),
        typeof(bool),
        typeof(SignaturePad),
        true,
        propertyChanged: (b, _, n) => ((SignaturePad)b).cancelButton.IsVisible = (bool)n);

    public bool ShowCancelButton
    {
        get => (bool)GetValue(ShowCancelButtonProperty);
        set => SetValue(ShowCancelButtonProperty, value);
    }

    public static readonly BindableProperty PanelBackgroundColorProperty = BindableProperty.Create(
        nameof(PanelBackgroundColor),
        typeof(Color),
        typeof(SignaturePad),
        Colors.White,
        propertyChanged: (b, _, n) => ((SignaturePad)b).floatingPanel.PanelBackgroundColor = (Color)n);

    public Color PanelBackgroundColor
    {
        get => (Color)GetValue(PanelBackgroundColorProperty);
        set => SetValue(PanelBackgroundColorProperty, value);
    }

    public static readonly BindableProperty PanelCornerRadiusProperty = BindableProperty.Create(
        nameof(PanelCornerRadius),
        typeof(double),
        typeof(SignaturePad),
        16.0,
        propertyChanged: (b, _, n) => ((SignaturePad)b).floatingPanel.PanelCornerRadius = (double)n);

    public double PanelCornerRadius
    {
        get => (double)GetValue(PanelCornerRadiusProperty);
        set => SetValue(PanelCornerRadiusProperty, value);
    }

    public static readonly BindableProperty HasBackdropProperty = BindableProperty.Create(
        nameof(HasBackdrop),
        typeof(bool),
        typeof(SignaturePad),
        true,
        propertyChanged: (b, _, n) => ((SignaturePad)b).floatingPanel.HasBackdrop = (bool)n);

    public bool HasBackdrop
    {
        get => (bool)GetValue(HasBackdropProperty);
        set => SetValue(HasBackdropProperty, value);
    }

    public static readonly BindableProperty ExportWidthProperty = BindableProperty.Create(
        nameof(ExportWidth),
        typeof(int),
        typeof(SignaturePad),
        600);

    public int ExportWidth
    {
        get => (int)GetValue(ExportWidthProperty);
        set => SetValue(ExportWidthProperty, value);
    }

    public static readonly BindableProperty ExportHeightProperty = BindableProperty.Create(
        nameof(ExportHeight),
        typeof(int),
        typeof(SignaturePad),
        200);

    public int ExportHeight
    {
        get => (int)GetValue(ExportHeightProperty);
        set => SetValue(ExportHeightProperty, value);
    }

    public static readonly BindableProperty SignCommandProperty = BindableProperty.Create(
        nameof(SignCommand),
        typeof(System.Windows.Input.ICommand),
        typeof(SignaturePad));

    public System.Windows.Input.ICommand? SignCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(SignCommandProperty);
        set => SetValue(SignCommandProperty, value);
    }

    public static readonly BindableProperty CancelCommandProperty = BindableProperty.Create(
        nameof(CancelCommand),
        typeof(System.Windows.Input.ICommand),
        typeof(SignaturePad));

    public System.Windows.Input.ICommand? CancelCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    // Events
    public event EventHandler<SignatureImageEventArgs>? Signed;
    public event EventHandler? Cancelled;

    static void OnIsOpenChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var pad = (SignaturePad)bindable;
        if (pad.isSyncing) return;
        pad.isSyncing = true;
        pad.floatingPanel.IsOpen = (bool)newValue;
        pad.isSyncing = false;
    }
}
