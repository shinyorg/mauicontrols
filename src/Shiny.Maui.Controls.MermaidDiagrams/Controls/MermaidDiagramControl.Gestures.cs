namespace Shiny.Maui.Controls.MermaidDiagrams.Controls;

public partial class MermaidDiagramControl
{
    float panStartX;
    float panStartY;
    float pinchStartScale;
    PanGestureRecognizer? panGesture;
    PinchGestureRecognizer? pinchGesture;

    void SetupGestures()
    {
        UpdateGestures();
    }

    void UpdateGestures()
    {
        // Remove existing gesture recognizers
        if (panGesture != null)
        {
            panGesture.PanUpdated -= OnPanUpdated;
            graphicsView.GestureRecognizers.Remove(panGesture);
            panGesture = null;
        }

        if (pinchGesture != null)
        {
            pinchGesture.PinchUpdated -= OnPinchUpdated;
            graphicsView.GestureRecognizers.Remove(pinchGesture);
            pinchGesture = null;
        }

        // Add back based on current settings
        if (AllowPanning)
        {
            panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            graphicsView.GestureRecognizers.Add(panGesture);
        }

        if (AllowZooming)
        {
            pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            graphicsView.GestureRecognizers.Add(pinchGesture);
        }
    }

    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (!AllowPanning)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                panStartX = drawable.OffsetX;
                panStartY = drawable.OffsetY;
                break;

            case GestureStatus.Running:
                drawable.OffsetX = panStartX + (float)e.TotalX;
                drawable.OffsetY = panStartY + (float)e.TotalY;
                graphicsView.Invalidate();
                break;
        }
    }

    void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (!AllowZooming)
            return;

        switch (e.Status)
        {
            case GestureStatus.Started:
                pinchStartScale = drawable.UserScale;
                break;

            case GestureStatus.Running:
                // e.Scale is cumulative from gesture start, so multiply against the starting scale
                var newScale = pinchStartScale * (float)e.Scale;
                newScale = Math.Clamp(newScale, 0.1f, 5f);
                ZoomLevel = newScale;
                break;
        }
    }
}