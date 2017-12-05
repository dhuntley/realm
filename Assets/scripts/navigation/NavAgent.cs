using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : NavObstacle {

    // Current Path
    // Includes every node between here and destination.
    // Current position is not included in path.
    private Queue<Vector2Int> path = new Queue<Vector2Int>();

    private Vector2Int _destination;

    // Current Destination
    public Vector2Int destination {
        get {
            return _destination;
        }
    }

    private Unit unit;

    void Reset() {
        navigationLayer = NavigationLayer.Player;
    }

    private void Start() {
        unit = GetComponent<Unit>();
    }

    protected override void OnEnable() {
        base.OnEnable();
    }

    public bool SetDestination(Vector2Int dest) {
        _destination = dest;
        // Do pathfinding
        return RecalculatePath();

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

    public void ClearPath() {
        path.Clear();
    }

    public Vector2Int PopNextCell() {
        return path.Dequeue();
    }

    public Vector2Int PeekNextCell() {
        return path.Peek();
    }

    public bool RecalculatePath() {
        Queue<Vector2Int> tempPath = navController.CalculatePath(unit.cell, destination);
        if (tempPath.Count > 0) {
            path = tempPath;
            return true;
        }
        return false;
    }
}
