using UnityEngine;
using System.Collections.Generic;

public class NavAreaMap {

    private const int NAV_AREA_SIZE = 500;
    
    private Dictionary<Vector2Int, int> navAreaAssignments = new Dictionary<Vector2Int, int>(new Vector2IntComparer());
    
    private Dictionary<int, Color> navAreaColors = new Dictionary<int, Color>();

    private NavGraph navGraph;

    public NavAreaMap(NavGraph navGraph) {
        Debug.Assert(navGraph != null);
        this.navGraph = navGraph;

        Vector2IntComparer comparer = new Vector2IntComparer();
        
        int currentNavAreaId = 0;

        Vector2Int originCell;

        while (GetUnassignedCell(out originCell)) {
            List<Vector2Int> currentNavArea = new List<Vector2Int>();
            Queue<Vector2Int> open = new Queue<Vector2Int>();

            open.Enqueue(originCell);

            while (currentNavArea.Count < NAV_AREA_SIZE) {
                if (open.Count == 0) {
                    break;
                }

                Vector2Int currentTile = open.Dequeue();
                currentNavArea.Add(currentTile);

                List<NavGraph.NavEdge> neighbors = navGraph.GetNeighbors(currentTile);
                foreach (NavGraph.NavEdge edge in neighbors) {
                    if (!currentNavArea.Contains(edge.dest)
                            && !open.Contains(edge.dest)
                            && !navAreaAssignments.ContainsKey(edge.dest)) {
                        open.Enqueue(edge.dest);
                    }
                }
            }

            foreach (Vector2Int cell in currentNavArea) {
                navAreaAssignments.Add(cell, currentNavAreaId);
            }
            navAreaColors.Add(currentNavAreaId, Random.ColorHSV());
            currentNavAreaId++;
        }
    }

    private bool GetUnassignedCell(out Vector2Int unassignedCell) {
        foreach (Vector2Int cell in navGraph.mapModel.tiles.Keys) {
            if (navGraph.mapModel.Traversable(cell) && !navAreaAssignments.ContainsKey(cell)) {
                unassignedCell = cell;
                return true;
            }
        }
        unassignedCell = new Vector2Int(int.MinValue, int.MinValue);
        return false;
    }

    public Dictionary<Vector2Int, int> GetNavAreaAssignments() {
        return navAreaAssignments;
    }

    public Color GetNavAreaColor(int navAreaId) {
        return navAreaColors[navAreaId];
    }
}
