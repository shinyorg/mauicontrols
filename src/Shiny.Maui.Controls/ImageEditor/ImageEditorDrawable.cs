using Shiny.Maui.Controls.ImageEditor.EditActions;

namespace Shiny.Maui.Controls.ImageEditor;

internal sealed class ImageEditorDrawable : IDrawable
{
    public Microsoft.Maui.Graphics.IImage? Image { get; set; }
    public ImageEditorState State { get; set; } = new();

    // Current tool mode
    public ImageEditorToolMode ToolMode { get; set; }

    // In-progress crop rect (normalized 0-1, relative to current image bounds)
    public RectF? ActiveCropRect { get; set; }

    // In-progress draw stroke
    public List<PointF>? ActiveStrokePoints { get; set; }
    public Color ActiveStrokeColor { get; set; } = Colors.White;
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

        DrawImageWithActions(canvas, dirtyRect);

        // Draw border around the image surface area
        if (imageRect is { Width: > 0, Height: > 0 })
        {
            canvas.StrokeColor = Color.FromRgba(255, 255, 255, 0.3f);
            canvas.StrokeSize = 1f;
            canvas.DrawRectangle(imageRect);
        }

        // Draw in-progress tool overlays
        if (ToolMode == ImageEditorToolMode.Crop && ActiveCropRect.HasValue)
            DrawCropOverlay(canvas, ActiveCropRect.Value);

        if (ToolMode == ImageEditorToolMode.Draw && ActiveStrokePoints is { Count: >= 2 })
            DrawStroke(canvas, ActiveStrokePoints, ActiveStrokeColor, ActiveStrokeWidth);

        canvas.RestoreState();
    }

    void DrawImageWithActions(ICanvas canvas, RectF dirtyRect)
    {
        // Compute cumulative crop region in normalized image coordinates (0-1)
        // and cumulative rotation
        var cropX = 0f;
        var cropY = 0f;
        var cropW = 1f;
        var cropH = 1f;
        float cumulativeRotation = 0f;

        foreach (var action in State.Actions)
        {
            switch (action)
            {
                case RotateAction rotate:
                    cumulativeRotation += rotate.AngleDegrees;
                    break;

                case CropAction crop:
                    // Compose crops: new crop is relative to current visible region
                    cropX += crop.CropRect.X * cropW;
                    cropY += crop.CropRect.Y * cropH;
                    cropW *= crop.CropRect.Width;
                    cropH *= crop.CropRect.Height;
                    break;
            }
        }

        cumulativeRotation %= 360f;
        if (cumulativeRotation < 0) cumulativeRotation += 360f;

        // Determine the effective visible image dimensions (in source pixels)
        var srcW = Image!.Width * cropW;
        var srcH = Image.Height * cropH;

        // For 90/270 rotation, the visible dimensions are swapped
        var needsSwap = Math.Abs(cumulativeRotation % 180 - 90) < 0.1f;
        var displayW = needsSwap ? srcH : srcW;
        var displayH = needsSwap ? srcW : srcH;

        // Fit the effective visible portion into the available area
        imageRect = CalculateFitRect(displayW, displayH, dirtyRect);

        // Now we need to draw only the cropped portion of the image, filling imageRect.
        // Strategy: clip to imageRect, then position/scale the full image so the cropped
        // portion aligns with imageRect.
        canvas.SaveState();
        canvas.ClipRectangle(imageRect);

        // Calculate where the full image would go such that the crop region fills imageRect
        var fullDrawW = imageRect.Width / (needsSwap ? cropH : cropW);
        var fullDrawH = imageRect.Height / (needsSwap ? cropW : cropH);

        float fullDrawX, fullDrawY;

        if (Math.Abs(cumulativeRotation) > 0.1f)
        {
            // With rotation, translate to center of imageRect, rotate, then draw offset
            canvas.Translate(imageRect.Center.X, imageRect.Center.Y);
            canvas.Rotate(cumulativeRotation);
            canvas.Translate(-imageRect.Center.X, -imageRect.Center.Y);

            if (needsSwap)
            {
                // After rotation, source X maps to display Y and vice versa
                var unrotatedW = imageRect.Height / cropW;
                var unrotatedH = imageRect.Width / cropH;
                fullDrawX = imageRect.Center.X - unrotatedW / 2f - cropX / cropW * imageRect.Height;
                fullDrawY = imageRect.Center.Y - unrotatedH / 2f - cropY / cropH * imageRect.Width;
                canvas.DrawImage(Image, fullDrawX, fullDrawY, unrotatedW, unrotatedH);
            }
            else
            {
                fullDrawX = imageRect.X - cropX / cropW * imageRect.Width;
                fullDrawY = imageRect.Y - cropY / cropH * imageRect.Height;
                canvas.DrawImage(Image, fullDrawX, fullDrawY, fullDrawW, fullDrawH);
            }
        }
        else
        {
            // No rotation: position the full image so crop region aligns with imageRect
            fullDrawX = imageRect.X - cropX / cropW * imageRect.Width;
            fullDrawY = imageRect.Y - cropY / cropH * imageRect.Height;
            canvas.DrawImage(Image, fullDrawX, fullDrawY, fullDrawW, fullDrawH);
        }

        canvas.RestoreState();

        // Draw overlay actions (strokes, text) mapped to the visible imageRect
        foreach (var action in State.Actions)
        {
            switch (action)
            {
                case DrawStrokeAction stroke:
                    var scaledPoints = stroke.Points
                        .Select(p => new PointF(
                            imageRect.X + p.X * imageRect.Width,
                            imageRect.Y + p.Y * imageRect.Height))
                        .ToList();
                    DrawStroke(canvas, scaledPoints, stroke.StrokeColor, stroke.StrokeWidth);
                    break;

                case TextAnnotationAction text:
                    var textX = imageRect.X + text.Position.X * imageRect.Width;
                    var textY = imageRect.Y + text.Position.Y * imageRect.Height;
                    canvas.FontSize = text.FontSize;
                    canvas.FontColor = text.TextColor;
                    if (!string.IsNullOrEmpty(text.FontFamily))
                        canvas.Font = new Microsoft.Maui.Graphics.Font(text.FontFamily);
                    else
                        canvas.Font = Microsoft.Maui.Graphics.Font.Default;
                    canvas.DrawString(
                        text.Text,
                        textX, textY,
                        imageRect.Width - (textX - imageRect.X),
                        text.FontSize * 1.5f,
                        HorizontalAlignment.Left,
                        VerticalAlignment.Top);
                    break;
            }
        }
    }

    void DrawCropOverlay(ICanvas canvas, RectF normalizedCrop)
    {
        var cropRect = new RectF(
            imageRect.X + normalizedCrop.X * imageRect.Width,
            imageRect.Y + normalizedCrop.Y * imageRect.Height,
            normalizedCrop.Width * imageRect.Width,
            normalizedCrop.Height * imageRect.Height
        );

        // Dim overlay around crop area
        var dimColor = Color.FromRgba(0, 0, 0, 0.5f);
        canvas.FillColor = dimColor;

        canvas.FillRectangle(imageRect.X, imageRect.Y, imageRect.Width, cropRect.Y - imageRect.Y);
        var bottomY = cropRect.Bottom;
        canvas.FillRectangle(imageRect.X, bottomY, imageRect.Width, imageRect.Bottom - bottomY);
        canvas.FillRectangle(imageRect.X, cropRect.Y, cropRect.X - imageRect.X, cropRect.Height);
        var rightX = cropRect.Right;
        canvas.FillRectangle(rightX, cropRect.Y, imageRect.Right - rightX, cropRect.Height);

        // Crop border
        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 2f;
        canvas.DrawRectangle(cropRect);

        // Rule-of-thirds
        canvas.StrokeColor = Color.FromRgba(255, 255, 255, 0.3f);
        canvas.StrokeSize = 1f;
        var thirdW = cropRect.Width / 3f;
        var thirdH = cropRect.Height / 3f;
        canvas.DrawLine(cropRect.X + thirdW, cropRect.Y, cropRect.X + thirdW, cropRect.Bottom);
        canvas.DrawLine(cropRect.X + thirdW * 2, cropRect.Y, cropRect.X + thirdW * 2, cropRect.Bottom);
        canvas.DrawLine(cropRect.X, cropRect.Y + thirdH, cropRect.Right, cropRect.Y + thirdH);
        canvas.DrawLine(cropRect.X, cropRect.Y + thirdH * 2, cropRect.Right, cropRect.Y + thirdH * 2);

        // 8 drag handles
        canvas.FillColor = Colors.White;
        const float halfHandle = 5f;

        DrawHandle(canvas, cropRect.X, cropRect.Y, halfHandle);
        DrawHandle(canvas, cropRect.Right, cropRect.Y, halfHandle);
        DrawHandle(canvas, cropRect.X, cropRect.Bottom, halfHandle);
        DrawHandle(canvas, cropRect.Right, cropRect.Bottom, halfHandle);
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

    public RectF GetImageRect() => imageRect;
}
