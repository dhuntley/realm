﻿using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class MapController : MonoBehaviour {

    private Grid _mapGrid;

    private MapModel _mapModel;

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

    private Tilemap tilemap;
    
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

        tilemap = mapGridGameObject.GetComponentInChildren<Tilemap>();
        if (tilemap == null) {
            Debug.LogError("Could not find Tilemap on GameObject with 'Map' tag.");
        }

        _mapModel = new MapModel(tilemap);
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

    public void AddStructure(Structure structure) {
        HashSet<Vector2Int> positions = new HashSet<Vector2Int>();

        Vector2Int cell = WorldToCell(structure.transform.position);

        int minX = cell.x - structure.width / 2;
        int maxX = cell.x + structure.width / 2 - 1;

        int minY = cell.y - structure.length / 2;
        int maxY = cell.y + structure.length / 2 - 1;

        for (int x = minX; x <= maxX; x++) {
            for (int y = minY; y <= maxY; y++) {
                positions.Add(new Vector2Int(x, y));
            }
        }
        
        _mapModel.AddStructure(structure, positions);
    }

    public void RemoveStructure(Structure structure) {
        _mapModel.RemoveStructure(structure);
    }

    public Vector3 GetStructurePositionWorld(Structure structure) {
        HashSet<Vector2Int> positions = _mapModel.GetStructurePositions(structure);

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

    public List<Vector3> GetStructurePositionsWorld(Structure structure) {
        HashSet<Vector2Int> positions = _mapModel.GetStructurePositions(structure);
        List<Vector3> worldPositions = new List<Vector3>();

        foreach (Vector2Int position in positions) {
            worldPositions.Add(GetCellCenterWorld(position));
        }

        return worldPositions;
    }
}
