namespace Shiny.Maui.Controls.ImageEditor;

public partial class ImageEditor
{
    const float MinZoom = 1f;
    const float MaxZoom = 5f;
    const float CropHandleHitRadius = 24f;

    float startScale = 1f;
    PointF touchStartPoint;
    float viewStartX, viewStartY;
    CropHandle activeCropHandle = CropHandle.None;
    RectF cropStartRect;
    bool isPanning;

    void SetupGestures()
    {
        var pinch = new PinchGestureRecognizer();
        pinch.PinchUpdated += OnPinchUpdated;
        graphicsView.GestureRecognizers.Add(pinch);

        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        graphicsView.GestureRecognizers.Add(pan);

        var tap = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
        tap.Tapped += OnTapped;
        graphicsView.GestureRecognizers.Add(tap);
    }

    void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (!AllowZoom || CurrentToolMode != ImageEditorToolMode.None)
            return;

        switch (e.Status)
        {
            case GestureStatus.Started:
                startScale = drawable.ViewScale;
                break;

            case GestureStatus.Running:
                drawable.ViewScale = Math.Clamp(
                    startScale * (float)e.Scale,
                    MinZoom,
                    MaxZoom);
                Invalidate();
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (drawable.ViewScale <= MinZoom + 0.05f)
                    ResetViewTransform();
                Invalidate();
                break;
        }
    }

    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                OnPanStart();
                break;

            case GestureStatus.Running:
                OnPanMove((float)e.TotalX, (float)e.TotalY);
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                OnPanEnd();
                break;
        }
    }

    void OnPanStart()
    {
        isPanning = true;

        switch (CurrentToolMode)
        {
            case ImageEditorToolMode.None:
                viewStartX = drawable.ViewOffsetX;
                viewStartY = drawable.ViewOffsetY;
                break;

            case ImageEditorToolMode.Crop:
                // We need to estimate start point for crop; use center of image as fallback.
                // The actual handle detection happens with the first delta.
                touchStartPoint = PointF.Zero;
                activeCropHandle = CropHandle.None;
                break;

            case ImageEditorToolMode.Draw:
                drawable.ActiveStrokePoints = [];
                break;
        }
    }

    void OnPanMove(float totalX, float totalY)
    {
        switch (CurrentToolMode)
        {
            case ImageEditorToolMode.None:
                if (drawable.ViewScale > MinZoom + 0.05f)
                {
                    drawable.ViewOffsetX = viewStartX + totalX;
                    drawable.ViewOffsetY = viewStartY + totalY;
                    Invalidate();
                }
                break;

            case ImageEditorToolMode.Crop:
                HandleCropPan(totalX, totalY);
                break;

            case ImageEditorToolMode.Draw:
                HandleDrawPan(totalX, totalY);
                break;
        }
    }

    void OnPanEnd()
    {
        isPanning = false;

        switch (CurrentToolMode)
        {
            case ImageEditorToolMode.Crop:
                activeCropHandle = CropHandle.None;
                break;

            case ImageEditorToolMode.Draw:
                CommitCurrentStroke();
                break;
        }
    }

    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (CurrentToolMode != ImageEditorToolMode.Text)
            return;

        var point = e.GetPosition(graphicsView);
        if (point.HasValue)
            HandleTextPlacement(point.Value);
    }

    #region Crop Interaction

    void HandleCropPan(float totalX, float totalY)
    {
        if (!drawable.ActiveCropRect.HasValue)
            return;

        var imageRect = drawable.GetImageRect();
        if (imageRect is not { Width: > 0, Height: > 0 })
            return;

        // On first move, determine which handle to drag based on the center of the crop
        if (activeCropHandle == CropHandle.None)
        {
            cropStartRect = drawable.ActiveCropRect.Value;

            // Use the crop center as the assumed start point
            var cropPixel = new RectF(
                imageRect.X + cropStartRect.X * imageRect.Width,
                imageRect.Y + cropStartRect.Y * imageRect.Height,
                cropStartRect.Width * imageRect.Width,
                cropStartRect.Height * imageRect.Height
            );

            // Default to bottom-right handle for initial drags (shrinking from full image)
            activeCropHandle = CropHandle.BottomRight;

            // Try to detect which edge/corner is being dragged based on delta direction
            if (totalX < -5 && totalY < -5) activeCropHandle = CropHandle.BottomRight;
            else if (totalX > 5 && totalY > 5) activeCropHandle = CropHandle.TopLeft;
            else if (totalX < -5 && totalY > 5) activeCropHandle = CropHandle.BottomLeft;
            else if (totalX > 5 && totalY < -5) activeCropHandle = CropHandle.TopRight;
            else activeCropHandle = CropHandle.Move;
        }

        // Delta in normalized coords
        var dx = totalX / imageRect.Width;
        var dy = totalY / imageRect.Height;

        var crop = cropStartRect;
        RectF newCrop;

        switch (activeCropHandle)
        {
            case CropHandle.Move:
                newCrop = new RectF(
                    Math.Clamp(crop.X + dx, 0, 1 - crop.Width),
                    Math.Clamp(crop.Y + dy, 0, 1 - crop.Height),
                    crop.Width,
                    crop.Height);
                break;

            case CropHandle.TopLeft:
                newCrop = ResizeCrop(crop, dx, dy, 0, 0);
                break;
            case CropHandle.TopCenter:
                newCrop = ResizeCrop(crop, 0, dy, 0, 0);
                break;
            case CropHandle.TopRight:
                newCrop = ResizeCrop(crop, 0, dy, dx, 0);
                break;
            case CropHandle.MiddleLeft:
                newCrop = ResizeCrop(crop, dx, 0, 0, 0);
                break;
            case CropHandle.MiddleRight:
                newCrop = ResizeCrop(crop, 0, 0, dx, 0);
                break;
            case CropHandle.BottomLeft:
                newCrop = ResizeCrop(crop, dx, 0, 0, dy);
                break;
            case CropHandle.BottomCenter:
                newCrop = ResizeCrop(crop, 0, 0, 0, dy);
                break;
            case CropHandle.BottomRight:
                newCrop = ResizeCrop(crop, 0, 0, dx, dy);
                break;
            default:
                return;
        }

        drawable.ActiveCropRect = newCrop;
        Invalidate();
    }

    static RectF ResizeCrop(RectF crop, float dLeft, float dTop, float dRight, float dBottom)
    {
        const float minSize = 0.05f;

        var x = crop.X + dLeft;
        var y = crop.Y + dTop;
        var w = crop.Width - dLeft + dRight;
        var h = crop.Height - dTop + dBottom;

        if (w < minSize) { w = minSize; x = crop.X + crop.Width - minSize; }
        if (h < minSize) { h = minSize; y = crop.Y + crop.Height - minSize; }

        x = Math.Clamp(x, 0, 1 - minSize);
        y = Math.Clamp(y, 0, 1 - minSize);
        w = Math.Min(w, 1 - x);
        h = Math.Min(h, 1 - y);

        return new RectF(x, y, w, h);
    }

    #endregion

    #region Draw Interaction

    void HandleDrawPan(float totalX, float totalY)
    {
        if (drawable.ActiveStrokePoints == null)
            return;

        // PanGestureRecognizer gives TotalX/TotalY relative to gesture start.
        // We approximate absolute position using the center of the GraphicsView as origin,
        // offset by the pan delta. This works because GraphicsView fills the control.
        var imageRect = drawable.GetImageRect();
        if (imageRect is not { Width: > 0, Height: > 0 })
            return;

        // For drawing, we store the point directly
        // First point establishes origin, subsequent points are relative
        if (drawable.ActiveStrokePoints.Count == 0)
        {
            // Store the first point at image center + delta
            var startX = imageRect.Center.X + totalX;
            var startY = imageRect.Center.Y + totalY;
            drawable.ActiveStrokePoints.Add(new PointF(startX, startY));
        }
        else
        {
            // Compute new point relative to first point's delta
            var firstPoint = drawable.ActiveStrokePoints[0];
            var newPoint = new PointF(
                firstPoint.X + (totalX - (drawable.ActiveStrokePoints.Count > 1 ? 0 : 0)),
                firstPoint.Y + (totalY - (drawable.ActiveStrokePoints.Count > 1 ? 0 : 0))
            );

            // Actually, we need to track the origin offset
            // Since TotalX/Y are cumulative from start, each point is origin + totalDelta
            var origin = drawable.ActiveStrokePoints[0];
            // Recalculate: origin was placed at some position, and totalX/Y grows
            // The first call had some totalX/Y, so all subsequent points are:
            // origin + (currentTotal - firstTotal)
            // But we don't have firstTotal. Instead, add point at each new total:
            if (drawable.ActiveStrokePoints.Count == 1)
            {
                // Store the reference totalX/Y with the origin
                touchStartPoint = new PointF(totalX, totalY);
            }

            var px = origin.X + (totalX - touchStartPoint.X);
            var py = origin.Y + (totalY - touchStartPoint.Y);
            drawable.ActiveStrokePoints.Add(new PointF(px, py));
        }

        Invalidate();
    }

    #endregion

    #region Text Placement

    async void HandleTextPlacement(PointF point)
    {
        var imageRect = drawable.GetImageRect();
        if (imageRect is not { Width: > 0, Height: > 0 })
            return;

        if (!imageRect.Contains(point))
            return;

        var text = await PromptForTextAsync();
        if (string.IsNullOrWhiteSpace(text))
            return;

        var normalized = new PointF(
            (point.X - imageRect.X) / imageRect.Width,
            (point.Y - imageRect.Y) / imageRect.Height
        );

        state.Push(new EditActions.TextAnnotationAction
        {
            Text = text,
            Position = normalized,
            FontSize = (float)TextFontSize,
            TextColor = AnnotationTextColor
        });
    }

    Task<string?> PromptForTextAsync()
    {
        var page = GetParentPage();
        if (page == null)
            return Task.FromResult<string?>(null);

        return page.DisplayPromptAsync("Add Text", "Enter annotation text:");
    }

    Page? GetParentPage()
    {
        Element? current = this;
        while (current != null)
        {
            if (current is Page page)
                return page;
            current = current.Parent;
        }
        return null;
    }

    #endregion

    #region Stroke Commit

    void CommitCurrentStroke()
    {
        if (drawable.ActiveStrokePoints is not { Count: >= 2 })
        {
            drawable.ActiveStrokePoints = null;
            return;
        }

        var imageRect = drawable.GetImageRect();
        if (imageRect is not { Width: > 0, Height: > 0 })
        {
            drawable.ActiveStrokePoints = null;
            return;
        }

        var normalized = drawable.ActiveStrokePoints
            .Select(p => new PointF(
                (p.X - imageRect.X) / imageRect.Width,
                (p.Y - imageRect.Y) / imageRect.Height))
            .ToArray();

        state.Push(new EditActions.DrawStrokeAction
        {
            Points = normalized,
            StrokeColor = DrawStrokeColor,
            StrokeWidth = (float)DrawStrokeWidth
        });

        drawable.ActiveStrokePoints = null;
        Invalidate();
    }

    #endregion

    enum CropHandle
    {
        None,
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleRight,
        BottomLeft, BottomCenter, BottomRight,
        Move
    }
}
