using Shiny.Maui.Controls.MermaidDiagrams.Layout.Algorithms;
using Shiny.Maui.Controls.MermaidDiagrams.Models;

namespace Shiny.Maui.Controls.MermaidDiagrams.Layout;

public sealed class SugiyamaLayoutEngine : ILayoutEngine
{
    public LayoutResult Layout(DiagramModel model, DiagramLayoutOptions options)
    {
        var result = new LayoutResult();

        if (model.Nodes.Count == 0)
            return result;

        var nodeIds = model.Nodes.Select(n => n.Id).ToList();

        // Phase 1: Cycle removal
        var acyclicEdges = CycleRemover.RemoveCycles(nodeIds, model.Edges);

        // Phase 2: Layer assignment (with dummy nodes)
        var (layers, edgesWithDummies, allNodes) = LayerAssigner.Assign(nodeIds, acyclicEdges);

        // Phase 3: Crossing minimization
        var layerLists = CrossingMinimizer.Minimize(layers, edgesWithDummies, allNodes);

        // Phase 4: Node positioning
        var nodeMap = model.Nodes.ToDictionary(n => n.Id);
        var nodeLayouts = NodePositioner.Position(layerLists, nodeMap, model.Direction, options);

        // Phase 5: Edge routing
        var edgeRoutes = EdgeRouter.Route(model.Edges, nodeLayouts, edgesWithDummies, model.Direction);

        // Copy real node layouts (skip dummies)
        foreach (var (id, layout) in nodeLayouts)
        {
            if (!id.StartsWith("__dummy_"))
                result.Nodes[id] = layout;
        }

        result.Edges.AddRange(edgeRoutes);

        // Calculate subgraph layouts
        foreach (var subgraph in model.Subgraphs)
        {
            var subgraphLayout = CalculateSubgraphBounds(subgraph, result.Nodes, options);
            if (subgraphLayout != null)
                result.Subgraphs[subgraph.Id] = subgraphLayout;
        }

        // Calculate total bounds
        CalculateTotalBounds(result, options);

        return result;
    }

    static SubgraphLayout? CalculateSubgraphBounds(
        SubgraphModel subgraph,
        Dictionary<string, NodeLayout> nodeLayouts,
        DiagramLayoutOptions options)
    {
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;
        var hasNodes = false;

        foreach (var nodeId in subgraph.NodeIds)
        {
            if (!nodeLayouts.TryGetValue(nodeId, out var layout))
                continue;

            hasNodes = true;
            minX = Math.Min(minX, layout.X);
            minY = Math.Min(minY, layout.Y);
            maxX = Math.Max(maxX, layout.X + layout.Width);
            maxY = Math.Max(maxY, layout.Y + layout.Height);
        }

        if (!hasNodes)
            return null;

        var padding = options.SubgraphPadding;
        return new SubgraphLayout
        {
            Id = subgraph.Id,
            Title = subgraph.Title,
            X = minX - padding,
            Y = minY - padding - 20, // extra space for title
            Width = maxX - minX + padding * 2,
            Height = maxY - minY + padding * 2 + 20
        };
    }

    static void CalculateTotalBounds(LayoutResult result, DiagramLayoutOptions options)
    {
        float maxX = 0, maxY = 0;

        foreach (var layout in result.Nodes.Values)
        {
            maxX = Math.Max(maxX, layout.X + layout.Width);
            maxY = Math.Max(maxY, layout.Y + layout.Height);
        }

        foreach (var sg in result.Subgraphs.Values)
        {
            maxX = Math.Max(maxX, sg.X + sg.Width);
            maxY = Math.Max(maxY, sg.Y + sg.Height);
        }

        result.TotalWidth = maxX + options.Margin;
        result.TotalHeight = maxY + options.Margin;
    }
}