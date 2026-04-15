namespace Shiny.Maui.Controls.MermaidDiagrams.Models;

public sealed class DiagramEdge
{
    public required string SourceId { get; init; }
    public required string TargetId { get; init; }
    public string? Label { get; init; }
    public EdgeStyle Style { get; init; } = EdgeStyle.Solid;
    public ArrowType ArrowType { get; init; } = ArrowType.Arrow;
}