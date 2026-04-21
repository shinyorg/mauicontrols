namespace Shiny.Maui.Controls.ColorPicker.Internal;

class HueBarView : GraphicsView, IDrawable
{
    float hue;
    bool isDragging;

    public event Action<float>? HueChanged;

    public float Hue
    {
        get => hue;
        set
        {
            hue = Math.Clamp(value, 0, 360);
            Invalidate();
        }
    }

    public HueBarView()
    {
        Drawable = this;

        StartInteraction += OnStartInteraction;
        DragInteraction += OnDragInteraction;
        EndInteraction += OnEndInteraction;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var w = dirtyRect.Width;
        var h = dirtyRect.Height;

        if (w <= 0 || h <= 0) return;

        // Draw hue spectrum: 0° red → 60° yellow → 120° green → 180° cyan → 240° blue → 300° magenta → 360° red
        var hueGradient = new LinearGradientPaint
        {
            StartPoint = new Point(0, 0.5),
            EndPoint = new Point(1, 0.5),
            GradientStops =
            [
                new PaintGradientStop(0f / 6f, Color.FromHsla(0.0 / 6.0, 1, 0.5)),
                new PaintGradientStop(1f / 6f, Color.FromHsla(1.0 / 6.0, 1, 0.5)),
                new PaintGradientStop(2f / 6f, Color.FromHsla(2.0 / 6.0, 1, 0.5)),
                new PaintGradientStop(3f / 6f, Color.FromHsla(3.0 / 6.0, 1, 0.5)),
                new PaintGradientStop(4f / 6f, Color.FromHsla(4.0 / 6.0, 1, 0.5)),
                new PaintGradientStop(5f / 6f, Color.FromHsla(5.0 / 6.0, 1, 0.5)),
                new PaintGradientStop(1f, Color.FromHsla(0.0, 1, 0.5))
            ]
        };

        var barRect = new RectF(0, 0, w, h);
        canvas.SetFillPaint(hueGradient, barRect);
        canvas.FillRoundedRectangle(barRect, 6);

        // Draw selector
        var cx = (hue / 360f) * w;
        var cy = h / 2f;

        canvas.FillColor = Colors.White;
        canvas.FillCircle(cx, cy, 10);
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 1.5f;
        canvas.DrawCircle(cx, cy, 10);
    }

    void OnStartInteraction(object? sender, TouchEventArgs e)
    {
        isDragging = true;
        PickFromPoint(e.Touches[0]);
    }

    void OnDragInteraction(object? sender, TouchEventArgs e)
    {
        if (isDragging)
            PickFromPoint(e.Touches[0]);
    }

    void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        isDragging = false;
    }

    void PickFromPoint(PointF point)
    {
        hue = Math.Clamp(point.X / (float)Width, 0, 1) * 360;
        Invalidate();
        HueChanged?.Invoke(hue);
    }
}
