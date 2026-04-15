namespace Shiny.Maui.Controls.MermaidDiagrams.Models;

public sealed class DiagramModel
{
    public DiagramDirection Direction { get; set; } = DiagramDirection.TopToBottom;
    public List<DiagramNode> Nodes { get; } = [];
    public List<DiagramEdge> Edges { get; } = [];
    public List<SubgraphModel> Subgraphs { get; } = [];

    public DiagramNode GetOrAddNode(string id, string? label = null, NodeShape shape = NodeShape.Rectangle)
    {
        var existing = Nodes.Find(n => n.Id == id);
        if (existing != null)
            return existing;

        var node = new DiagramNode
        {
            Id = id,
            Label = label ?? id,
            Shape = shape
        };
        Nodes.Add(node);
        return node;
    }
}