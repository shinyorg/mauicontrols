using Shiny.Maui.Controls.MermaidDiagrams.Models;

namespace Shiny.Maui.Controls.MermaidDiagrams.Layout.Algorithms;

/// <summary>
/// Assigns x,y coordinates to nodes based on layer ordering and direction.
/// </summary>
static class NodePositioner
{
    public static Dictionary<string, NodeLayout> Position(
        List<List<string>> layerLists,
        Dictionary<string, DiagramNode> nodeMap,
        DiagramDirection direction,
        DiagramLayoutOptions options)
    {
        var result = new Dictionary<string, NodeLayout>();
        var isVertical = direction is DiagramDirection.TopToBottom or DiagramDirection.BottomToTop;

        for (var layerIdx = 0; layerIdx < layerLists.Count; layerIdx++)
        {
            var layer = layerLists[layerIdx];

            for (var posIdx = 0; posIdx < layer.Count; posIdx++)
            {
                var nodeId = layer[posIdx];
                var isDummy = nodeId.StartsWith("__dummy_");

                // Calculate node size based on label
                float width, height;
                var label = nodeId;

                if (!isDummy && nodeMap.TryGetValue(nodeId, out var node))
                {
                    label = node.Label;
                    width = MeasureTextWidth(label, options.FontSize);
                    width = Math.Max(width + 30, options.NodeWidth);
                    height = options.NodeHeight;

                    // Adjust size for shape
                    if (node.Shape is NodeShape.Circle)
                    {
                        var dim = Math.Max(width, height);
                        width = dim;
                        height = dim;
                    }
                    else if (node.Shape is NodeShape.Diamond)
                    {
                        width *= 1.4f;
                        height *= 1.4f;
                    }
                }
                else
                {
                    width = 10;
                    height = 10;
                }

                float x, y;
                if (isVertical)
                {
                    x = options.Margin + posIdx * (options.NodeWidth + options.HorizontalSpacing);
                    y = options.Margin + layerIdx * (options.NodeHeight + options.VerticalSpacing);
                }
                else
                {
                    x = options.Margin + layerIdx * (options.NodeWidth + options.HorizontalSpacing);
                    y = options.Margin + posIdx * (options.NodeHeight + options.VerticalSpacing);
                }

                // Reverse for BT / RL
                if (direction is DiagramDirection.BottomToTop)
                    y = options.Margin + (layerLists.Count - 1 - layerIdx) *
                        (options.NodeHeight + options.VerticalSpacing);
                else if (direction is DiagramDirection.RightToLeft)
                    x = options.Margin + (layerLists.Count - 1 - layerIdx) *
                        (options.NodeWidth + options.HorizontalSpacing);

                // Center node within its slot
                var slotWidth = isVertical
                    ? options.NodeWidth + options.HorizontalSpacing
                    : options.NodeWidth + options.HorizontalSpacing;
                x += (slotWidth - width) / 2;

                var shape = NodeShape.Rectangle;
                if (!isDummy && nodeMap.TryGetValue(nodeId, out var n))
                    shape = n.Shape;

                result[nodeId] = new NodeLayout
                {
                    Id = nodeId,
                    Label = label,
                    Shape = shape,
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height
                };
            }
        }

        return result;
    }

    static float MeasureTextWidth(string text, float fontSize)
    {
        return text.Length * fontSize * 0.6f;
    }
}