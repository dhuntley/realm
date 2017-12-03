using System.Collections.Generic;
using UnityEngine;

public class AStar : PathfindingAlgorithm {
 
    public Queue<Vector2Int> CalculatePath(NavGraph graph, Vector2Int start, Vector2Int goal) {
        Vector2IntComparer comparer = new Vector2IntComparer();

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>(comparer);
        Dictionary<Vector2Int, float> g = new Dictionary<Vector2Int, float>(comparer);
        Dictionary<Vector2Int, float> f = new Dictionary<Vector2Int, float>(comparer);

        HashSet<Vector2Int> open = new HashSet<Vector2Int>(comparer);
        HashSet<Vector2Int> closed = new HashSet<Vector2Int>(comparer);

        g[start] = 0;
        f[start] = H(start, goal);
        open.Add(start);

        while(open.Count > 0) {
            Vector2Int current = GetCurrent(open, f);

            if (comparer.Equals(current, goal)) {
                return BuildPath(cameFrom, goal);
            }

            open.Remove(current);
            closed.Add(current);

            List<NavGraph.NavEdge> neighbors = graph.GetNeighbors(current);
            foreach (NavGraph.NavEdge edge in neighbors) {
                if (closed.Contains(edge.dest)) {
                    continue;
                }
                else if (!open.Contains(edge.dest)) {
                    open.Add(edge.dest);
                }

                float gScore = g[current] + edge.cost;
                if (g.ContainsKey(edge.dest) && gScore >= g[edge.dest]) {
                    continue;
                } else {
                    cameFrom[edge.dest] = current;
                    g[edge.dest] = gScore;
                    f[edge.dest] = gScore + H(edge.dest, goal);
                }
            }
        }

        return new Queue<Vector2Int>();
    }

    private Vector2Int GetCurrent(HashSet<Vector2Int> open, Dictionary<Vector2Int, float> f) {
        float minScore = Mathf.Infinity;
        Vector2Int minNode = new Vector2Int(int.MaxValue, int.MaxValue);

        foreach (Vector2Int node in open) {
            if (f[node] < minScore) {
                minScore = f[node];
                minNode = node;
            }
        }

        return minNode;
    }

    private Queue<Vector2Int> BuildPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int goal) {
        Stack<Vector2Int> reversePath = new Stack<Vector2Int>();
        reversePath.Push(goal);
        Vector2Int current = goal;
        while (cameFrom.ContainsKey(current)) {
            current = cameFrom[current];
            reversePath.Push(current);
        }

        Queue<Vector2Int> path = new Queue<Vector2Int>();
        while (reversePath.Count > 0) {
            path.Enqueue(reversePath.Pop());
        }

        return path;
    }

    private float H(Vector2Int a, Vector2Int b) {
        return Vector2Int.Distance(a, b);
    }
}
