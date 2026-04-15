using Shiny.Maui.Controls.MermaidDiagrams.Layout;
using Shiny.Maui.Controls.MermaidDiagrams.Theming;

namespace Shiny.Maui.Controls.MermaidDiagrams.Rendering;

public sealed class DiagramDrawable : IDrawable
{
    public LayoutResult? LayoutResult { get; set; }
    public DiagramTheme Theme { get; set; } = new DefaultTheme();
    public float OffsetX { get; set; }
    public float OffsetY { get; set; }
    public float UserScale { get; set; } = 1f;

    // The computed fit scale from the last draw
    public float FitScale { get; private set; } = 1f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Theme.BackgroundColor;
        canvas.FillRectangle(dirtyRect);

        if (LayoutResult == null)
            return;

        // Compute scale to fit diagram content into the available rect
        FitScale = 1f;
        float centerOffsetX = 0;
        float centerOffsetY = 0;

        if (LayoutResult.TotalWidth > 0 && LayoutResult.TotalHeight > 0 &&
            dirtyRect.Width > 0 && dirtyRect.Height > 0)
        {
            var scaleX = dirtyRect.Width / LayoutResult.TotalWidth;
            var scaleY = dirtyRect.Height / LayoutResult.TotalHeight;
            FitScale = Math.Min(scaleX, scaleY);

            // Don't upscale beyond 1x if diagram is smaller than viewport
            FitScale = Math.Min(FitScale, 1f);

            // Center the diagram in the available space
            var scaledWidth = LayoutResult.TotalWidth * FitScale * UserScale;
            var scaledHeight = LayoutResult.TotalHeight * FitScale * UserScale;
            centerOffsetX = (dirtyRect.Width - scaledWidth) / 2f;
            centerOffsetY = (dirtyRect.Height - scaledHeight) / 2f;
        }

        var effectiveScale = FitScale * UserScale;

        canvas.SaveState();
        canvas.Translate(centerOffsetX + OffsetX, centerOffsetY + OffsetY);
        canvas.Scale(effectiveScale, effectiveScale);

        // Draw subgraphs first (background)
        foreach (var subgraph in LayoutResult.Subgraphs.Values)
            SubgraphRenderer.Draw(canvas, subgraph, Theme.DefaultSubgraphStyle, Theme.FontSize);

        // Draw edges
        foreach (var edge in LayoutResult.Edges)
            EdgeRenderer.Draw(canvas, edge, Theme.DefaultEdgeStyle, Theme.FontSize);

        // Draw nodes on top
        var nodeIndex = 0;
        foreach (var node in LayoutResult.Nodes.Values)
        {
            var style = Theme.GetNodeStyle(nodeIndex++);
            NodeRenderer.Draw(canvas, node, style, Theme.FontSize);
        }

        canvas.RestoreState();
    }
}