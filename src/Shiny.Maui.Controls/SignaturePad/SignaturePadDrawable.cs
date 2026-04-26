namespace Shiny.Maui.Controls.SignaturePad;

internal sealed class SignaturePadDrawable : IDrawable
{
    readonly List<List<PointF>> committedStrokes = new();
    List<PointF>? activeStroke;

    public Color BackgroundColor { get; set; } = Colors.White;
    public Color StrokeColor { get; set; } = Colors.Black;
    public float StrokeWidth { get; set; } = 3f;
    public bool HasSignature => committedStrokes.Count > 0;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = BackgroundColor;
        canvas.FillRectangle(dirtyRect);

        DrawStrokes(canvas, committedStrokes);

        if (activeStroke is { Count: >= 2 })
            DrawSingleStroke(canvas, activeStroke);
    }

    public void BeginStroke(PointF point)
    {
        activeStroke = new List<PointF> { point };
    }

    public void AddPoint(PointF point)
    {
        activeStroke?.Add(point);
    }

    public void EndStroke()
    {
        if (activeStroke is { Count: >= 2 })
            committedStrokes.Add(activeStroke);

        activeStroke = null;
    }

    public void Clear()
    {
        committedStrokes.Clear();
        activeStroke = null;
    }

    public Stream ExportToPng(int width, int height)
    {
#if IOS || MACCATALYST || ANDROID
        using var context = new Microsoft.Maui.Graphics.Platform.PlatformBitmapExportContext(width, height, 1f);
        Draw(context.Canvas, new RectF(0, 0, width, height));
        return context.Image.AsStream(Microsoft.Maui.Graphics.ImageFormat.Png);
#else
        return Stream.Null;
#endif
    }

    void DrawStrokes(ICanvas canvas, List<List<PointF>> strokes)
    {
        foreach (var stroke in strokes)
        {
            if (stroke.Count >= 2)
                DrawSingleStroke(canvas, stroke);
        }
    }

    void DrawSingleStroke(ICanvas canvas, IList<PointF> points)
    {
        canvas.StrokeColor = StrokeColor;
        canvas.StrokeSize = StrokeWidth;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;

        var path = new PathF();
        path.MoveTo(points[0]);
        for (var i = 1; i < points.Count; i++)
            path.LineTo(points[i]);

        canvas.DrawPath(path);
    }
}
