using Shiny.Maui.Controls.MermaidDiagrams.Models;

namespace Shiny.Maui.Controls.MermaidDiagrams.Layout;

public interface ILayoutEngine
{
    LayoutResult Layout(DiagramModel model, DiagramLayoutOptions options);
}

public sealed class DiagramLayoutOptions
{
    public float NodeWidth { get; init; } = 150;
    public float NodeHeight { get; init; } = 50;
    public float HorizontalSpacing { get; init; } = 60;
    public float VerticalSpacing { get; init; } = 80;
    public float SubgraphPadding { get; init; } = 30;
    public float FontSize { get; init; } = 14;
    public float Margin { get; init; } = 40;
}