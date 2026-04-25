namespace Shiny.Maui.Controls.ImageEditor;

public partial class ImageEditor
{
    const double MinScale = 1.0;
    const double MaxScale = 5.0;
    const float CropHandleHitRadius = 24f;

    readonly PinchGestureRecognizer pinchGesture = new();
    readonly PanGestureRecognizer panGesture = new();

    double currentScale = 1;
    double startScale = 1;
    double xOffset, yOffset;
    double startX, startY;
    bool isPinching;

    PointF touchStartPoint;
    CropHandle activeCropHandle = CropHandle.None;
    RectF cropStartRect;
    bool isDragging;

    void SetupGestures()
    {
        pinchGesture.PinchUpdated += OnPinchUpdated;
        panGesture.PanUpdated += OnPanUpdated;

        var doubleTap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        doubleTap.Tapped += OnDoubleTapped;
        graphicsView.GestureRecognizers.Add(doubleTap);

        var tap = new TapGestureRecognizer { NumberOfTapsRequired = 1 };
        tap.Tapped += OnTapped;
        graphicsView.GestureRecognizers.Add(tap);
    }

    void EnableMoveGestures()
    {
        // Remove touch interaction handlers — they consume touches before gesture recognizers
        DisableTouchInteraction();

        if (!graphicsView.GestureRecognizers.Contains(pinchGesture))
            graphicsView.GestureRecognizers.Add(pinchGesture);
    }

    void DisableMoveGestures()
    {
        graphicsView.GestureRecognizers.Remove(pinchGesture);
        graphicsView.GestureRecognizers.Remove(panGesture);

        if (currentScale > MinScale)
            _ = AnimateResetZoomAsync();
    }

    void EnableTouchInteraction()
    {
        graphicsView.StartInteraction += OnStartInteraction;
        graphicsView.DragInteraction += OnDragInteraction;
        graphicsView.EndInteraction += OnEndInteraction;
    }

    void DisableTouchInteraction()
    {
        graphicsView.StartInteraction -= OnStartInteraction;
        graphicsView.DragInteraction -= OnDragInteraction;
        graphicsView.EndInteraction -= OnEndInteraction;
    }

    #region Pinch Zoom (Move mode — native view transforms like ImageViewer)

    void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (CurrentToolMode != ImageEditorToolMode.Move) return;

        switch (e.Status)
        {
            case GestureStatus.Started:
                isPinching = true;
                startScale = currentScale;
                break;

            case GestureStatus.Running:
                currentScale += (e.Scale - 1) * startScale;
                currentScale = Math.Clamp(currentScale, MinScale, MaxScale);

                var pinchX = (e.ScaleOrigin.X - 0.5) * graphicsView.Width;
                var pinchY = (e.ScaleOrigin.Y - 0.5) * graphicsView.Height;
                var scaleDelta = currentScale - startScale;

                var targetX = xOffset - pinchX * scaleDelta;
                var targetY = yOffset - pinchY * scaleDelta;

                graphicsView.TranslationX = ClampX(targetX);
                graphicsView.TranslationY = ClampY(targetY);
                graphicsView.Scale = currentScale;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                isPinching = false;
                xOffset = graphicsView.TranslationX;
                yOffset = graphicsView.TranslationY;

                if (currentScale <= MinScale)
                    _ = AnimateResetZoomAsync();
                else if (!graphicsView.GestureRecognizers.Contains(panGesture))
                    graphicsView.GestureRecognizers.Add(panGesture);
                break;
        }
    }

    void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (isPinching || currentScale <= MinScale || CurrentToolMode != ImageEditorToolMode.Move)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                startX = xOffset;
                startY = yOffset;
                break;

            case GestureStatus.Running:
                graphicsView.TranslationX = ClampX(startX + e.TotalX);
                graphicsView.TranslationY = ClampY(startY + e.TotalY);
                break;

            case GestureStatus.Completed:
                xOffset = graphicsView.TranslationX;
                yOffset = graphicsView.TranslationY;
                break;
        }
    }

    void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (CurrentToolMode != ImageEditorToolMode.Move) return;

        if (currentScale > MinScale)
            _ = AnimateResetZoomAsync();
        else
            _ = AnimateZoomInAsync(e);
    }

    async Task AnimateZoomInAsync(TappedEventArgs e)
    {
        var targetScale = Math.Min(2.5, MaxScale);
        var point = e.GetPosition(graphicsView);

        double tx = 0, ty = 0;
        if (point.HasValue)
        {
            tx = -(point.Value.X - graphicsView.Width / 2) * (targetScale - 1);
            ty = -(point.Value.Y - graphicsView.Height / 2) * (targetScale - 1);
        }

        currentScale = targetScale;
        tx = ClampX(tx);
        ty = ClampY(ty);
        xOffset = tx;
        yOffset = ty;

        await Task.WhenAll(
            graphicsView.ScaleTo(targetScale, 250, Easing.CubicOut),
            graphicsView.TranslateTo(tx, ty, 250, Easing.CubicOut)
        );

        if (!graphicsView.GestureRecognizers.Contains(panGesture))
            graphicsView.GestureRecognizers.Add(panGesture);
    }

    async Task AnimateResetZoomAsync()
    {
        graphicsView.GestureRecognizers.Remove(panGesture);

        await Task.WhenAll(
            graphicsView.ScaleTo(1, 250, Easing.CubicOut),
            graphicsView.TranslateTo(0, 0, 250, Easing.CubicOut)
        );

        currentScale = 1;
        xOffset = 0;
        yOffset = 0;
    }

    double ClampX(double x)
    {
        if (currentScale <= MinScale) return 0;
        var maxX = graphicsView.Width * (currentScale - 1) / 2;
        return Math.Clamp(x, -maxX, maxX);
    }

    double ClampY(double y)
    {
        if (currentScale <= MinScale) return 0;
        var maxY = graphicsView.Height * (currentScale - 1) / 2;
        return Math.Clamp(y, -maxY, maxY);
    }

    #endregion

    #region Touch Interaction — Draw, Crop, Text (Start/Drag/End)

    void OnStartInteraction(object? sender, TouchEventArgs e)
    {
        var point = e.Touches[0];

        // Text mode — place text entry at touch point
        if (CurrentToolMode == ImageEditorToolMode.Text)
        {
            HandleTextPlacement(new PointF(point.X, point.Y));
            return;
        }

        if (CurrentToolMode != ImageEditorToolMode.Draw &&
            CurrentToolMode != ImageEditorToolMode.Crop &&
            CurrentToolMode != ImageEditorToolMode.Line &&
            CurrentToolMode != ImageEditorToolMode.Arrow)
            return;

        touchStartPoint = point;
        isDragging = true;

        switch (CurrentToolMode)
        {
            case ImageEditorToolMode.Crop when drawable.ActiveCropRect.HasValue:
                cropStartRect = drawable.ActiveCropRect.Value;
                activeCropHandle = HitTestCropHandle(point);
                break;

            case ImageEditorToolMode.Draw:
            {
                var imageRect = drawable.GetImageRect();
                if (imageRect is not { Width: > 0, Height: > 0 } || !imageRect.Contains(point))
                    return;
                drawable.ActiveStrokePoints = [point];
                Invalidate();
                break;
            }

            case ImageEditorToolMode.Line:
            case ImageEditorToolMode.Arrow:
            {
                var imageRect = drawable.GetImageRect();
                if (imageRect is not { Width: > 0, Height: > 0 } || !imageRect.Contains(point))
                    return;
                drawable.ActiveLineStart = point;
                drawable.ActiveLineEnd = point;
                drawable.ActiveLineIsArrow = CurrentToolMode == ImageEditorToolMode.Arrow;
                Invalidate();
                break;
            }
        }
    }

    void OnDragInteraction(object? sender, TouchEventArgs e)
    {
        if (!isDragging) return;

        var point = e.Touches[0];

        switch (CurrentToolMode)
        {
            case ImageEditorToolMode.Crop when activeCropHandle != CropHandle.None:
                HandleCropDrag(point);
                break;

            case ImageEditorToolMode.Draw when drawable.ActiveStrokePoints != null:
            {
                var imageRect = drawable.GetImageRect();
                if (imageRect is { Width: > 0, Height: > 0 })
                {
                    // Clamp point to image bounds
                    var clamped = new PointF(
                        Math.Clamp(point.X, imageRect.X, imageRect.Right),
                        Math.Clamp(point.Y, imageRect.Y, imageRect.Bottom));
                    drawable.ActiveStrokePoints.Add(clamped);
                }
                Invalidate();
                break;
            }

            case ImageEditorToolMode.Line:
            case ImageEditorToolMode.Arrow:
            {
                if (drawable.ActiveLineStart.HasValue)
                {
                    var imageRect = drawable.GetImageRect();
                    if (imageRect is { Width: > 0, Height: > 0 })
                    {
                        var clamped = new PointF(
                            Math.Clamp(point.X, imageRect.X, imageRect.Right),
                            Math.Clamp(point.Y, imageRect.Y, imageRect.Bottom));
                        drawable.ActiveLineEnd = clamped;
                    }
                    Invalidate();
                }
                break;
            }
        }
    }

    void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        isDragging = false;

        switch (CurrentToolMode)
        {
            case ImageEditorToolMode.Crop:
                activeCropHandle = CropHandle.None;
                break;

            case ImageEditorToolMode.Draw:
                CommitCurrentStroke();
                break;

            case ImageEditorToolMode.Line:
            case ImageEditorToolMode.Arrow:
                CommitCurrentLine();
                break;
        }
    }

    void CommitCurrentLine()
    {
        if (drawable.ActiveLineStart is not { } start || drawable.ActiveLineEnd is not { } end)
        {
            drawable.ActiveLineStart = null;
            drawable.ActiveLineEnd = null;
            return;
        }

        var imageRect = drawable.GetImageRect();
        if (imageRect is { Width: > 0, Height: > 0 })
        {
            var dx = end.X - start.X;
            var dy = end.Y - start.Y;
            // Ignore taps without drag
            if (dx * dx + dy * dy >= 4f)
            {
                state.Push(new EditActions.LineAction
                {
                    Start = new PointF((start.X - imageRect.X) / imageRect.Width, (start.Y - imageRect.Y) / imageRect.Height),
                    End = new PointF((end.X - imageRect.X) / imageRect.Width, (end.Y - imageRect.Y) / imageRect.Height),
                    StrokeColor = DrawStrokeColor,
                    StrokeWidth = (float)DrawStrokeWidth,
                    IsArrow = drawable.ActiveLineIsArrow
                });
            }
        }

        drawable.ActiveLineStart = null;
        drawable.ActiveLineEnd = null;
        Invalidate();
    }

    #endregion

    #region Tap (Text placement)

    void OnTapped(object? sender, TappedEventArgs e)
    {
        if (CurrentToolMode != ImageEditorToolMode.Text)
            return;

        var point = e.GetPosition(graphicsView);
        if (point.HasValue)
            HandleTextPlacement(point.Value);
    }

    #endregion

    #region Crop Interaction

    void HandleCropDrag(PointF point)
    {
        var imageRect = drawable.GetImageRect();
        if (imageRect is not { Width: > 0, Height: > 0 })
            return;

        var dx = (point.X - touchStartPoint.X) / imageRect.Width;
        var dy = (point.Y - touchStartPoint.Y) / imageRect.Height;

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

    CropHandle HitTestCropHandle(PointF touchPoint)
    {
        if (!drawable.ActiveCropRect.HasValue)
            return CropHandle.None;

        var imageRect = drawable.GetImageRect();
        if (imageRect is not { Width: > 0, Height: > 0 })
            return CropHandle.None;

        var crop = drawable.ActiveCropRect.Value;
        var cropPixel = new RectF(
            imageRect.X + crop.X * imageRect.Width,
            imageRect.Y + crop.Y * imageRect.Height,
            crop.Width * imageRect.Width,
            crop.Height * imageRect.Height
        );

        if (IsNear(touchPoint, cropPixel.X, cropPixel.Y)) return CropHandle.TopLeft;
        if (IsNear(touchPoint, cropPixel.Right, cropPixel.Y)) return CropHandle.TopRight;
        if (IsNear(touchPoint, cropPixel.X, cropPixel.Bottom)) return CropHandle.BottomLeft;
        if (IsNear(touchPoint, cropPixel.Right, cropPixel.Bottom)) return CropHandle.BottomRight;

        if (IsNear(touchPoint, cropPixel.Center.X, cropPixel.Y)) return CropHandle.TopCenter;
        if (IsNear(touchPoint, cropPixel.Center.X, cropPixel.Bottom)) return CropHandle.BottomCenter;
        if (IsNear(touchPoint, cropPixel.X, cropPixel.Center.Y)) return CropHandle.MiddleLeft;
        if (IsNear(touchPoint, cropPixel.Right, cropPixel.Center.Y)) return CropHandle.MiddleRight;

        if (IsNearHorizontalEdge(touchPoint, cropPixel.X, cropPixel.Right, cropPixel.Y)) return CropHandle.TopCenter;
        if (IsNearHorizontalEdge(touchPoint, cropPixel.X, cropPixel.Right, cropPixel.Bottom)) return CropHandle.BottomCenter;
        if (IsNearVerticalEdge(touchPoint, cropPixel.Y, cropPixel.Bottom, cropPixel.X)) return CropHandle.MiddleLeft;
        if (IsNearVerticalEdge(touchPoint, cropPixel.Y, cropPixel.Bottom, cropPixel.Right)) return CropHandle.MiddleRight;

        if (cropPixel.Contains(touchPoint))
            return CropHandle.Move;

        return CropHandle.None;
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

    static bool IsNear(PointF touch, float x, float y)
    {
        var dx = touch.X - x;
        var dy = touch.Y - y;
        return dx * dx + dy * dy <= CropHandleHitRadius * CropHandleHitRadius;
    }

    static bool IsNearHorizontalEdge(PointF touch, float x1, float x2, float y)
    {
        return touch.X >= x1 - CropHandleHitRadius && touch.X <= x2 + CropHandleHitRadius
            && MathF.Abs(touch.Y - y) <= CropHandleHitRadius;
    }

    static bool IsNearVerticalEdge(PointF touch, float y1, float y2, float x)
    {
        return touch.Y >= y1 - CropHandleHitRadius && touch.Y <= y2 + CropHandleHitRadius
            && MathF.Abs(touch.X - x) <= CropHandleHitRadius;
    }

    #endregion

    #region Text Placement

    Entry? activeTextEntry;
    PointF activeTextPosition;

    void HandleTextPlacement(PointF point)
    {
        var imageRect = drawable.GetImageRect();
        if (imageRect is not { Width: > 0, Height: > 0 })
            return;

        if (!imageRect.Contains(point))
            return;

        CommitActiveTextEntry();

        activeTextPosition = point;

        activeTextEntry = new Entry
        {
            FontSize = TextFontSize,
            FontFamily = TextFontFamily,
            TextColor = DrawStrokeColor,
            BackgroundColor = Colors.Transparent,
            Placeholder = "Type here...",
            PlaceholderColor = DrawStrokeColor.WithAlpha(0.5f),
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            WidthRequest = 200,
            Margin = new Thickness(point.X, point.Y, 0, 0)
        };

        activeTextEntry.Completed += OnTextEntryCompleted;
        activeTextEntry.Unfocused += OnTextEntryUnfocused;

        Grid.SetRow(activeTextEntry, 0);
        rootGrid.Children.Add(activeTextEntry);

        activeTextEntry.Focus();
    }

    void OnTextEntryCompleted(object? sender, EventArgs e) => CommitActiveTextEntry();
    void OnTextEntryUnfocused(object? sender, FocusEventArgs e) => CommitActiveTextEntry();

    void CommitActiveTextEntry()
    {
        if (activeTextEntry == null)
            return;

        var text = activeTextEntry.Text;
        var entry = activeTextEntry;
        activeTextEntry = null;

        entry.Completed -= OnTextEntryCompleted;
        entry.Unfocused -= OnTextEntryUnfocused;
        rootGrid.Children.Remove(entry);

        if (string.IsNullOrWhiteSpace(text))
            return;

        var imageRect = drawable.GetImageRect();
        if (imageRect is not { Width: > 0, Height: > 0 })
            return;

        var normalized = new PointF(
            (activeTextPosition.X - imageRect.X) / imageRect.Width,
            (activeTextPosition.Y - imageRect.Y) / imageRect.Height
        );

        state.Push(new EditActions.TextAnnotationAction
        {
            Text = text,
            Position = normalized,
            FontSize = (float)TextFontSize,
            TextColor = DrawStrokeColor,
            FontFamily = TextFontFamily
        });
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
