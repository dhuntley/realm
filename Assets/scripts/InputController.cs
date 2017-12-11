using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Singleton<InputController> {

    public Color selectBorderColor;

    public Color selectFillColor;

    private List<Unit> _selectedUnits = new List<Unit>();

    public List<Unit> selectedUnits {
        get {
            return _selectedUnits;
        }
    }

    public Unit selectedUnit {
        get {
            return _selectedUnits.Count > 0 ? _selectedUnits[0] : null;
        }
    }

    private bool _isSelecting = false;

    public bool isSelecting {
        get { return _isSelecting; }
    }

    private Vector3 selectStart;

    private Vector3 selectStartWorld;

    public bool isBuilding {
        get {
            return StructurePlacer.Instance.enabled;
        }
    }

    protected InputController() { }

    // Use this for initialization
    void Start() {
        StructurePlacer.Instance.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (!isBuilding) {
            if (Input.GetMouseButtonDown(1)) {
                foreach (Unit playerUnit in _selectedUnits) {
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
                    _selectedUnits = unitsToSelect;
                    foreach (Unit unit in _selectedUnits) {
                        unit.SetSelected(true);
                    }
                    RefreshGUIForSelection();
                }
            }
        } else {
            // StructurePlacer enabled / isBuilding == true
            _isSelecting = false;
            if (Input.GetMouseButtonUp(1)) {
                StructurePlacer.Instance.enabled = false;
            } else if (Input.GetMouseButtonUp(0)) {
                StructurePlacer.Instance.HandleConfirmPlacement();
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

        _selectedUnits.Add(unit);
        RefreshGUIForSelection();
        return true;
    }

    public bool IsInSelectRect(Unit unit) {
        return false;
    }

    private void DeselectAllUnits() {
        foreach (Unit selectedUnit in _selectedUnits) {
            selectedUnit.SetSelected(false);
        }
        _selectedUnits.Clear();
        RefreshGUIForSelection();
    }
    
    public void RefreshGUIForSelection() {
        GUIController.Instance.RefreshForUnitSelection();
    }
}
