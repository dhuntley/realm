using System.Collections.Generic;
using UnityEngine;

public class NavGraph {

    // Static obstacles and their locations
    //private Dictionary<int, NavObstacle> obstacles = new Dictionary<int, NavObstacle>();

    // Obstacle locations
    //private Dictionary<Vector2Int, NavObstacle> obstaclePositions = new Dictionary<Vector2Int, NavObstacle>(new Vector2IntComparer());

    // Dynamic agents, indexed by GameObject InstanceId
    //private Dictionary<int, NavAgent> agents = new Dictionary<int, NavAgent>();

    // Agent locations
    //private Dictionary<Vector2Int, NavAgent> agentPositions = new Dictionary<Vector2Int, NavAgent>(new Vector2IntComparer());

    public struct NavEdge {
        public Vector2Int start;
        public Vector2Int dest;
        public float cost;

        public NavEdge(Vector2Int s, Vector2Int d, float c) {
            start = s;
            dest = d;
            cost = c;
        }
    }

    private const float DIAG_COST = 1.414f;
    private const float ORTH_COST = 1.0f;
    private const float OBST_COST = 40.0f;  // The simulated cost to discourage a route blocked by an obstacle or agent

    public MapModel mapModel;

    public List<NavEdge> GetNeighbors(Vector2Int node) {
        List<NavEdge> neighbors = new List<NavEdge>();

        Vector2Int ul = new Vector2Int(node.x - 1, node.y + 1);
        Vector2Int u = new Vector2Int(node.x, node.y + 1);
        Vector2Int ur = new Vector2Int(node.x + 1, node.y + 1);
        Vector2Int l = new Vector2Int(node.x - 1, node.y);
        Vector2Int r = new Vector2Int(node.x + 1, node.y);
        Vector2Int dl = new Vector2Int(node.x - 1, node.y - 1);
        Vector2Int d = new Vector2Int(node.x, node.y - 1);
        Vector2Int dr = new Vector2Int(node.x + 1, node.y - 1);

        //Test Up-left
        if (mapModel.Traversable(ul) && !mapModel.ContainsStructure(ul) && !mapModel.HardCorners(l) && !mapModel.HardCorners(u)) {
            if (mapModel.ContainsUnit(ul)) {
                neighbors.Add(new NavEdge(node, ul, OBST_COST));
            } else {
                neighbors.Add(new NavEdge(node, ul, DIAG_COST));
            }
        }

        //Test Up
        if (mapModel.Traversable(u) && !mapModel.ContainsStructure(u)) {
            if (mapModel.ContainsUnit(u)) {
                neighbors.Add(new NavEdge(node, u, OBST_COST));
            } else {
                neighbors.Add(new NavEdge(node, u, ORTH_COST));
            }
        }

        //Test Up-right
        if (mapModel.Traversable(ur) && !mapModel.ContainsStructure(ur) && !mapModel.HardCorners(r) && !mapModel.HardCorners(u)) {
            if (mapModel.ContainsUnit(ur)) {
                neighbors.Add(new NavEdge(node, ur, OBST_COST));
            } else {
                neighbors.Add(new NavEdge(node, ur, ORTH_COST));
            }
        }

        //Test Left
        if (mapModel.Traversable(l) && !mapModel.ContainsStructure(l)) {
            if (mapModel.ContainsUnit(l)) {
                neighbors.Add(new NavEdge(node, l, OBST_COST));
            } else {
                neighbors.Add(new NavEdge(node, l, ORTH_COST));
            }
        }

        //Test Right
        if (mapModel.Traversable(r) && !mapModel.ContainsStructure(r)) {
            if (mapModel.ContainsUnit(r)) {
                neighbors.Add(new NavEdge(node, r, OBST_COST));
            } else {
                neighbors.Add(new NavEdge(node, r, ORTH_COST));
            }
        }

        //Test Down-left
        if (mapModel.Traversable(dl) && !mapModel.ContainsStructure(dl) && !mapModel.HardCorners(l) && !mapModel.HardCorners(d)) {
            if (mapModel.ContainsUnit(dl)) {
                neighbors.Add(new NavEdge(node, dl, OBST_COST));
            } else {
                neighbors.Add(new NavEdge(node, dl, DIAG_COST));
            }
        }

        //Test Down
        if (mapModel.Traversable(d) && !mapModel.ContainsStructure(d)) {
            if (mapModel.ContainsUnit(d)) {
                neighbors.Add(new NavEdge(node, d, OBST_COST));
            } else {
                neighbors.Add(new NavEdge(node, d, ORTH_COST));
            }
        }

        //Test Down-right
        if (mapModel.Traversable(dr) && !mapModel.ContainsStructure(dr) && !mapModel.HardCorners(r) && !mapModel.HardCorners(d)) {
            if (mapModel.ContainsUnit(dr)) {
                neighbors.Add(new NavEdge(node, dr, OBST_COST));
            } else {
                neighbors.Add(new NavEdge(node, dr, DIAG_COST));
            }
        }

        return neighbors;
    }
}
