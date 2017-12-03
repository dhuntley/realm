using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    private NavAgent navAgent;

    // The immediate next position that the Unit has begun a move into
    private Vector3 nextPosition;
    private bool hasNextPosition = false;
    private bool unitIsBlocked = false;

    protected MapController mapController;

    protected InputController inputController;

    private GameObject selectionCircle;

    private bool isMoving = false;

    private bool isHovered = false;

    public float moveSpeed = 2.0f;

	// Use this for initialization
	void Start () {
        navAgent = GetComponent<NavAgent>();
       
        if (navAgent == null) {
            Debug.LogError("Could not find navAgent component on Unit game object.");
        }

        GameObject mapControllerGameObject = GameObject.FindWithTag("MapController");

        if (mapControllerGameObject == null) {
            Debug.LogError("Could not find mapController by tag to initialize Unit.");
        }

        mapController = mapControllerGameObject.GetComponent<MapController>();
        if (mapController == null) {
            Debug.LogError("Could not find mapController by tag to initialize Unit.");
        }

        inputController = GameObject.FindObjectOfType<InputController>();
        if (inputController == null) {
            Debug.LogError("Could not find InputController to initialize Unit.");
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
    void Update () {
        if (isMoving) {
            if (unitIsBlocked && hasNextPosition) {
                Vector2Int nextPos2Int = mapController.WorldToCell(nextPosition);
                if (mapController.UpdateUnitPosition(this, nextPos2Int)) {
                    unitIsBlocked = false;
                }
            }

            if (!hasNextPosition && navAgent.HasNextCell()) {
                Vector2Int nextPos2Int = navAgent.PopNextCell();
                nextPosition = mapController.GetCellCenterWorld(nextPos2Int);

                hasNextPosition = true;
                // Immediately update the map position to the neighbouring square
                if (!mapController.UpdateUnitPosition(this, nextPos2Int)) {
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
                }
            }
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Min(transform.position.x, transform.position.y));

        if (inputController.isSelecting && inputController.IsInSelectRect(this)) {
            // TODO: Visual indicator of tentative selection?
        }
	}

    private void OnDrawGizmosSelected() {
        if (mapController) {
            Gizmos.DrawSphere(mapController.GetUnitPositionWorld(this), 0.1f);
        }
    }

    public void SetSelected(bool isSelected) {
        selectionCircle.SetActive(isSelected);
    }

    private void OnMouseEnter() {
        isHovered = true;
    }

    private void OnMouseDown() {
        bool clearCurrentSelection = !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift);
        if (inputController.SelectUnit(this, clearCurrentSelection)) {
            selectionCircle.SetActive(true);
        }
    }

    private void OnMouseExit() {
        isHovered = false;
    }

    public void HandleMoveRequest(Vector3 worldPoint) {
        navAgent.SetDestination(mapController.WorldToCell(worldPoint));
        isMoving = true;
        unitIsBlocked = false;
        hasNextPosition = false;
    }
    
    private void Register() {
        mapController.AddUnit(this);
    }

    private void Deregister() {
        mapController.RemoveUnit(this);
    }
}
