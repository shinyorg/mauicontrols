using Shiny.Maui.Controls.MermaidDiagrams.Models;
using Shiny.Maui.Controls.MermaidDiagrams.Parsing;
using Shouldly;
using Xunit;

namespace Shiny.Maui.Controls.MermaidDiagrams.Tests.Parsing;

public class MermaidParserTests
{
    // -----------------------------------------------------------------
    // Parse_SimpleFlowchart_ReturnsModel
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_SimpleFlowchart_ReturnsModel()
    {
        var model = MermaidParser.Parse("graph TD\nA-->B");

        model.Nodes.Count.ShouldBe(2);
        model.Edges.Count.ShouldBe(1);
        model.Direction.ShouldBe(DiagramDirection.TopToBottom);
    }

    // -----------------------------------------------------------------
    // Parse_LeftToRight_SetsDirection
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_LeftToRight_SetsDirection()
    {
        var model = MermaidParser.Parse("graph LR\nA-->B");

        model.Direction.ShouldBe(DiagramDirection.LeftToRight);
    }

    // -----------------------------------------------------------------
    // Parse_NodeWithLabel_SetsLabel
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_NodeWithLabel_SetsLabel()
    {
        var model = MermaidParser.Parse("graph TD\nA[My Label]-->B");

        var nodeA = model.Nodes.FirstOrDefault(n => n.Id == "A");
        nodeA.ShouldNotBeNull();
        nodeA.Label.ShouldBe("My Label");
    }

    // -----------------------------------------------------------------
    // Parse_RoundedNode_SetsShape
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_RoundedNode_SetsShape()
    {
        var model = MermaidParser.Parse("graph TD\nA(Rounded)");

        var nodeA = model.Nodes.FirstOrDefault(n => n.Id == "A");
        nodeA.ShouldNotBeNull();
        nodeA.Shape.ShouldBe(NodeShape.RoundedRect);
    }

    // -----------------------------------------------------------------
    // Parse_CircleNode_SetsShape
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_CircleNode_SetsShape()
    {
        var model = MermaidParser.Parse("graph TD\nA((Circle))");

        var nodeA = model.Nodes.FirstOrDefault(n => n.Id == "A");
        nodeA.ShouldNotBeNull();
        nodeA.Shape.ShouldBe(NodeShape.Circle);
    }

    // -----------------------------------------------------------------
    // Parse_DiamondNode_SetsShape
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_DiamondNode_SetsShape()
    {
        var model = MermaidParser.Parse("graph TD\nA{Diamond}");

        var nodeA = model.Nodes.FirstOrDefault(n => n.Id == "A");
        nodeA.ShouldNotBeNull();
        nodeA.Shape.ShouldBe(NodeShape.Diamond);
    }

    // -----------------------------------------------------------------
    // Parse_EdgeWithLabel_SetsLabel
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_EdgeWithLabel_SetsLabel()
    {
        var model = MermaidParser.Parse("graph TD\nA-->|yes|B");

        var edge = model.Edges.FirstOrDefault();
        edge.ShouldNotBeNull();
        edge.Label.ShouldBe("yes");
    }

    // -----------------------------------------------------------------
    // Parse_Subgraph_CreatesSubgraph
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_Subgraph_CreatesSubgraph()
    {
        var model = MermaidParser.Parse("graph TD\nsubgraph sub1\nA-->B\nend");

        model.Subgraphs.Count.ShouldBe(1);
        model.Subgraphs[0].Title.ShouldBe("sub1");
    }

    // -----------------------------------------------------------------
    // Parse_ChainedEdges_CreatesMultipleEdges
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_ChainedEdges_CreatesMultipleEdges()
    {
        var model = MermaidParser.Parse("graph TD\nA-->B-->C");

        model.Edges.Count.ShouldBe(2);

        var firstEdge = model.Edges.FirstOrDefault(e => e.SourceId == "A" && e.TargetId == "B");
        firstEdge.ShouldNotBeNull();

        var secondEdge = model.Edges.FirstOrDefault(e => e.SourceId == "B" && e.TargetId == "C");
        secondEdge.ShouldNotBeNull();
    }

    // -----------------------------------------------------------------
    // Parse_FlowchartKeyword_Works
    // -----------------------------------------------------------------
    [Fact]
    public void Parse_FlowchartKeyword_Works()
    {
        var model = MermaidParser.Parse("flowchart TD\nA-->B");

        model.Nodes.Count.ShouldBe(2);
        model.Edges.Count.ShouldBe(1);
    }
}