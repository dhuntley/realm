using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructurePlacer : MonoBehaviour {

    [HideInInspector]
    public StructureModel structureModel;

    public GameObject structure;

    public GameObject structureGhost;

    private GameObject activeStructureGhost;

    public GameObject placementHint;

    public Color goodPlacementColor;

    public Color badPlacementColor;

    private List<GameObject> activePlacementHints = new List<GameObject>();

    private MapController mapController;

    private bool _positionIsValid = true;

    public bool positionIsVald {
        get {
            return _positionIsValid;
        }
    }
    
    private void Awake() {
        enabled = false;
    }

    private void OnEnable() {
        activeStructureGhost = Instantiate(structureGhost, this.transform);
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
        
        if (Input.GetMouseButtonUp(0) && _positionIsValid) {
            GameObject.Instantiate(structure, activeStructureGhost.transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("Structures").transform);
            enabled = false;
        }
    }

    private void UpdatePlacementHints() {
        if (mapController == null) {
            mapController = FindObjectOfType<MapController>();
        }
        HashSet<Vector2Int> structPositions = mapController.WorldToStructurePositions(Camera.main.ScreenToWorldPoint(Input.mousePosition), structureModel.width, structureModel.length);

        int i = 0;
        _positionIsValid = true;

        foreach (Vector2Int structPosition in structPositions) {
            activePlacementHints[i].transform.position = mapController.GetCellCenterWorld(structPosition);
           
            if (mapController.mapModel.IsOccupied(structPosition)) {
                _positionIsValid = false;
                activePlacementHints[i].GetComponent<SpriteRenderer>().color = badPlacementColor;
            } else {
                activePlacementHints[i].GetComponent<SpriteRenderer>().color = goodPlacementColor;
            }
            i++;
        }

        activeStructureGhost.transform.position = mapController.GetStructurePositionWorld(structPositions);
    }
}

