using Shiny.Maui.Controls.MermaidDiagrams.Layout;
using Shiny.Maui.Controls.MermaidDiagrams.Models;
using Shouldly;
using Xunit;

namespace Shiny.Maui.Controls.MermaidDiagrams.Tests.Layout;

public class SugiyamaLayoutEngineTests
{
    static LayoutResult RunLayout(DiagramModel model)
        => new SugiyamaLayoutEngine().Layout(model, new DiagramLayoutOptions());

    // -----------------------------------------------------------------
    // Layout_EmptyModel_ReturnsEmptyResult
    // -----------------------------------------------------------------
    [Fact]
    public void Layout_EmptyModel_ReturnsEmptyResult()
    {
        var model = new DiagramModel();

        var result = RunLayout(model);

        result.Nodes.ShouldBeEmpty();
        result.Edges.ShouldBeEmpty();
    }

    // -----------------------------------------------------------------
    // Layout_SingleNode_PositionsNode
    // -----------------------------------------------------------------
    [Fact]
    public void Layout_SingleNode_PositionsNode()
    {
        var model = new DiagramModel();
        model.GetOrAddNode("A");

        var result = RunLayout(model);

        result.Nodes.Count.ShouldBe(1);

        var nodeLayout = result.Nodes["A"];
        nodeLayout.ShouldNotBeNull();
        nodeLayout.Width.ShouldBeGreaterThan(0f);
        nodeLayout.Height.ShouldBeGreaterThan(0f);
    }

    // -----------------------------------------------------------------
    // Layout_TwoConnectedNodes_HasTwoNodesAndOneEdge
    // -----------------------------------------------------------------
    [Fact]
    public void Layout_TwoConnectedNodes_HasTwoNodesAndOneEdge()
    {
        var model = new DiagramModel();
        model.GetOrAddNode("A");
        model.GetOrAddNode("B");
        model.Edges.Add(new DiagramEdge { SourceId = "A", TargetId = "B" });

        var result = RunLayout(model);

        result.Nodes.Count.ShouldBe(2);
        result.Edges.Count.ShouldBe(1);
    }

    // -----------------------------------------------------------------
    // Layout_TopToBottom_SourceAboveTarget
    // -----------------------------------------------------------------
    [Fact]
    public void Layout_TopToBottom_SourceAboveTarget()
    {
        var model = new DiagramModel { Direction = DiagramDirection.TopToBottom };
        model.GetOrAddNode("A");
        model.GetOrAddNode("B");
        model.Edges.Add(new DiagramEdge { SourceId = "A", TargetId = "B" });

        var result = RunLayout(model);

        var sourceLayout = result.Nodes["A"];
        var targetLayout = result.Nodes["B"];

        sourceLayout.Y.ShouldBeLessThan(targetLayout.Y);
    }

    // -----------------------------------------------------------------
    // Layout_LeftToRight_SourceLeftOfTarget
    // -----------------------------------------------------------------
    [Fact]
    public void Layout_LeftToRight_SourceLeftOfTarget()
    {
        var model = new DiagramModel { Direction = DiagramDirection.LeftToRight };
        model.GetOrAddNode("A");
        model.GetOrAddNode("B");
        model.Edges.Add(new DiagramEdge { SourceId = "A", TargetId = "B" });

        var result = RunLayout(model);

        var sourceLayout = result.Nodes["A"];
        var targetLayout = result.Nodes["B"];

        sourceLayout.X.ShouldBeLessThan(targetLayout.X);
    }

    // -----------------------------------------------------------------
    // Layout_WithSubgraph_CreatesSubgraphLayout
    // -----------------------------------------------------------------
    [Fact]
    public void Layout_WithSubgraph_CreatesSubgraphLayout()
    {
        var model = new DiagramModel();

        var nodeA = model.GetOrAddNode("A");
        nodeA.SubgraphId = "sg1";

        var nodeB = model.GetOrAddNode("B");
        nodeB.SubgraphId = "sg1";

        model.Edges.Add(new DiagramEdge { SourceId = "A", TargetId = "B" });

        model.Subgraphs.Add(new SubgraphModel
        {
            Id = "sg1",
            Title = "My Subgraph",
            NodeIds = { "A", "B" }
        });

        var result = RunLayout(model);

        result.Subgraphs.ShouldContainKey("sg1");
        var subgraphLayout = result.Subgraphs["sg1"];
        subgraphLayout.ShouldNotBeNull();
        subgraphLayout.Title.ShouldBe("My Subgraph");
    }
}