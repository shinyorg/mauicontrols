namespace Shiny.Maui.Controls.MermaidDiagrams.Models;

public sealed class DiagramNode
{
    public required string Id { get; init; }
    public required string Label { get; init; }
    public NodeShape Shape { get; init; } = NodeShape.Rectangle;
    public string? SubgraphId { get; set; }
}