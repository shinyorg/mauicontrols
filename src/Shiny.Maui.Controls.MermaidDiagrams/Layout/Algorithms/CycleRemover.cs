using Shiny.Maui.Controls.MermaidDiagrams.Models;

namespace Shiny.Maui.Controls.MermaidDiagrams.Layout.Algorithms;

/// <summary>
/// DFS-based cycle removal. Identifies back edges and reverses them.
/// </summary>
static class CycleRemover
{
    public static List<(string Source, string Target)> RemoveCycles(
        List<string> nodeIds,
        List<DiagramEdge> edges)
    {
        var result = new List<(string Source, string Target)>();
        var visited = new HashSet<string>();
        var inStack = new HashSet<string>();
        var backEdges = new HashSet<(string, string)>();

        // Build adjacency
        var adj = new Dictionary<string, List<string>>();
        foreach (var id in nodeIds)
            adj[id] = [];
        foreach (var e in edges)
        {
            if (adj.ContainsKey(e.SourceId))
                adj[e.SourceId].Add(e.TargetId);
        }

        foreach (var id in nodeIds)
        {
            if (!visited.Contains(id))
                Dfs(id, adj, visited, inStack, backEdges);
        }

        // Return edges with back edges reversed
        foreach (var e in edges)
        {
            if (backEdges.Contains((e.SourceId, e.TargetId)))
                result.Add((e.TargetId, e.SourceId));
            else
                result.Add((e.SourceId, e.TargetId));
        }

        return result;
    }

    static void Dfs(
        string node,
        Dictionary<string, List<string>> adj,
        HashSet<string> visited,
        HashSet<string> inStack,
        HashSet<(string, string)> backEdges)
    {
        visited.Add(node);
        inStack.Add(node);

        foreach (var neighbor in adj[node])
        {
            if (!visited.Contains(neighbor))
            {
                Dfs(neighbor, adj, visited, inStack, backEdges);
            }
            else if (inStack.Contains(neighbor))
            {
                backEdges.Add((node, neighbor));
            }
        }

        inStack.Remove(node);
    }
}