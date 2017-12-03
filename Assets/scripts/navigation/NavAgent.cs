using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : NavObstacle {

    // Current Path
    // Includes every node between here and destination.
    // Current position is not included in path.
    private Queue<Vector2Int> path;

    private Vector2Int _destination;

    // Current Destination
    public Vector2Int destination {
        get {
            return _destination;
        }
    }

    void Reset() {
        navigationLayer = NavigationLayer.Player;
    }

    protected override void OnEnable() {
        base.OnEnable();
    }

    public void SetDestination(Vector2Int dest) {
        _destination = dest;
        // Do pathfinding
        RecalculatePath();

        /*Vector2Int startPos2 = mapController.WorldToCell(transform.position);
        Vector3 currPos = new Vector3(startPos2.x, startPos2.y, -1);
        while (path.Count > 0) {
            Vector2Int nextPos2 = path.Dequeue();
            Vector3 nextPos = new Vector3(nextPos2.x, nextPos2.y, -1);

            Debug.DrawLine(currPos, nextPos, Color.green, 0.2f);
            currPos = nextPos;
        }*/
    }

    public bool HasNextCell() {
        return path.Count > 0;
    }

    public Vector2Int PopNextCell() {
        return path.Dequeue();
    }

    public Vector2Int PeekNextCell() {
        return path.Peek();
    }

    public void RecalculatePath() {
        path = navController.CalculatePath(cell, destination);
    }
}
