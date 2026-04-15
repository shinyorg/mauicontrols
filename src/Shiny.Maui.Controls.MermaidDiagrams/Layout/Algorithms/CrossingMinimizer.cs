namespace Shiny.Maui.Controls.MermaidDiagrams.Layout.Algorithms;

/// <summary>
/// Barycenter heuristic for crossing minimization between adjacent layers.
/// </summary>
static class CrossingMinimizer
{
    public static List<List<string>> Minimize(
        Dictionary<string, int> layers,
        List<(string Source, string Target)> edges,
        List<string> allNodes)
    {
        // Group nodes by layer
        var maxLayer = 0;
        foreach (var kvp in layers)
        {
            if (kvp.Value > maxLayer)
                maxLayer = kvp.Value;
        }

        var layerLists = new List<List<string>>();
        for (var i = 0; i <= maxLayer; i++)
            layerLists.Add([]);

        foreach (var node in allNodes)
        {
            if (layers.TryGetValue(node, out var layer))
                layerLists[layer].Add(node);
        }

        // Build adjacency for barycenter calculation
        var downNeighbors = new Dictionary<string, List<string>>();
        var upNeighbors = new Dictionary<string, List<string>>();
        foreach (var node in allNodes)
        {
            downNeighbors[node] = [];
            upNeighbors[node] = [];
        }

        foreach (var (source, target) in edges)
        {
            if (downNeighbors.ContainsKey(source))
                downNeighbors[source].Add(target);
            if (upNeighbors.ContainsKey(target))
                upNeighbors[target].Add(source);
        }

        // Run barycenter heuristic (multiple passes)
        for (var iteration = 0; iteration < 4; iteration++)
        {
            // Forward pass
            for (var layer = 1; layer < layerLists.Count; layer++)
            {
                var prevLayer = layerLists[layer - 1];
                SortByBarycenter(layerLists[layer], prevLayer, upNeighbors);
            }

            // Backward pass
            for (var layer = layerLists.Count - 2; layer >= 0; layer--)
            {
                var nextLayer = layerLists[layer + 1];
                SortByBarycenter(layerLists[layer], nextLayer, downNeighbors);
            }
        }

        return layerLists;
    }

    static void SortByBarycenter(
        List<string> currentLayer,
        List<string> referenceLayer,
        Dictionary<string, List<string>> neighbors)
    {
        var positionMap = new Dictionary<string, int>();
        for (var i = 0; i < referenceLayer.Count; i++)
            positionMap[referenceLayer[i]] = i;

        var barycenters = new Dictionary<string, float>();
        foreach (var node in currentLayer)
        {
            var relevantNeighbors = neighbors[node];
            if (relevantNeighbors.Count == 0)
            {
                barycenters[node] = float.MaxValue;
                continue;
            }

            var sum = 0f;
            var count = 0;
            foreach (var n in relevantNeighbors)
            {
                if (positionMap.TryGetValue(n, out var p))
                {
                    sum += p;
                    count++;
                }
            }

            barycenters[node] = count > 0 ? sum / count : float.MaxValue;
        }

        currentLayer.Sort((a, b) => barycenters[a].CompareTo(barycenters[b]));
    }
}