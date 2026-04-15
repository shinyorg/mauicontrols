using Shiny.Maui.Controls.MermaidDiagrams.Theming;

namespace Shiny.Maui.Controls.MermaidDiagrams.Rendering;

static class LabelRenderer
{
    public static void DrawNodeLabel(ICanvas canvas, string text, float x, float y, float width, float height,
        NodeRenderStyle style, float fontSize)
    {
        canvas.FontColor = style.TextColor;
        canvas.FontSize = fontSize;
        canvas.DrawString(text, x, y, width, height, HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    public static void DrawEdgeLabel(ICanvas canvas, string text, float x, float y, EdgeRenderStyle style,
        float fontSize)
    {
        var textWidth = text.Length * fontSize * 0.6f;
        var textHeight = fontSize * 1.4f;
        var padding = 4f;

        // Background
        canvas.FillColor = style.LabelBackgroundColor;
        canvas.FillRoundedRectangle(
            x - textWidth / 2 - padding,
            y - textHeight / 2 - padding,
            textWidth + padding * 2,
            textHeight + padding * 2,
            3);

        // Text
        canvas.FontColor = style.LabelTextColor;
        canvas.FontSize = fontSize * 0.85f;
        canvas.DrawString(text,
            x - textWidth / 2 - padding,
            y - textHeight / 2 - padding,
            textWidth + padding * 2,
            textHeight + padding * 2,
            HorizontalAlignment.Center,
            VerticalAlignment.Center);
    }
}