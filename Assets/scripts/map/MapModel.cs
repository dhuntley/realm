using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class MapModel {
    public Dictionary<Vector2Int, MapTile> tiles;

    // Dynamic agents, indexed by GameObject InstanceId
    private Dictionary<int, Unit> units = new Dictionary<int, Unit>();

    // Agent locations
    private Dictionary<int, Vector2Int> unitPositionsForward = new Dictionary<int, Vector2Int>();
    private Dictionary<Vector2Int, int> unitPositionsReverse = new Dictionary<Vector2Int, int>(new Vector2IntComparer());

    // Structures, which may occupy more than one tile
    private Dictionary<int, Structure> structures = new Dictionary<int, Structure>();

    // Structure locations
    private Dictionary<int, HashSet<Vector2Int>> structurePositionsForward = new Dictionary<int, HashSet<Vector2Int>>();
    private Dictionary<Vector2Int, int> structurePositionsReverse = new Dictionary<Vector2Int, int>(new Vector2IntComparer());


    /*public MapModel() {
        tiles = new MapTile[20, 20];
        for (int x=0; x<tiles.GetLength(0); x++) {
            for (int y=0; y<tiles.GetLength(1); y++) {
                tiles[x, y] = new MapTile((x+y) % 2 == 0);
            }
        }
    }*/

    public MapModel(Tilemap tilemap) {
        tiles = new Dictionary<Vector2Int, MapTile>();
        BoundsInt tileMapBounds = tilemap.cellBounds;
        for (int x = tileMapBounds.xMin; x <= tileMapBounds.xMax; x++) {
            for (int y = tileMapBounds.yMin; y <= tileMapBounds.yMax; y++) {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile != null && tile.name != null) {
                    bool isWall = tile.name.Equals("checker_1");
                    tiles[new Vector2Int(x, y)] = new MapTile(!isWall, isWall);
                } else {
                    tiles[new Vector2Int(x, y)] = new MapTile(false, true);
                }

            }
        }
    }

    public bool Traversable(Vector2Int cell) {
        return tiles.ContainsKey(cell) && tiles[cell].traversable;
    }

    public bool HardCorners(Vector2Int cell) {
        return !tiles.ContainsKey(cell) || tiles[cell].hardCorners || ContainsStructure(cell);
    }

    /* Units */

    public void AddUnit(Unit unit, Vector2Int position) {
        units[unit.GetInstanceID()] = unit;

        unitPositionsForward[unit.GetInstanceID()] = position;
        unitPositionsReverse[position] = unit.GetInstanceID();
    }

    public void RemoveUnit(Unit unit) {
        Debug.Assert(unitPositionsForward.ContainsKey(unit.GetInstanceID()));
        Debug.Assert(units.ContainsKey(unit.GetInstanceID()));
        Debug.Assert(unitPositionsReverse.ContainsValue(unit.GetInstanceID()));

        units.Remove(unit.GetInstanceID());

        unitPositionsReverse.Remove(unitPositionsForward[unit.GetInstanceID()]);
        unitPositionsForward.Remove(unit.GetInstanceID());
    }

    public bool UpdateUnitPosition(Unit unit, Vector2Int dest) {
        if (!unitPositionsReverse.ContainsKey(dest) || unitPositionsReverse[dest] == unit.GetInstanceID()) {
            Debug.Assert(units.ContainsKey(unit.GetInstanceID()));
            Debug.Assert(unitPositionsReverse.ContainsValue(unit.GetInstanceID()));
            Debug.Assert(unitPositionsForward.ContainsKey(unit.GetInstanceID()));

            unitPositionsReverse.Remove(unitPositionsForward[unit.GetInstanceID()]);
            unitPositionsForward[unit.GetInstanceID()] = dest;
            unitPositionsReverse[dest] = unit.GetInstanceID();
            return true;
        } else {
            return false;
        }
    }

    public bool ContainsUnit(Vector2Int cell) {
        return unitPositionsReverse.ContainsKey(cell);
    }

    public bool GetUnit(Vector2Int cell) {
        return units[unitPositionsReverse[cell]];
    }

    public Vector2Int GetUnitPosition(Unit unit) {
        return unitPositionsForward[unit.GetInstanceID()];
    }

    /* Structures */
    public void AddStructure(Structure structure, HashSet<Vector2Int> positions) {
        structures[structure.GetInstanceID()] = structure;

        structurePositionsForward[structure.GetInstanceID()] = positions;
        foreach (Vector2Int position in positions) {
            structurePositionsReverse[position] = structure.GetInstanceID();
        }
    }

    public void RemoveStructure(Structure structure) {
        Debug.Assert(structurePositionsForward.ContainsKey(structure.GetInstanceID()));
        Debug.Assert(structures.ContainsKey(structure.GetInstanceID()));
        Debug.Assert(structurePositionsReverse.ContainsValue(structure.GetInstanceID()));

        structures.Remove(structure.GetInstanceID());

        HashSet<Vector2Int> positions = structurePositionsForward[structure.GetInstanceID()];
        foreach (Vector2Int position in positions) {
            structurePositionsReverse.Remove(position);
        }
        unitPositionsForward.Remove(structure.GetInstanceID());
    }

    public bool ContainsStructure(Vector2Int cell) {
        return structurePositionsReverse.ContainsKey(cell);
    }

    public bool GetStructure(Vector2Int cell) {
        return structures[structurePositionsReverse[cell]];
    }

    public HashSet<Vector2Int> GetStructurePositions(Structure structure) {
        return structurePositionsForward[structure.GetInstanceID()];
    }

    public bool IsOccupied(Vector2Int cell) {
        return ContainsStructure(cell) || ContainsUnit(cell) || !Traversable(cell);
    }
}
