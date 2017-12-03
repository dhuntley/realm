using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavController {
    // Graph of nodes and agents reflecting locations of obstacles / agents
    private NavGraph graph = new NavGraph();

    private PathfindingAlgorithm algorithm = new AStar();

    // Map
    // - try and use the map just for bounds and terrain. Rely on locations of game objects
    // to decouple ourselves in terms of object locations etc.
    // - Use Grid on map to convert to int Vector locations though to make sure
    // our coordinate system is consistent
    private MapModel _mapModel;

    public MapModel mapModel {
        get {
            return _mapModel;
        }
        set {
            _mapModel = value;
            
            graph.mapModel = mapModel;

            // TODO: Make sure there isn't a lifecycle bug here.
            // We might be double registering agents.
        }
    }

    public Queue<Vector2Int> CalculatePath(Vector2Int start, Vector2Int dest) {
        return algorithm.CalculatePath(graph, start, dest);
    }
}
