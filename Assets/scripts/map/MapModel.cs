using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class MapModel {
    // TODO: Consider splitting ownership of trees to a separate sub-map that the MapModel owns
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

    // Trees
    private Dictionary<Vector2Int, GameObject> trees = new Dictionary<Vector2Int, GameObject>();
    
    /*public MapModel() {
        tiles = new MapTile[20, 20];
        for (int x=0; x<tiles.GetLength(0); x++) {
            for (int y=0; y<tiles.GetLength(1); y++) {
                tiles[x, y] = new MapTile((x+y) % 2 == 0);
            }
        }
    }*/

    public MapModel(Tilemap terrainTilemap) {
        tiles = new Dictionary<Vector2Int, MapTile>();
        BoundsInt tileMapBounds = terrainTilemap.cellBounds;
        for (int x = tileMapBounds.xMin; x <= tileMapBounds.xMax; x++) {
            for (int y = tileMapBounds.yMin; y <= tileMapBounds.yMax; y++) {
                TileBase terrainTile = terrainTilemap.GetTile(new Vector3Int(x, y, 0));
                if (terrainTile != null && terrainTile.name != null) {
                    bool isWall = terrainTile.name.Equals("checker_1");
                    tiles[new Vector2Int(x, y)] = new MapTile(!isWall, isWall);
                } else {
                    tiles[new Vector2Int(x, y)] = new MapTile(false, true);
                }
            }
        }

    }

    public bool Traversable(Vector2Int cell) {
        return tiles.ContainsKey(cell) && tiles[cell].traversable && !ContainsTree(cell);
    }

    public bool HardCorners(Vector2Int cell) {
        return !tiles.ContainsKey(cell) || tiles[cell].hardCorners || ContainsTree(cell) || ContainsStructure(cell);
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

    public Structure GetStructure(Vector2Int cell) {
        return structures[structurePositionsReverse[cell]];
    }

    public HashSet<Vector2Int> GetStructurePositions(Structure structure) {
        return structurePositionsForward[structure.GetInstanceID()];
    }

    public Structure GetClosestStructure(Vector2Int cell) {
        float minDistance = float.MaxValue;
        Structure closestStructure = null;
        Vector3 cellPosition = new Vector3(cell.x, cell.y);

        foreach (int key in structures.Keys) {
            Vector2Int structureCell = structurePositionsForward[key].GetEnumerator().Current;
            float sqrDistance = (cellPosition - new Vector3(structureCell.x, structureCell.y)).sqrMagnitude;

            if (sqrDistance < minDistance) {
                minDistance = sqrDistance;
                closestStructure = structures[key];
            }
        }
        return closestStructure;
    }

    /* Trees */
    public bool ContainsTree(Vector2Int cell) {
        return trees.ContainsKey(cell);
    }

    public void AddTree(GameObject tree, Vector2Int cell) {
        if (!trees.ContainsKey(cell)) {
            trees[cell] = tree;
        } else {
            Debug.LogError("Tried to spawn a tree where there already is one.");
        }
    }

    public void RemoveTree(Vector2Int cell) {
        if (trees.ContainsKey(cell)) {
            GameObject.Destroy(trees[cell]);
            trees.Remove(cell);
        } else {
            Debug.LogError("Tried to remove a tree where there wasn't one.");
        }
    }

    // TODO: This could use improvement. Think of a good heuristic.
    public Vector2Int GetNextTreePosition(Vector2Int treeCell, Vector2Int origin) {
        Vector2Int ul = new Vector2Int(treeCell.x - 1, treeCell.y - 1);
        Vector2Int u = new Vector2Int(treeCell.x, treeCell.y - 1);
        Vector2Int ur = new Vector2Int(treeCell.x + 1, treeCell.y - 1);
        Vector2Int l = new Vector2Int(treeCell.x - 1, treeCell.y);
        Vector2Int r = new Vector2Int(treeCell.x + 1, treeCell.y);
        Vector2Int dl = new Vector2Int(treeCell.x - 1, treeCell.y + 1);
        Vector2Int d = new Vector2Int(treeCell.x, treeCell.y + 1);
        Vector2Int dr = new Vector2Int(treeCell.x + 1, treeCell.y + 1);

        List<Vector2Int> candidateNodes = new List<Vector2Int>(new[] { ul, u, ur, l, r, dl, d, dr });

        int maxInteractionNodes = 0;
        Vector2Int nextTreeCell = new Vector2Int(int.MinValue, int.MinValue);

        foreach (Vector2Int treeNode in candidateNodes) {
            if (trees.ContainsKey(treeNode) && GetInteractionNodes(treeNode, origin).Length > 0) {
                Vector2Int[] interactionNodes = GetInteractionNodes(treeNode, origin);
                if (interactionNodes.Length > maxInteractionNodes) {
                    maxInteractionNodes = interactionNodes.Length;
                    nextTreeCell = treeNode;
                }
            }
        }

        return nextTreeCell;
    }

    private static Vector2Int GetClosestNode(Vector2Int[] nodes, Vector2Int origin) {
        float minDistance = float.MaxValue;
        Vector2Int bestNode = new Vector2Int(int.MaxValue, int.MaxValue);

        foreach (Vector2Int node in nodes) {
            float distance = Vector2Int.Distance(node, origin);
            if (distance < minDistance) {
                minDistance = distance;
                bestNode = node;
            }
        }

        return bestNode;
    }

    public Vector2Int GetInteractionNode(Vector2Int cell, Vector2Int origin) {
        Vector2Int[] nodes = GetInteractionNodes(cell, origin);
        return GetClosestNode(nodes, origin);
    }

    public Vector2Int GetInteractionNode(Structure structure, Vector2Int origin) {
        Vector2Int[] nodes = GetInteractionNodes(structure, origin);
        return GetClosestNode(nodes, origin);
    }

    // TODO: Make this more efficient. We are currently considering nodes that we know are not traversable.
    // We are also considering duplicate nodes.
    public Vector2Int[] GetInteractionNodes(Structure structure, Vector2Int origin) {
        List<Vector2Int> nodes = new List<Vector2Int>();
        HashSet<Vector2Int> structurePositions = GetStructurePositions(structure);
        foreach (Vector2Int structurePosition in structurePositions) {
            nodes.AddRange(GetInteractionNodes(structurePosition, origin));
        }
        return nodes.ToArray();
    }

    public Vector2Int[] GetInteractionNodes(Vector2Int cell, Vector2Int origin) {
        Vector2Int u = new Vector2Int(cell.x, cell.y - 1);
        Vector2Int d = new Vector2Int(cell.x, cell.y + 1);
        Vector2Int l = new Vector2Int(cell.x - 1, cell.y);
        Vector2Int r = new Vector2Int(cell.x + 1, cell.y);

        List<Vector2Int> candidateNodes = new List<Vector2Int>(new [] { u, d, l, r });
        List<Vector2Int> nodes = new List<Vector2Int>();

        if (trees.ContainsKey(cell)) {
            foreach (Vector2Int node in candidateNodes) {
                if (node.Equals(origin) || !IsOccupied(node)) {
                    nodes.Add(node);
                }
            }
        } else if (structurePositionsReverse.ContainsKey(cell)) {
            foreach (Vector2Int node in candidateNodes) {
                if (node.Equals(origin) || !IsOccupied(node)) {
                    nodes.Add(node);
                }
            }
        }

        return nodes.ToArray();
    }

    public bool IsOccupied(Vector2Int cell) {
        return ContainsStructure(cell) || ContainsUnit(cell) || !Traversable(cell);
    }
}
