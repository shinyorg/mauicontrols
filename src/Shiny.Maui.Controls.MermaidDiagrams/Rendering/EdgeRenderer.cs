using Shiny.Maui.Controls.MermaidDiagrams.Layout;
using Shiny.Maui.Controls.MermaidDiagrams.Models;
using Shiny.Maui.Controls.MermaidDiagrams.Theming;
using PointF = Shiny.Maui.Controls.MermaidDiagrams.Layout.PointF;

namespace Shiny.Maui.Controls.MermaidDiagrams.Rendering;

static class EdgeRenderer
{
    public static void Draw(ICanvas canvas, EdgeRoute edge, EdgeRenderStyle style, float fontSize)
    {
        if (edge.Points.Count < 2)
            return;

        canvas.StrokeColor = style.StrokeColor;
        canvas.StrokeSize = edge.Style == EdgeStyle.Thick ? style.StrokeWidth * 2 : style.StrokeWidth;

        // Set dash pattern for dotted lines
        if (edge.Style == EdgeStyle.Dotted)
            canvas.StrokeDashPattern = [6, 4];
        else
            canvas.StrokeDashPattern = null;

        // Draw polyline
        var path = new PathF();
        path.MoveTo(edge.Points[0].X, edge.Points[0].Y);
        for (var i = 1; i < edge.Points.Count; i++)
            path.LineTo(edge.Points[i].X, edge.Points[i].Y);

        canvas.DrawPath(path);

        // Draw arrowhead
        if (edge.ArrowType == ArrowType.Arrow && edge.Points.Count >= 2)
        {
            var tip = edge.Points[^1];
            var from = edge.Points[^2];
            DrawArrowhead(canvas, from, tip, style);
        }

        // Draw edge label
        if (!string.IsNullOrWhiteSpace(edge.Label))
        {
            var mid = GetMidpoint(edge.Points);
            // Reset dash pattern for label
            canvas.StrokeDashPattern = null;
            LabelRenderer.DrawEdgeLabel(canvas, edge.Label, mid.X, mid.Y, style, fontSize);
        }

        // Reset dash pattern
        canvas.StrokeDashPattern = null;
    }

    static void DrawArrowhead(ICanvas canvas, PointF from, PointF tip, EdgeRenderStyle style)
    {
        var dx = tip.X - from.X;
        var dy = tip.Y - from.Y;
        var length = MathF.Sqrt(dx * dx + dy * dy);

        if (length < 0.001f)
            return;

        var nx = dx / length;
        var ny = dy / length;

        var arrowSize = 10f;
        var baseX = tip.X - nx * arrowSize;
        var baseY = tip.Y - ny * arrowSize;

        var perpX = -ny * arrowSize * 0.4f;
        var perpY = nx * arrowSize * 0.4f;

        var arrowPath = new PathF();
        arrowPath.MoveTo(tip.X, tip.Y);
        arrowPath.LineTo(baseX + perpX, baseY + perpY);
        arrowPath.LineTo(baseX - perpX, baseY - perpY);
        arrowPath.Close();

        canvas.FillColor = style.StrokeColor;
        canvas.StrokeDashPattern = null;
        canvas.FillPath(arrowPath);
    }

    static PointF GetMidpoint(List<PointF> points)
    {
        if (points.Count == 2)
            return new PointF((points[0].X + points[1].X) / 2, (points[0].Y + points[1].Y) / 2);

        var midIdx = points.Count / 2;
        return points[midIdx];
    }
}