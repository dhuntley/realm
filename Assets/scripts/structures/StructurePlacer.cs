using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePlacer : Singleton<StructurePlacer> {

    [HideInInspector]
    public StructureModel structureModel;

    public GameObject structurePrefab {
        get {
            return structureModel.prefab;
        }
    }

    public GameObject structureGhostPrefab;

    private GameObject activeStructureGhost;

    public GameObject placementHint;

    public Color goodPlacementColor;

    public Color badPlacementColor;

    private List<GameObject> activePlacementHints = new List<GameObject>();

    private bool _positionIsValid = true;

    public bool positionIsValid {
        get {
            return _positionIsValid;
        }
    }

    protected StructurePlacer() { }

    private void Awake() {
        enabled = false;
    }

    private void OnEnable() {
        activeStructureGhost = Instantiate(structureGhostPrefab, this.transform);
        activeStructureGhost.GetComponent<SpriteRenderer>().sprite = structureModel.sprite;

        for (int i = 0; i < structureModel.area; i++) {
            activePlacementHints.Add(Instantiate(placementHint));
        }
        UpdatePlacementHints();
    }

    private void OnDisable() {
        Destroy(activeStructureGhost);
        activeStructureGhost = null;

        foreach (GameObject hint in activePlacementHints) {
            Destroy(hint);
        }
        activePlacementHints.Clear();
    }

    private void Update() {
        UpdatePlacementHints();
    }

    private void UpdatePlacementHints() {
        Vector2Int[] structPositions = MapController.Instance.WorldToStructurePositions(Camera.main.ScreenToWorldPoint(Input.mousePosition), structureModel.width, structureModel.length);
        Unit selectedUnit = InputController.Instance.selectedUnit;
        Vector2Int selectedUnitPosition = selectedUnit != null ? MapController.Instance.GetUnitPosition(selectedUnit) : new Vector2Int(int.MaxValue, int.MaxValue);
        bool hasResources = HasSufficientResources();

        int i = 0;
        _positionIsValid = true;

        foreach (Vector2Int structPosition in structPositions) {
            activePlacementHints[i].transform.position = MapController.Instance.GetCellCenterWorld(structPosition);

            if (!hasResources || (MapController.Instance.mapModel.IsOccupied(structPosition) && structPosition != selectedUnitPosition)) {
                _positionIsValid = false;
                activePlacementHints[i].GetComponent<SpriteRenderer>().color = badPlacementColor;
            } else {
                activePlacementHints[i].GetComponent<SpriteRenderer>().color = goodPlacementColor;
            }
            i++;
        }

        activeStructureGhost.transform.position = MapController.Instance.GetStructurePositionWorld(structPositions);
    }

    public void HandleConfirmPlacement() {
        if (_positionIsValid) {
            Vector3 position = activeStructureGhost.transform.position;
            GameObject prefab = structurePrefab;

            if (HasSufficientResources()) {
                Unit builder = InputController.Instance.selectedUnit;
                if (builder) {
                    Vector2Int targetCell = MapController.Instance.WorldToCell(position);
                    builder.HandleMoveRequest(targetCell, () => {
                        Structure structure = InstantiateStructure(prefab, position);
                        if (structure) {
                            structure.builder = builder;
                        }
                        builder.isBusy = true;
                        InputController.Instance.DeselectUnit(builder);
                    });
                } else {
                    InstantiateStructure(prefab, position);
                }
                enabled = false;
            }
        }
    }

    public bool HasSufficientResources() {
        return ResourceManager.Instance.HasResources(structureModel);
    }

    private Structure InstantiateStructure(GameObject prefab, Vector3 position) {
        // TODO: Re-validate structure location before placing structure, in case something has moved into the way
        if (ResourceManager.Instance.SpendResources(structureModel)) {
            GameObject structureGameObject = GameObject.Instantiate(prefab, position, Quaternion.identity, GameObject.FindGameObjectWithTag("Structures").transform);
            return structureGameObject.GetComponent<Structure>();
        }
        return null;
    }
}

