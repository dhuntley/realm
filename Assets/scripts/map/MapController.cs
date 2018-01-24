using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Clean up the interface of MapController / MapModel.
// It is muddy how other classes are supposed to interact with these two.

public class MapController : Singleton<MapController> {

    private Grid _mapGrid;

    private MapModel _mapModel;

    public MapModel mapModel {
        get { return _mapModel; }
    }

    public Grid mapGrid {
        get {
            return _mapGrid;
        }
    }

    private NavController _navController;

    public NavController navController {
        get {
            return _navController;
        }
    }

    private Tilemap terrainTilemap;

    public GameObject stumpPrefab;

    public GameObject treePrefab;

    protected MapController() { }

    // Use this for initialization
    void Awake() {
        _navController = new NavController();

        GameObject mapGridGameObject = GameObject.FindWithTag("MapGrid");

        if (mapGridGameObject == null) {
            Debug.LogError("Could not find mapGrid by tag to initialize MapController.");
        }

        _mapGrid = mapGridGameObject.GetComponentInChildren<Grid>();
        if (_mapGrid == null) {
            Debug.LogError("Could not find Grid on GameObject with 'Map' tag.");
        }

        // Init map model from terrain tilemap
        terrainTilemap = GameObject.FindGameObjectWithTag("TerrainTilemap").GetComponent<Tilemap>();
        if (terrainTilemap == null) {
            Debug.LogError("Could not find Tilemap on GameObject with 'Map' tag.");
        }

        _mapModel = new MapModel(terrainTilemap);

        // Spawn trees based on tree tilemap
        GameObject treeContainer = new GameObject("Trees");
        treeContainer.tag = "Trees";
        GameObject treeTileMapGameObject = GameObject.FindGameObjectWithTag("TreeTilemap");
        Tilemap treeTilemap = treeTileMapGameObject.GetComponent<Tilemap>();
        if (treeTilemap == null) {
            Debug.LogError("Could not find Tilemap on GameObject with 'Map' tag.");
        }
        BoundsInt tileMapBounds = treeTilemap.cellBounds;
        for (int x = tileMapBounds.xMin; x <= tileMapBounds.xMax; x++) {
            for (int y = tileMapBounds.yMin; y <= tileMapBounds.yMax; y++) {
                TileBase treeTile = treeTilemap.GetTile(new Vector3Int(x, y, 0));
                if (treeTile) {
                    Vector2Int treeCell = new Vector2Int(x, y);
                    Vector3 treeWorldPosition = GetCellCenterWorld(treeCell);
                    GameObject treeGameObject = GameObject.Instantiate(treePrefab, treeWorldPosition, Quaternion.identity, treeContainer.transform);
                    treeGameObject.GetComponent<SpriteRenderer>().sortingOrder = Mathf.FloorToInt(-4 * treeWorldPosition.y);
                    _mapModel.AddTree(treeGameObject, treeCell);
                }
            }
        }

        GameObject.Destroy(treeTileMapGameObject);

        // Initialize navController after Trees are placed
        navController.mapModel = _mapModel;
    }

    public Vector2Int WorldToCell(Vector3 position) {
        if (_mapGrid != null) {
            Vector3Int cell = _mapGrid.WorldToCell(position);
            return new Vector2Int(cell.x, cell.y);
        } else {
            return new Vector2Int(int.MinValue, int.MinValue);
        }
    }

    public Vector3 GetCellCenterWorld(Vector2Int position) {
        if (_mapGrid != null) {
            return _mapGrid.GetCellCenterWorld(new Vector3Int(position.x, position.y, 0));
        } else {
            return Vector3.negativeInfinity;
        }
    }

    public void RemoveTree(Vector2Int cell) {
        mapModel.RemoveTree(cell);
        Vector3 treeWorldPosition = GetCellCenterWorld(cell);
        GameObject treeContainer = GameObject.FindGameObjectWithTag("Trees");
        GameObject treeGameObject = GameObject.Instantiate(stumpPrefab, treeWorldPosition, Quaternion.identity, treeContainer.transform);
    }

    // Units

    public void AddUnit(Unit unit) {
        _mapModel.AddUnit(unit, WorldToCell(unit.transform.position));
    }

    public void RemoveUnit(Unit unit) {
        _mapModel.RemoveUnit(unit);
    }

    public bool UpdateUnitPosition(Unit unit, Vector2Int dest) {
        return _mapModel.UpdateUnitPosition(unit, dest);
    }

    public Vector2Int GetUnitPosition(Unit unit) {
        return _mapModel.GetUnitPosition(unit);
    }

    public Vector3 GetUnitPositionWorld(Unit unit) {
        return GetCellCenterWorld(_mapModel.GetUnitPosition(unit));
    }

    // Structures

    public void AddStructure(Structure structure) {
        HashSet<Vector2Int> positions = WorldToStructurePositions(structure.transform.position, structure.width, structure.length);
        _mapModel.AddStructure(structure, positions);
    }

    public void RemoveStructure(Structure structure) {
        _mapModel.RemoveStructure(structure);
    }

    public HashSet<Vector2Int> WorldToStructurePositions(Vector3 worldPosition, int structWidth, int structLength) {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>(new Vector2IntComparer());
        Vector2Int cell = WorldToCell(worldPosition);

        int minX = cell.x - Mathf.FloorToInt(structWidth / 2.0f);
        int maxX = cell.x + Mathf.CeilToInt(structWidth / 2.0f) - 1;

        int minY = cell.y - Mathf.FloorToInt(structLength / 2.0f);
        int maxY = cell.y + Mathf.CeilToInt(structLength / 2.0f) - 1;

        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                positions.Add(new Vector2Int(x, y));
            }
        }
        return positions;
    }

    public Vector3 GetStructurePositionWorld(HashSet<Vector2Int> positions) {
        if (positions.Count > 0) {
            Vector3 average = Vector3.zero;
            foreach (Vector2Int position in positions) {
                average += GetCellCenterWorld(position);
            }

            return average / positions.Count;
        } else {
            return Vector3.negativeInfinity;
        }
    }

    public Vector3 GetStructurePositionWorld(Structure structure) {
        return GetStructurePositionWorld(_mapModel.GetStructurePositions(structure));
    }

    public List<Vector3> GetStructurePositionsWorld(Structure structure) {
        HashSet<Vector2Int> positions = _mapModel.GetStructurePositions(structure);
        List<Vector3> worldPositions = new List<Vector3>();

        foreach (Vector2Int position in positions) {
            worldPositions.Add(GetCellCenterWorld(position));
        }

        return worldPositions;
    }

    /*private void OnDrawGizmos() {
        Dictionary<Vector2Int, int> navAreaAssignments = navController.navGraph.navAreaMap.GetNavAreaAssignments();
        foreach (Vector2Int cell in navAreaAssignments.Keys) {
            Gizmos.color = navController.navGraph.navAreaMap.GetNavAreaColor(navAreaAssignments[cell]);
            Gizmos.DrawCube(GetCellCenterWorld(cell), Vector3.one / 5);
        }
    }*/
}
