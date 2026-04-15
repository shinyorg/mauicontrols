namespace Shiny.Maui.Controls.MermaidDiagrams.Layout.Algorithms;

/// <summary>
/// Assigns layers using longest-path algorithm. Inserts dummy nodes for edges spanning multiple layers.
/// </summary>
static class LayerAssigner
{
    public static (Dictionary<string, int> Layers, List<(string Source, string Target)> EdgesWithDummies, List<string>
        AllNodes)
        Assign(List<string> nodeIds, List<(string Source, string Target)> edges)
    {
        var adj = new Dictionary<string, List<string>>();
        var inDegree = new Dictionary<string, int>();

        foreach (var id in nodeIds)
        {
            adj[id] = [];
            inDegree[id] = 0;
        }

        foreach (var (source, target) in edges)
        {
            if (adj.ContainsKey(source) && inDegree.ContainsKey(target))
            {
                adj[source].Add(target);
                inDegree[target]++;
            }
        }

        // Longest path from sources
        var layers = new Dictionary<string, int>();
        var queue = new Queue<string>();

        foreach (var id in nodeIds)
        {
            if (inDegree[id] == 0)
            {
                queue.Enqueue(id);
                layers[id] = 0;
            }
        }

        // If no roots found (all nodes in cycles), start from first node
        if (queue.Count == 0 && nodeIds.Count > 0)
        {
            queue.Enqueue(nodeIds[0]);
            layers[nodeIds[0]] = 0;
        }

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            var currentLayer = layers[node];

            foreach (var neighbor in adj[node])
            {
                var newLayer = currentLayer + 1;
                if (!layers.ContainsKey(neighbor) || layers[neighbor] < newLayer)
                    layers[neighbor] = newLayer;

                inDegree[neighbor]--;
                if (inDegree[neighbor] <= 0)
                    queue.Enqueue(neighbor);
            }
        }

        // Ensure all nodes have a layer
        foreach (var id in nodeIds)
        {
            if (!layers.ContainsKey(id))
                layers[id] = 0;
        }

        // Insert dummy nodes for multi-layer edges
        var allNodes = new List<string>(nodeIds);
        var finalEdges = new List<(string Source, string Target)>();
        var dummyCount = 0;

        foreach (var (source, target) in edges)
        {
            if (!layers.ContainsKey(source) || !layers.ContainsKey(target))
            {
                finalEdges.Add((source, target));
                continue;
            }

            var span = layers[target] - layers[source];
            if (span <= 1)
            {
                finalEdges.Add((source, target));
            }
            else
            {
                var prev = source;
                for (var i = 1; i < span; i++)
                {
                    var dummyId = $"__dummy_{dummyCount++}";
                    allNodes.Add(dummyId);
                    layers[dummyId] = layers[source] + i;
                    finalEdges.Add((prev, dummyId));
                    prev = dummyId;
                }

                finalEdges.Add((prev, target));
            }
        }

        return (layers, finalEdges, allNodes);
    }
}