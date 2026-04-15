using Shiny.Maui.Controls.MermaidDiagrams.Layout;
using Shiny.Maui.Controls.MermaidDiagrams.Models;
using Shiny.Maui.Controls.MermaidDiagrams.Theming;

namespace Shiny.Maui.Controls.MermaidDiagrams.Rendering;

static class NodeRenderer
{
    public static void Draw(ICanvas canvas, NodeLayout node, NodeRenderStyle style, float fontSize)
    {
        canvas.StrokeColor = style.StrokeColor;
        canvas.StrokeSize = style.StrokeWidth;
        canvas.FillColor = style.FillColor;

        switch (node.Shape)
        {
            case NodeShape.Rectangle:
                DrawRectangle(canvas, node, style);
                break;
            case NodeShape.RoundedRect:
                DrawRoundedRect(canvas, node, style);
                break;
            case NodeShape.Stadium:
                DrawStadium(canvas, node, style);
                break;
            case NodeShape.Circle:
                DrawCircle(canvas, node, style);
                break;
            case NodeShape.Diamond:
                DrawDiamond(canvas, node, style);
                break;
            case NodeShape.Hexagon:
                DrawHexagon(canvas, node, style);
                break;
        }

        LabelRenderer.DrawNodeLabel(canvas, node.Label, node.X, node.Y, node.Width, node.Height, style, fontSize);
    }

    static void DrawRectangle(ICanvas canvas, NodeLayout node, NodeRenderStyle style)
    {
        canvas.FillRectangle(node.X, node.Y, node.Width, node.Height);
        canvas.DrawRectangle(node.X, node.Y, node.Width, node.Height);
    }

    static void DrawRoundedRect(ICanvas canvas, NodeLayout node, NodeRenderStyle style)
    {
        canvas.FillRoundedRectangle(node.X, node.Y, node.Width, node.Height, style.CornerRadius);
        canvas.DrawRoundedRectangle(node.X, node.Y, node.Width, node.Height, style.CornerRadius);
    }

    static void DrawStadium(ICanvas canvas, NodeLayout node, NodeRenderStyle style)
    {
        var radius = node.Height / 2;
        canvas.FillRoundedRectangle(node.X, node.Y, node.Width, node.Height, radius);
        canvas.DrawRoundedRectangle(node.X, node.Y, node.Width, node.Height, radius);
    }

    static void DrawCircle(ICanvas canvas, NodeLayout node, NodeRenderStyle style)
    {
        var cx = node.CenterX;
        var cy = node.CenterY;
        var r = Math.Min(node.Width, node.Height) / 2;
        canvas.FillCircle(cx, cy, r);
        canvas.DrawCircle(cx, cy, r);
    }

    static void DrawDiamond(ICanvas canvas, NodeLayout node, NodeRenderStyle style)
    {
        var cx = node.CenterX;
        var cy = node.CenterY;
        var hw = node.Width / 2;
        var hh = node.Height / 2;

        var path = new PathF();
        path.MoveTo(cx, cy - hh);
        path.LineTo(cx + hw, cy);
        path.LineTo(cx, cy + hh);
        path.LineTo(cx - hw, cy);
        path.Close();

        canvas.FillPath(path);
        canvas.DrawPath(path);
    }

    static void DrawHexagon(ICanvas canvas, NodeLayout node, NodeRenderStyle style)
    {
        var cx = node.CenterX;
        var cy = node.CenterY;
        var hw = node.Width / 2;
        var hh = node.Height / 2;
        var inset = hw * 0.25f;

        var path = new PathF();
        path.MoveTo(node.X + inset, node.Y);
        path.LineTo(node.X + node.Width - inset, node.Y);
        path.LineTo(node.X + node.Width, cy);
        path.LineTo(node.X + node.Width - inset, node.Y + node.Height);
        path.LineTo(node.X + inset, node.Y + node.Height);
        path.LineTo(node.X, cy);
        path.Close();

        canvas.FillPath(path);
        canvas.DrawPath(path);
    }
}