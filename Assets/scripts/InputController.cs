using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    public Color selectBorderColor;

    public Color selectFillColor;

    private List<Unit> selectedUnits = new List<Unit>();

    private bool _isSelecting = false;

    public bool isSelecting {
        get { return _isSelecting; }
    }

    private Vector3 selectStart;

    private Vector3 selectStartWorld;

    private bool _isBuilding = false;

    public bool isBuilding {
        get {
            return GetComponent<StructurePlacer>().enabled;
        }
    }

    // Use this for initialization
    void Start() {
        GetComponent<StructurePlacer>().enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            foreach (Unit playerUnit in selectedUnits) {
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                playerUnit.HandleMoveRequest(worldPoint);
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            _isSelecting = true;
            selectStart = Input.mousePosition;
            selectStartWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0) && _isSelecting) {
            _isSelecting = false;

            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float width = Mathf.Abs(mouseWorld.x - selectStartWorld.x);
            float height = Mathf.Abs(mouseWorld.y - selectStartWorld.y);
            float x = Mathf.Min(mouseWorld.x, selectStartWorld.x);
            float y = Mathf.Min(mouseWorld.y, selectStartWorld.y);

            Rect selectRect = new Rect(x, y, width, height);
            Unit[] units = FindObjectsOfType<Unit>();
            List<Unit> unitsToSelect = new List<Unit>();

            foreach (Unit unit in units) {
                if (selectRect.Contains(unit.transform.position)) {
                    unitsToSelect.Add(unit);
                }
            }

            if (unitsToSelect.Count > 0) {
                DeselectAllUnits();
                selectedUnits = unitsToSelect;
                foreach (Unit unit in selectedUnits) {
                    unit.SetSelected(true);
                }
            }
        }
    }

    private void OnGUI() {
        if (_isSelecting) {
            Rect selectRect = GuiUtils.GetScreenRect(selectStart, Input.mousePosition);
            GuiUtils.DrawScreenRect(selectRect, selectFillColor);
            GuiUtils.DrawScreenRectBorder(selectRect, 1f, selectBorderColor);
        }
    }

    public bool SelectUnit(Unit unit, bool clearExistingSelection) {
        if (clearExistingSelection) {
            DeselectAllUnits();
        }

        selectedUnits.Add(unit);
        return true;
    }

    public bool IsInSelectRect(Unit unit) {
        return false;
    }

    private void DeselectAllUnits() {
        foreach (Unit selectedUnit in selectedUnits) {
            selectedUnit.SetSelected(false);
        }
        selectedUnits.Clear();
    }

    public void HandleClickBuild(StructureModel structureModel) {
        StructurePlacer structurePlacer = GetComponent<StructurePlacer>();
        structurePlacer.enabled = !structurePlacer.enabled;
        if (isBuilding) {
            //Cursor.visible = false;
            structurePlacer.structureModel = structureModel;
        } 
    }
}
