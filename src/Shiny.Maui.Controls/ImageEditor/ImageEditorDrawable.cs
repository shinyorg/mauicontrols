using Shiny.Maui.Controls.ImageEditor.EditActions;

namespace Shiny.Maui.Controls.ImageEditor;

internal sealed class ImageEditorDrawable : IDrawable
{
    public Microsoft.Maui.Graphics.IImage? Image { get; set; }
    public ImageEditorState State { get; } = new();

    // View transform (zoom/pan - not edit actions)
    public float ViewScale { get; set; } = 1f;
    public float ViewOffsetX { get; set; }
    public float ViewOffsetY { get; set; }

    // Current tool mode
    public ImageEditorToolMode ToolMode { get; set; }

    // In-progress crop rect (normalized 0-1, relative to current image bounds)
    public RectF? ActiveCropRect { get; set; }

    // In-progress draw stroke
    public List<PointF>? ActiveStrokePoints { get; set; }
    public Color ActiveStrokeColor { get; set; } = Colors.Red;
    public float ActiveStrokeWidth { get; set; } = 3f;

    // Cached effective image bounds after applying all crop/rotate actions
    RectF imageRect;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(dirtyRect);

        if (Image == null)
            return;

        canvas.SaveState();

        // Calculate base image rect (fit image into available area)
        imageRect = CalculateFitRect(Image.Width, Image.Height, dirtyRect);

        // Apply view transform (zoom/pan)
        var cx = dirtyRect.Width / 2f;
        var cy = dirtyRect.Height / 2f;
        canvas.Translate(cx + ViewOffsetX, cy + ViewOffsetY);
        canvas.Scale(ViewScale, ViewScale);
        canvas.Translate(-cx, -cy);

        // Draw the image with all committed actions applied
        DrawImageWithActions(canvas, dirtyRect);

        // Draw in-progress tool overlays
        if (ToolMode == ImageEditorToolMode.Crop && ActiveCropRect.HasValue)
            DrawCropOverlay(canvas, ActiveCropRect.Value);

        if (ToolMode == ImageEditorToolMode.Draw && ActiveStrokePoints is { Count: >= 2 })
            DrawStroke(canvas, ActiveStrokePoints, ActiveStrokeColor, ActiveStrokeWidth);

        canvas.RestoreState();
    }

    void DrawImageWithActions(ICanvas canvas, RectF dirtyRect)
    {
        // Build up cumulative transform from actions
        var currentRect = imageRect;
        float cumulativeRotation = 0f;

        // First pass: compute the effective crop region and rotation
        foreach (var action in State.Actions)
        {
            switch (action)
            {
                case RotateAction rotate:
                    cumulativeRotation += rotate.AngleDegrees;
                    break;

                case CropAction crop:
                    // Apply crop to current rect (crop rect is normalized 0-1)
                    currentRect = new RectF(
                        currentRect.X + crop.CropRect.X * currentRect.Width,
                        currentRect.Y + crop.CropRect.Y * currentRect.Height,
                        crop.CropRect.Width * currentRect.Width,
                        crop.CropRect.Height * currentRect.Height
                    );
                    break;
            }
        }

        // Normalize rotation
        cumulativeRotation %= 360f;
        if (cumulativeRotation < 0) cumulativeRotation += 360f;

        // For 90/270 degree rotations, the image rect dimensions swap
        var drawRect = currentRect;
        var needsSwap = Math.Abs(cumulativeRotation % 180 - 90) < 0.1f;
        if (needsSwap)
        {
            // Swap width/height and recenter
            var newWidth = currentRect.Height;
            var newHeight = currentRect.Width;
            drawRect = new RectF(
                currentRect.Center.X - newWidth / 2f,
                currentRect.Center.Y - newHeight / 2f,
                newWidth,
                newHeight
            );
        }

        // Draw the image with rotation
        if (Math.Abs(cumulativeRotation) > 0.1f)
        {
            canvas.SaveState();
            canvas.Translate(drawRect.Center.X, drawRect.Center.Y);
            canvas.Rotate(cumulativeRotation);
            canvas.Translate(-drawRect.Center.X, -drawRect.Center.Y);

            // When rotated, draw into the unswapped rect so rotation makes it fit
            if (needsSwap)
            {
                var unswapped = new RectF(
                    drawRect.Center.X - currentRect.Width / 2f,
                    drawRect.Center.Y - currentRect.Height / 2f,
                    currentRect.Width,
                    currentRect.Height
                );
                canvas.DrawImage(Image, unswapped.X, unswapped.Y, unswapped.Width, unswapped.Height);
            }
            else
            {
                canvas.DrawImage(Image, drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height);
            }

            canvas.RestoreState();
        }
        else
        {
            canvas.DrawImage(Image, drawRect.X, drawRect.Y, drawRect.Width, drawRect.Height);
        }

        // Second pass: draw overlay actions (strokes, text)
        foreach (var action in State.Actions)
        {
            switch (action)
            {
                case DrawStrokeAction stroke:
                    var scaledPoints = stroke.Points
                        .Select(p => new PointF(
                            drawRect.X + p.X * drawRect.Width,
                            drawRect.Y + p.Y * drawRect.Height))
                        .ToList();
                    DrawStroke(canvas, scaledPoints, stroke.StrokeColor, stroke.StrokeWidth);
                    break;

                case TextAnnotationAction text:
                    var textX = drawRect.X + text.Position.X * drawRect.Width;
                    var textY = drawRect.Y + text.Position.Y * drawRect.Height;
                    canvas.FontSize = text.FontSize;
                    canvas.FontColor = text.TextColor;
                    canvas.DrawString(
                        text.Text,
                        textX, textY,
                        drawRect.Width - (textX - drawRect.X),
                        text.FontSize * 1.5f,
                        HorizontalAlignment.Left,
                        VerticalAlignment.Top);
                    break;
            }
        }
    }

    void DrawCropOverlay(ICanvas canvas, RectF normalizedCrop)
    {
        // Convert normalized crop to pixel coords
        var cropRect = new RectF(
            imageRect.X + normalizedCrop.X * imageRect.Width,
            imageRect.Y + normalizedCrop.Y * imageRect.Height,
            normalizedCrop.Width * imageRect.Width,
            normalizedCrop.Height * imageRect.Height
        );

        // Draw dim overlay in four rectangles around the crop area
        var dimColor = Color.FromRgba(0, 0, 0, 0.5f);
        canvas.FillColor = dimColor;

        // Top
        canvas.FillRectangle(imageRect.X, imageRect.Y, imageRect.Width, cropRect.Y - imageRect.Y);
        // Bottom
        var bottomY = cropRect.Bottom;
        canvas.FillRectangle(imageRect.X, bottomY, imageRect.Width, imageRect.Bottom - bottomY);
        // Left
        canvas.FillRectangle(imageRect.X, cropRect.Y, cropRect.X - imageRect.X, cropRect.Height);
        // Right
        var rightX = cropRect.Right;
        canvas.FillRectangle(rightX, cropRect.Y, imageRect.Right - rightX, cropRect.Height);

        // Draw crop border
        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 2f;
        canvas.DrawRectangle(cropRect);

        // Draw rule-of-thirds lines
        canvas.StrokeColor = Color.FromRgba(255, 255, 255, 0.3f);
        canvas.StrokeSize = 1f;
        var thirdW = cropRect.Width / 3f;
        var thirdH = cropRect.Height / 3f;
        canvas.DrawLine(cropRect.X + thirdW, cropRect.Y, cropRect.X + thirdW, cropRect.Bottom);
        canvas.DrawLine(cropRect.X + thirdW * 2, cropRect.Y, cropRect.X + thirdW * 2, cropRect.Bottom);
        canvas.DrawLine(cropRect.X, cropRect.Y + thirdH, cropRect.Right, cropRect.Y + thirdH);
        canvas.DrawLine(cropRect.X, cropRect.Y + thirdH * 2, cropRect.Right, cropRect.Y + thirdH * 2);

        // Draw 8 drag handles (4 corners + 4 midpoints)
        canvas.FillColor = Colors.White;
        const float handleSize = 10f;
        var halfHandle = handleSize / 2f;

        // Corners
        DrawHandle(canvas, cropRect.X, cropRect.Y, halfHandle);
        DrawHandle(canvas, cropRect.Right, cropRect.Y, halfHandle);
        DrawHandle(canvas, cropRect.X, cropRect.Bottom, halfHandle);
        DrawHandle(canvas, cropRect.Right, cropRect.Bottom, halfHandle);

        // Midpoints
        DrawHandle(canvas, cropRect.Center.X, cropRect.Y, halfHandle);
        DrawHandle(canvas, cropRect.Center.X, cropRect.Bottom, halfHandle);
        DrawHandle(canvas, cropRect.X, cropRect.Center.Y, halfHandle);
        DrawHandle(canvas, cropRect.Right, cropRect.Center.Y, halfHandle);
    }

    static void DrawHandle(ICanvas canvas, float x, float y, float halfSize)
    {
        canvas.FillRoundedRectangle(x - halfSize, y - halfSize, halfSize * 2, halfSize * 2, 2f);
    }

    static void DrawStroke(ICanvas canvas, IList<PointF> points, Color color, float width)
    {
        if (points.Count < 2)
            return;

        canvas.StrokeColor = color;
        canvas.StrokeSize = width;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;

        var path = new PathF();
        path.MoveTo(points[0]);
        for (var i = 1; i < points.Count; i++)
            path.LineTo(points[i]);

        canvas.DrawPath(path);
    }

    static RectF CalculateFitRect(float imageWidth, float imageHeight, RectF availableRect)
    {
        if (imageWidth <= 0 || imageHeight <= 0)
            return RectF.Zero;

        var scaleX = availableRect.Width / imageWidth;
        var scaleY = availableRect.Height / imageHeight;
        var scale = Math.Min(scaleX, scaleY);

        var fitWidth = imageWidth * scale;
        var fitHeight = imageHeight * scale;

        return new RectF(
            availableRect.X + (availableRect.Width - fitWidth) / 2f,
            availableRect.Y + (availableRect.Height - fitHeight) / 2f,
            fitWidth,
            fitHeight
        );
    }

    /// <summary>
    /// Returns the current image rect in view coordinates (for gesture hit testing).
    /// </summary>
    public RectF GetImageRect() => imageRect;
}
