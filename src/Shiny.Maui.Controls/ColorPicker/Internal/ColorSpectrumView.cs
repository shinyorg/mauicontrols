namespace Shiny.Maui.Controls.ColorPicker.Internal;

class ColorSpectrumView : GraphicsView, IDrawable
{
    float hue;
    float saturation = 1f;
    float brightness = 1f;
    bool isDragging;

    public event Action<float, float>? ColorPicked;

    public float Hue
    {
        get => hue;
        set
        {
            hue = value;
            Invalidate();
        }
    }

    public ColorSpectrumView()
    {
        Drawable = this;

        StartInteraction += OnStartInteraction;
        DragInteraction += OnDragInteraction;
        EndInteraction += OnEndInteraction;
    }

    public (float Hue, float Saturation, float Brightness) GetCurrentSaturationBrightness()
        => (hue, saturation, brightness);

    public void SetSaturationBrightness(float s, float b)
    {
        saturation = Math.Clamp(s, 0, 1);
        brightness = Math.Clamp(b, 0, 1);
        Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var w = dirtyRect.Width;
        var h = dirtyRect.Height;

        if (w <= 0 || h <= 0) return;

        // Draw saturation/brightness gradient
        // Horizontal: white → hue color (saturation)
        // Vertical: top → bottom (brightness, light → dark)
        var hueColor = HsvToColor(hue, 1, 1);

        // White to hue (horizontal)
        var satGradient = new LinearGradientPaint
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 0),
            GradientStops =
            [
                new PaintGradientStop(0, Colors.White),
                new PaintGradientStop(1, hueColor)
            ]
        };
        canvas.SetFillPaint(satGradient, dirtyRect);
        canvas.FillRectangle(dirtyRect);

        // Transparent to black (vertical)
        var brightGradient = new LinearGradientPaint
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1),
            GradientStops =
            [
                new PaintGradientStop(0, Colors.Transparent),
                new PaintGradientStop(1, Colors.Black)
            ]
        };
        canvas.SetFillPaint(brightGradient, dirtyRect);
        canvas.FillRectangle(dirtyRect);

        // Draw selector circle
        var cx = saturation * w;
        var cy = (1 - brightness) * h;

        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 3;
        canvas.DrawCircle(cx, cy, 8);
        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 1;
        canvas.DrawCircle(cx, cy, 9);
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
        saturation = Math.Clamp(point.X / (float)Width, 0, 1);
        brightness = 1 - Math.Clamp(point.Y / (float)Height, 0, 1);
        Invalidate();
        ColorPicked?.Invoke(saturation, brightness);
    }

    static Color HsvToColor(float h, float s, float v)
    {
        var c = v * s;
        var x = c * (1 - MathF.Abs((h / 60f) % 2 - 1));
        var m = v - c;

        float r, g, b;
        switch ((int)(h / 60) % 6)
        {
            case 0: r = c; g = x; b = 0; break;
            case 1: r = x; g = c; b = 0; break;
            case 2: r = 0; g = c; b = x; break;
            case 3: r = 0; g = x; b = c; break;
            case 4: r = x; g = 0; b = c; break;
            default: r = c; g = 0; b = x; break;
        }

        return new Color(r + m, g + m, b + m);
    }
}
