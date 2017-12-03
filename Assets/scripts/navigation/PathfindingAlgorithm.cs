using System.Collections.Generic;
using UnityEngine;

public interface PathfindingAlgorithm {
    Queue<Vector2Int> CalculatePath(NavGraph graph, Vector2Int start, Vector2Int dest);
}
