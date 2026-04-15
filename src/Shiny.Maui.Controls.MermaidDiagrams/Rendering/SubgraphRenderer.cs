using Shiny.Maui.Controls.MermaidDiagrams.Layout;
using Shiny.Maui.Controls.MermaidDiagrams.Theming;

namespace Shiny.Maui.Controls.MermaidDiagrams.Rendering;

static class SubgraphRenderer
{
    public static void Draw(ICanvas canvas, SubgraphLayout subgraph, SubgraphRenderStyle style, float fontSize)
    {
        // Fill
        canvas.FillColor = style.FillColor;
        canvas.FillRoundedRectangle(subgraph.X, subgraph.Y, subgraph.Width, subgraph.Height, style.CornerRadius);

        // Stroke (dashed)
        canvas.StrokeColor = style.StrokeColor;
        canvas.StrokeSize = style.StrokeWidth;
        canvas.StrokeDashPattern = [8, 4];
        canvas.DrawRoundedRectangle(subgraph.X, subgraph.Y, subgraph.Width, subgraph.Height, style.CornerRadius);
        canvas.StrokeDashPattern = null;

        // Title
        canvas.FontColor = style.TitleColor;
        canvas.FontSize = fontSize;
        canvas.DrawString(
            subgraph.Title,
            subgraph.X + 10,
            subgraph.Y + 4,
            subgraph.Width - 20,
            fontSize * 1.5f,
            HorizontalAlignment.Left,
            VerticalAlignment.Center);
    }
}