using Shiny.Maui.Controls.MermaidDiagrams.Models;

namespace Shiny.Maui.Controls.MermaidDiagrams.Layout;

public sealed class LayoutResult
{
    public Dictionary<string, NodeLayout> Nodes { get; } = new();
    public List<EdgeRoute> Edges { get; } = [];
    public Dictionary<string, SubgraphLayout> Subgraphs { get; } = new();
    public float TotalWidth { get; set; }
    public float TotalHeight { get; set; }
}

public sealed class NodeLayout
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public required NodeShape Shape { get; init; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }

    public float CenterX => X + Width / 2;
    public float CenterY => Y + Height / 2;
}

public sealed class EdgeRoute
{
    public required string SourceId { get; init; }
    public required string TargetId { get; init; }
    public string? Label { get; init; }
    public EdgeStyle Style { get; init; }
    public ArrowType ArrowType { get; init; }
    public List<PointF> Points { get; } = [];
}

public sealed class SubgraphLayout
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
}

public readonly record struct PointF(float X, float Y);