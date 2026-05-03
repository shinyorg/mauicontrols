namespace Shiny.Maui.Controls.Toast;

sealed class ToastView : ContentView
{
    static readonly Color DefaultBackground = Color.FromArgb("#323232");
    static readonly Color DefaultTextColor = Colors.White;
    static readonly Color ProgressBarColor = Color.FromArgb("#FFFFFF");

    readonly ToastConfig config;
    readonly Border border;
    readonly Label label;
    readonly ProgressBar? progressBar;
    readonly ActivityIndicator? spinner;
    readonly Image? icon;
    CancellationTokenSource? autoDismissCts;
    Action? onDismissed;
    bool isDismissing;

    public ToastView(ToastConfig config)
    {
        this.config = config;
        InputTransparent = false;

        var textColor = config.TextColor ?? DefaultTextColor;
        var bgColor = config.BackgroundColor ?? DefaultBackground;

        // Icon
        if (config.Icon is not null)
        {
            icon = new Image
            {
                Source = config.Icon,
                HeightRequest = 20,
                WidthRequest = 20,
                VerticalOptions = LayoutOptions.Center
            };
        }

        // Spinner
        if (config.Spinner != ToastSpinnerPosition.None)
        {
            spinner = new ActivityIndicator
            {
                IsRunning = true,
                Color = textColor,
                HeightRequest = 20,
                WidthRequest = 20,
                VerticalOptions = LayoutOptions.Center
            };
        }

        // Label
        label = new Label
        {
            Text = config.Text,
            TextColor = textColor,
            FontSize = 14,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Fill,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 2
        };

        // Content layout
        var contentLayout = new HorizontalStackLayout
        {
            Spacing = 10,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = config.DisplayMode == ToastDisplayMode.FillHorizontal
                ? LayoutOptions.Start
                : LayoutOptions.Center
        };

        // Add spinner on left
        if (config.Spinner == ToastSpinnerPosition.Left && spinner is not null)
            contentLayout.Children.Add(spinner);

        // Add icon
        if (icon is not null)
            contentLayout.Children.Add(icon);

        // Add label
        contentLayout.Children.Add(label);

        // Add spinner on right
        if (config.Spinner == ToastSpinnerPosition.Right && spinner is not null)
            contentLayout.Children.Add(spinner);

        // Main container
        View innerContent;
        if (config.ShowProgressBar && config.Duration > TimeSpan.Zero)
        {
            progressBar = new ProgressBar
            {
                Progress = 1.0,
                ProgressColor = ProgressBarColor,
                HeightRequest = 2,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0)
            };

            var grid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition(GridLength.Star),
                    new RowDefinition(GridLength.Auto)
                }
            };
            grid.Children.Add(contentLayout);
            Grid.SetRow(contentLayout, 0);
            grid.Children.Add(progressBar);
            Grid.SetRow(progressBar, 1);

            innerContent = grid;
        }
        else
        {
            innerContent = contentLayout;
        }

        // Border (pill or fill)
        var isPill = config.DisplayMode == ToastDisplayMode.Pill;
        border = new Border
        {
            Content = innerContent,
            BackgroundColor = bgColor,
            Padding = new Thickness(16, 10),
            StrokeThickness = config.BorderThickness,
            Stroke = config.BorderColor ?? Colors.Transparent,
            HorizontalOptions = isPill ? LayoutOptions.Center : LayoutOptions.Fill,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
            {
                CornerRadius = isPill ? config.CornerRadius : 0
            },
            Shadow = isPill
                ? new Shadow
                {
                    Brush = Colors.Black,
                    Offset = new Point(0, 2),
                    Radius = 8,
                    Opacity = 0.3f
                }
                : null
        };

        // Tap gesture
        if (config.DismissOnTap || config.TapCommand is not null)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += OnTapped;
            border.GestureRecognizers.Add(tap);
        }

        Content = border;

        // Accessibility
        AutomationProperties.SetName(this, config.Text);
    }

    public void SetOnDismissed(Action callback) => onDismissed = callback;

    public async Task AnimateInAsync()
    {
        var translateY = config.Position == ToastPosition.Bottom ? 80 : -80;
        TranslationY = translateY;
        Opacity = 0;
        IsVisible = true;

        await Task.WhenAll(
            this.TranslateTo(0, 0, 250, Easing.CubicOut),
            this.FadeTo(1, 250, Easing.CubicOut)
        );

        if (config.AnnounceToScreenReader)
        {
            try
            {
                SemanticScreenReader.Announce(config.Text);
            }
            catch
            {
                // May not be available on all platforms
            }
        }

        StartAutoDismiss();
        StartProgressBar();
    }

    public async Task AnimateOutAsync()
    {
        if (isDismissing)
            return;
        isDismissing = true;

        autoDismissCts?.Cancel();

        var translateY = config.Position == ToastPosition.Bottom ? 80 : -80;

        await Task.WhenAll(
            this.TranslateTo(0, translateY, 200, Easing.CubicIn),
            this.FadeTo(0, 200, Easing.CubicIn)
        );

        IsVisible = false;
        onDismissed?.Invoke();
    }

    public async Task AnimatePositionAsync(double newY)
    {
        await this.TranslateTo(TranslationX, newY - Bounds.Y, 150, Easing.CubicOut);
    }

    void OnTapped(object? sender, TappedEventArgs e)
    {
        config.TapCommand?.Execute(null);

        if (config.DismissOnTap)
            _ = AnimateOutAsync();
    }

    void StartAutoDismiss()
    {
        if (config.Duration <= TimeSpan.Zero)
            return;

        autoDismissCts = new CancellationTokenSource();
        var token = autoDismissCts.Token;

        Dispatcher.DispatchDelayed(config.Duration, () =>
        {
            if (!token.IsCancellationRequested)
                _ = AnimateOutAsync();
        });
    }

    void StartProgressBar()
    {
        if (progressBar is null || config.Duration <= TimeSpan.Zero)
            return;

        var animation = new Animation(v => progressBar.Progress = v, 1.0, 0.0);
        animation.Commit(
            this,
            "ProgressCountdown",
            length: (uint)config.Duration.TotalMilliseconds,
            easing: Easing.Linear
        );
    }
}
