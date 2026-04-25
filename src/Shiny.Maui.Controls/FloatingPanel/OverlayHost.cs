namespace Shiny.Maui.Controls.FloatingPanel;

public class OverlayHost : Grid
{
    const double DefaultBackdropMaxOpacity = 0.5;

    readonly BoxView backdrop;
    readonly List<FloatingPanel> activePanels = new();

    public OverlayHost()
    {
        InputTransparent = true;
        CascadeInputTransparent = false;

        backdrop = new BoxView
        {
            Color = BackdropColor,
            Opacity = 0,
            IsVisible = false,
            InputTransparent = true
        };
        var tap = new TapGestureRecognizer();
        tap.Tapped += OnBackdropTapped;
        backdrop.GestureRecognizers.Add(tap);
        Children.Add(backdrop);
    }

    public static readonly BindableProperty BackdropColorProperty = BindableProperty.Create(
        nameof(BackdropColor),
        typeof(Color),
        typeof(OverlayHost),
        Colors.Black,
        propertyChanged: (b, _, n) => ((OverlayHost)b).backdrop.Color = (Color)n);

    public Color BackdropColor
    {
        get => (Color)GetValue(BackdropColorProperty);
        set => SetValue(BackdropColorProperty, value);
    }

    public static readonly BindableProperty BackdropMaxOpacityProperty = BindableProperty.Create(
        nameof(BackdropMaxOpacity),
        typeof(double),
        typeof(OverlayHost),
        DefaultBackdropMaxOpacity);

    public double BackdropMaxOpacity
    {
        get => (double)GetValue(BackdropMaxOpacityProperty);
        set => SetValue(BackdropMaxOpacityProperty, value);
    }

    internal async void ShowBackdrop(FloatingPanel panel, uint animationDuration)
    {
        if (!activePanels.Contains(panel))
            activePanels.Add(panel);

        backdrop.InputTransparent = false;
        backdrop.IsVisible = true;
        await backdrop.FadeToAsync(BackdropMaxOpacity, animationDuration);
    }

    internal async void HideBackdrop(FloatingPanel panel, uint animationDuration)
    {
        activePanels.Remove(panel);

        if (activePanels.Count > 0)
            return;

        await backdrop.FadeToAsync(0, animationDuration);
        backdrop.IsVisible = false;
        backdrop.InputTransparent = true;
    }

    void OnBackdropTapped(object? sender, TappedEventArgs e)
    {
        // Close all panels that allow backdrop tap close — locked panels are only dismissable via code
        foreach (var panel in activePanels.ToList())
        {
            if (panel.CloseOnBackdropTap && !panel.IsLocked)
                panel.IsOpen = false;
        }
    }
}
