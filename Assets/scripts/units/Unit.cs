using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unit : MonoBehaviour {

    private NavAgent navAgent;

    // The immediate next position that the Unit has begun a move into
    private Vector3 nextPosition;
    private bool hasNextPosition = false;
    private bool unitIsBlocked = false;

    private GameObject selectionCircle;

    private bool isMoving = false;

    //private bool isHovered = false;

    private UnityAction onArriveAction;

    public float moveSpeed = 2.0f;

    public Vector2Int cell {
        get {
            return MapController.Instance.GetUnitPosition(this);
        }
    }

    // Use this for initialization
    void Start() {
        navAgent = GetComponent<NavAgent>();

        if (navAgent == null) {
            Debug.LogError("Could not find navAgent component on Unit game object.");
        }

        selectionCircle = transform.Find("SelectionCircle").gameObject;
        if (selectionCircle == null) {
            Debug.LogError("No SelectionCircle gameobject present on Unit.");
        }

        Register();
    }

    protected virtual void OnDisable() {
        // Deregister position with NavController
        Deregister();
    }

    // Update is called once per frame
    void Update() {
        if (isMoving) {

            // TODO: If a unit is blocked, but is "near enough" its destination, allow it to stop there
            // (i.e., for formation movement). Need to somehow consider whether a unit moved as part of the
            // same order has reached its destination already.
            if (unitIsBlocked && hasNextPosition) {
                Vector2Int nextPos2Int = MapController.Instance.WorldToCell(nextPosition);
                if (MapController.Instance.UpdateUnitPosition(this, nextPos2Int)) {
                    unitIsBlocked = false;
                }
            }

            if (!hasNextPosition && navAgent.HasNextCell()) {
                Vector2Int nextPos2Int = navAgent.PopNextCell();
                nextPosition = MapController.Instance.GetCellCenterWorld(nextPos2Int);

                hasNextPosition = true;
                // Immediately update the map position to the neighbouring square
                if (!MapController.Instance.UpdateUnitPosition(this, nextPos2Int)) {
                    unitIsBlocked = true;
                }
            } else if (!hasNextPosition && !navAgent.HasNextCell()) {
                isMoving = false;
                unitIsBlocked = false;
            }

            if (hasNextPosition && !unitIsBlocked) {
                transform.position = Vector2.MoveTowards(transform.position, nextPosition, Time.deltaTime * moveSpeed);

                if (Vector2.Distance(transform.position, nextPosition) == 0f) {
                    hasNextPosition = false;
                    if (!navAgent.HasNextCell() && onArriveAction != null) {
                        // When unit arrives at destination, invoke onArriveAction
                        onArriveAction.Invoke();
                    }
                }
            }
        }

        if (InputController.Instance.isSelecting && InputController.Instance.IsInSelectRect(this)) {
            // TODO: Visual indicator of tentative selection?
        }
    }

    private void OnDrawGizmosSelected() {
        if (MapController.HasInstance) {
            Gizmos.DrawSphere(MapController.Instance.GetUnitPositionWorld(this), 0.1f);
        }
    }

    public void SetSelected(bool isSelected) {
        selectionCircle.SetActive(isSelected);
    }

    private void OnMouseEnter() {
        //isHovered = true;
    }

    private void OnMouseDown() {
        bool clearCurrentSelection = !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift);
        if (InputController.Instance.SelectUnit(this, clearCurrentSelection)) {
            selectionCircle.SetActive(true);
        }
    }

    private void OnMouseExit() {
        //isHovered = false;
    }

    public void HandleMoveRequest(Vector3 worldPoint, UnityAction onArrive = null) {
        if (navAgent.SetDestination(MapController.Instance.WorldToCell(worldPoint))) {
            onArriveAction = onArrive;
            isMoving = true;
            unitIsBlocked = false;
            hasNextPosition = false;
        } else {
            navAgent.ClearPath();
        }
    }

    private void Register() {
        MapController.Instance.AddUnit(this);
    }

    private void Deregister() {
        if (MapController.HasInstance) {
            MapController.Instance.RemoveUnit(this);
        }
    }
}
