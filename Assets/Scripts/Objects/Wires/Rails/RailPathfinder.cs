using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RailPathfinder
{
    public List<RailNode> nodes;
    public RailPathfinder(List<RailNode> nodes)
    {
        this.nodes = nodes;
    }
    public List<RailNode> FindPath(RailNode start, RailNode end)
    {
        var queue = new Queue<RailNode>();
        var previous = new Dictionary<RailNode, RailNode>();

        queue.Enqueue(start);
        previous[start] = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current == end)
            {
                var path = new List<RailNode>();
                for (var n = end; n != null; n = previous[n]) { path.Add(n); }

                path.Reverse();
                return path;
            }

            List<RailNode> potential_nexts = nodes.Where(n => !n.Full && Vector2Int.Distance(n.RailPosition, current.RailPosition) == 1).ToList();
            foreach (var next in potential_nexts)
            {
                if (previous.ContainsKey(next)) { continue; }

                previous[next] = current;
                queue.Enqueue(next);
            }
        }

        return new List<RailNode>(); // No path found
    }
}