using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InputController : Singleton<InputController> {

    public Color selectBorderColor;

    public Color selectFillColor;
    
    public Unit[] selectedUnits {
        get {
            return UnitManager.Instance.GetAllSelectedUnits();
        }
    }

    public Unit selectedUnit {
        get {
            Unit[] units = selectedUnits;
            return (units != null && units.Length > 0) ? units[0] : null;
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
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!isBuilding) {
            if (Input.GetMouseButtonDown(1)) {
                // See if the user has clicked on an interactable object for the selected units
                Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorld);
                if (colliders != null && colliders.Length > 0) {
                    Collider2D frontCollider = null;
                    int sortOrder = int.MinValue;
                    foreach (Collider2D collider in colliders) {
                        SpriteRenderer renderer = collider.GetComponent<SpriteRenderer>();
                        if (renderer && renderer.sortingOrder > sortOrder) {
                            sortOrder = renderer.sortingOrder;
                            frontCollider = collider;
                        }
                    }
                    foreach (Unit playerUnit in selectedUnits) {
                        playerUnit.HandleInteractionRequest(frontCollider.transform.position);
                    }
                } else {
                    // Move selected units to the target tile
                    foreach (Unit playerUnit in selectedUnits) {
                        playerUnit.HandleMoveRequest(MapController.Instance.WorldToCell(mouseWorld));
                    }
                }
            }

            if (Input.GetMouseButtonDown(0)) {
                _isSelecting = true;
                selectStart = Input.mousePosition;
                selectStartWorld = mouseWorld;
            }

            if (Input.GetMouseButtonUp(0) && _isSelecting) {
                _isSelecting = false;
                
                float width = Mathf.Abs(mouseWorld.x - selectStartWorld.x);
                float height = Mathf.Abs(mouseWorld.y - selectStartWorld.y);
                float x = Mathf.Min(mouseWorld.x, selectStartWorld.x);
                float y = Mathf.Min(mouseWorld.y, selectStartWorld.y);

                Rect selectRect = new Rect(x, y, width, height);
                Unit[] units = FindObjectsOfType<Unit>();
                List<Unit> unitsToSelect = new List<Unit>();

                foreach (Unit unit in units) {
                    if (selectRect.Contains(unit.transform.position) && !unit.isBusy) {
                        unitsToSelect.Add(unit);
                    }
                }

                if (unitsToSelect.Count > 0) {
                    DeselectAllUnits();
                    foreach (Unit unit in unitsToSelect) {
                        unit.SetSelectedNoGUIRefresh(true);
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
        if (unit.isBusy) {
            return false;
        }

        if (clearExistingSelection) {
            DeselectAllUnits();
        }

        unit.selected = true;
        return true;
    }

    public bool IsInSelectRect(Unit unit) {
        return false;
    }

    private void DeselectAllUnits() {
        foreach (Unit selectedUnit in selectedUnits) {
            selectedUnit.SetSelectedNoGUIRefresh(false);
        }
        RefreshGUIForSelection();
    }

    private void RefreshGUIForSelection() {
        GUIController.Instance.RefreshForUnitSelection();
    }
}
