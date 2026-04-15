namespace Shiny.Maui.Controls.MermaidDiagrams.Theming;

public abstract class DiagramTheme
{
    public abstract Color BackgroundColor { get; }
    public abstract NodeRenderStyle DefaultNodeStyle { get; }
    public abstract EdgeRenderStyle DefaultEdgeStyle { get; }
    public abstract SubgraphRenderStyle DefaultSubgraphStyle { get; }
    public abstract Color TextColor { get; }
    public abstract float FontSize { get; }

    public virtual NodeRenderStyle GetNodeStyle(int index) => DefaultNodeStyle;
}

public sealed class NodeRenderStyle
{
    public required Color FillColor { get; init; }
    public required Color StrokeColor { get; init; }
    public required Color TextColor { get; init; }
    public float StrokeWidth { get; init; } = 2;
    public float CornerRadius { get; init; } = 4;
}

public sealed class EdgeRenderStyle
{
    public required Color StrokeColor { get; init; }
    public float StrokeWidth { get; init; } = 2;
    public required Color LabelBackgroundColor { get; init; }
    public required Color LabelTextColor { get; init; }
}

public sealed class SubgraphRenderStyle
{
    public required Color FillColor { get; init; }
    public required Color StrokeColor { get; init; }
    public required Color TitleColor { get; init; }
    public float StrokeWidth { get; init; } = 1.5f;
    public float CornerRadius { get; init; } = 8;
}