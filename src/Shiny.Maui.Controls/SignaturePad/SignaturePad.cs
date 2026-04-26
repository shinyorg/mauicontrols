using System.Collections.ObjectModel;
using Shiny.Maui.Controls.FloatingPanel;

namespace Shiny.Maui.Controls.SignaturePad;

public partial class SignaturePad : ContentView
{
    readonly SignaturePadDrawable drawable;
    readonly GraphicsView graphicsView;
    readonly Button signButton;
    readonly Button cancelButton;
    readonly FloatingPanel.FloatingPanel floatingPanel;
    bool isSyncing;

    public SignaturePad()
    {
        drawable = new SignaturePadDrawable();

        graphicsView = new GraphicsView
        {
            Drawable = drawable,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };
        graphicsView.StartInteraction += OnStartInteraction;
        graphicsView.DragInteraction += OnDragInteraction;
        graphicsView.EndInteraction += OnEndInteraction;

        signButton = new Button
        {
            Text = "Sign",
            BackgroundColor = Colors.Blue,
            TextColor = Colors.White,
            IsEnabled = false,
            CornerRadius = 8,
            HorizontalOptions = LayoutOptions.Fill
        };
        signButton.Clicked += OnSignClicked;

        cancelButton = new Button
        {
            Text = "Cancel",
            BackgroundColor = Colors.Gray,
            TextColor = Colors.White,
            CornerRadius = 8,
            HorizontalOptions = LayoutOptions.Fill
        };
        cancelButton.Clicked += OnCancelClicked;

        var clearButton = new Button
        {
            Text = "Clear",
            FontSize = 12,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.Gray,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Padding = new Thickness(8, 2)
        };
        clearButton.Clicked += OnClearClicked;

        // Canvas area with clear button overlaid
        var canvasGrid = new Grid();
        canvasGrid.Children.Add(graphicsView);
        canvasGrid.Children.Add(clearButton);

        // Button bar
        var buttonBar = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star)
            },
            ColumnSpacing = 10,
            Padding = new Thickness(16, 8, 16, 16)
        };
        buttonBar.Children.Add(cancelButton);
        Grid.SetColumn(cancelButton, 0);
        buttonBar.Children.Add(signButton);
        Grid.SetColumn(signButton, 1);

        var titleLabel = new Label
        {
            Text = "Draw your signature below",
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 12, 0, 4)
        };

        var contentGrid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            },
            Padding = new Thickness(16, 0)
        };
        contentGrid.Children.Add(titleLabel);
        Grid.SetRow(titleLabel, 0);
        contentGrid.Children.Add(canvasGrid);
        Grid.SetRow(canvasGrid, 1);
        contentGrid.Children.Add(buttonBar);
        Grid.SetRow(buttonBar, 2);

        floatingPanel = new FloatingPanel.FloatingPanel
        {
            IsLocked = true,
            HasBackdrop = true,
            CloseOnBackdropTap = false,
            ShowHandle = false,
            PanelBackgroundColor = Colors.White,
            PanelCornerRadius = 16,
            Position = FloatingPanelPosition.Bottom,
            PanelContent = contentGrid,
            Detents = new ObservableCollection<DetentValue> { DetentValue.Half }
        };

        floatingPanel.Opened += (_, _) =>
        {
            if (isSyncing) return;
            isSyncing = true;
            SetValue(IsOpenProperty, true);
            isSyncing = false;
        };

        floatingPanel.Closed += (_, _) =>
        {
            if (isSyncing) return;
            isSyncing = true;
            SetValue(IsOpenProperty, false);
            isSyncing = false;
            ResetCanvas();
        };

        Content = floatingPanel;
    }

    void OnStartInteraction(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length == 0) return;
        drawable.BeginStroke(e.Touches[0]);
        graphicsView.Invalidate();
    }

    void OnDragInteraction(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length == 0) return;
        drawable.AddPoint(e.Touches[0]);
        graphicsView.Invalidate();
    }

    void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        drawable.EndStroke();
        graphicsView.Invalidate();
        signButton.IsEnabled = drawable.HasSignature;
    }

    void OnSignClicked(object? sender, EventArgs e)
    {
        var stream = drawable.ExportToPng(ExportWidth, ExportHeight);
        var args = new SignatureImageEventArgs(stream);

        Signed?.Invoke(this, args);
        if (SignCommand?.CanExecute(args) == true)
            SignCommand.Execute(args);

        ResetCanvas();
        IsOpen = false;
    }

    void OnCancelClicked(object? sender, EventArgs e)
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
        if (CancelCommand?.CanExecute(null) == true)
            CancelCommand.Execute(null);

        ResetCanvas();
        IsOpen = false;
    }

    void OnClearClicked(object? sender, EventArgs e)
    {
        ResetCanvas();
    }

    void ResetCanvas()
    {
        drawable.Clear();
        graphicsView.Invalidate();
        signButton.IsEnabled = false;
    }
}
