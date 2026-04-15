using Shiny.Maui.Controls.MermaidDiagrams.Models;

namespace Shiny.Maui.Controls.MermaidDiagrams.Layout.Algorithms;

/// <summary>
/// Routes edges as polylines with shape-aware connection points.
/// </summary>
static class EdgeRouter
{
    public static List<EdgeRoute> Route(
        List<DiagramEdge> originalEdges,
        Dictionary<string, NodeLayout> nodeLayouts,
        List<(string Source, string Target)> layoutEdges,
        DiagramDirection direction)
    {
        var routes = new List<EdgeRoute>();

        foreach (var edge in originalEdges)
        {
            if (!nodeLayouts.TryGetValue(edge.SourceId, out var sourceLayout) ||
                !nodeLayouts.TryGetValue(edge.TargetId, out var targetLayout))
                continue;

            var route = new EdgeRoute
            {
                SourceId = edge.SourceId,
                TargetId = edge.TargetId,
                Label = edge.Label,
                Style = edge.Style,
                ArrowType = edge.ArrowType
            };

            // Find connection points on source and target shapes
            var (sx, sy) = GetConnectionPoint(sourceLayout, targetLayout.CenterX, targetLayout.CenterY, false);
            var (tx, ty) = GetConnectionPoint(targetLayout, sourceLayout.CenterX, sourceLayout.CenterY, true);

            route.Points.Add(new PointF(sx, sy));

            // Collect intermediate dummy points
            CollectDummyPoints(route, edge.SourceId, edge.TargetId, layoutEdges, nodeLayouts);

            route.Points.Add(new PointF(tx, ty));

            routes.Add(route);
        }

        return routes;
    }

    static void CollectDummyPoints(
        EdgeRoute route,
        string sourceId,
        string targetId,
        List<(string Source, string Target)> layoutEdges,
        Dictionary<string, NodeLayout> nodeLayouts)
    {
        // Walk through dummy nodes between source and target
        var current = sourceId;
        var visited = new HashSet<string> { current };

        while (true)
        {
            string? next = null;
            foreach (var (s, t) in layoutEdges)
            {
                if (s == current && !visited.Contains(t))
                {
                    if (t.StartsWith("__dummy_"))
                    {
                        next = t;
                        break;
                    }

                    if (t == targetId)
                        return; // reached target directly
                }
            }

            if (next == null)
                break;

            visited.Add(next);
            if (nodeLayouts.TryGetValue(next, out var dummy))
                route.Points.Add(new PointF(dummy.CenterX, dummy.CenterY));

            current = next;
        }
    }

    static (float X, float Y) GetConnectionPoint(
        NodeLayout node,
        float towardX,
        float towardY,
        bool isTarget)
    {
        var cx = node.CenterX;
        var cy = node.CenterY;
        var hw = node.Width / 2;
        var hh = node.Height / 2;

        return node.Shape switch
        {
            NodeShape.Circle => GetCircleConnection(cx, cy, hw, towardX, towardY),
            NodeShape.Diamond => GetDiamondConnection(cx, cy, hw, hh, towardX, towardY),
            _ => GetRectConnection(cx, cy, hw, hh, towardX, towardY)
        };
    }

    static (float X, float Y) GetRectConnection(float cx, float cy, float hw, float hh, float tx, float ty)
    {
        var dx = tx - cx;
        var dy = ty - cy;

        if (Math.Abs(dx) < 0.001f && Math.Abs(dy) < 0.001f)
            return (cx, cy + hh);

        // Use the edge that's most aligned with the direction
        if (Math.Abs(dy) * hw > Math.Abs(dx) * hh)
        {
            // Top or bottom edge
            var sign = dy > 0 ? 1f : -1f;
            return (cx + dx * hh / Math.Abs(dy) * sign / Math.Abs(sign), cy + hh * sign);
        }
        else
        {
            // Left or right edge
            var sign = dx > 0 ? 1f : -1f;
            return (cx + hw * sign, cy + dy * hw / Math.Abs(dx) * sign / Math.Abs(sign));
        }
    }

    static (float X, float Y) GetCircleConnection(float cx, float cy, float r, float tx, float ty)
    {
        var dx = tx - cx;
        var dy = ty - cy;
        var dist = MathF.Sqrt(dx * dx + dy * dy);

        if (dist < 0.001f)
            return (cx, cy + r);

        return (cx + dx / dist * r, cy + dy / dist * r);
    }

    static (float X, float Y) GetDiamondConnection(float cx, float cy, float hw, float hh, float tx, float ty)
    {
        var dx = tx - cx;
        var dy = ty - cy;

        if (Math.Abs(dx) < 0.001f && Math.Abs(dy) < 0.001f)
            return (cx, cy + hh);

        // Diamond edges are at 45 degrees from center
        var absDx = Math.Abs(dx);
        var absDy = Math.Abs(dy);

        float ix, iy;
        if (absDx * hh > absDy * hw)
        {
            var sign = dx > 0 ? 1f : -1f;
            ix = cx + hw * sign;
            iy = cy + dy * hw / absDx * sign / Math.Abs(sign);
        }
        else
        {
            var sign = dy > 0 ? 1f : -1f;
            ix = cx + dx * hh / absDy * sign / Math.Abs(sign);
            iy = cy + hh * sign;
        }

        return (ix, iy);
    }
}