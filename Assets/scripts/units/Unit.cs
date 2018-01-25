using UnityEngine;
using UnityEngine.Events;

public abstract class Unit : MonoBehaviour {

    private NavAgent navAgent;

    // The immediate next position that the Unit has begun a move into
    private Vector3 nextPosition;
    private bool hasNextPosition = false;
    private bool unitIsBlocked = false;

    private GameObject selectionCircle;

    private bool isMoving = false;

    public bool isBusy = false;

    //private bool isHovered = false;

    private UnityAction onArriveAction;

    public delegate Vector2Int OnBlockedAction();

    private OnBlockedAction onBlockedAction;

    public float moveSpeed = 2.0f;

    private ProgressBar progressBar;

    private GameObject progressBarGameObject;

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

        progressBar = GetComponentInChildren<ProgressBar>();
        if (progressBar == null) {
            Debug.LogError("No ProgressBar gameobject present on Unit.");
        }
        progressBarGameObject = progressBar.gameObject;
        progressBarGameObject.SetActive(false);

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
                if (!MapController.Instance.UpdateUnitPosition(this, nextPos2Int) && !unitIsBlocked) {
                    unitIsBlocked = true;
                    // If the agent is blocked and the destination cell is not occupied,
                    // calculate a new path.
                    if (!MapController.Instance.mapModel.IsOccupied(navAgent.destination)) {
                        navAgent.RecalculatePath();
                        hasNextPosition = false;
                    } else if (onBlockedAction != null) {
                        hasNextPosition = false;
                        navAgent.SetDestination(onBlockedAction.Invoke());
                    }
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

    private void OnDestroy() {
        StopAllRequests();
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

    public void HandleMoveRequest(Vector2Int targetCell, UnityAction onArrive = null, OnBlockedAction onBlocked = null) {
        StopAllRequests();
        if (navAgent.SetDestination(targetCell)) {
            onArriveAction = onArrive;
            onBlockedAction = onBlocked;
            isMoving = true;
            unitIsBlocked = false;
            hasNextPosition = false;
        } else {
            navAgent.ClearPath();
        }
    }

    public virtual void HandleInteractionRequest(Vector2Int worldPoint) { }

    public void HandleInteractionRequest(Vector3 worldPoint) {
        HandleInteractionRequest(MapController.Instance.WorldToCell(worldPoint));
    }

    public virtual void HandleInteractionRequest(Structure structure) { }

    public void SetProgress(float progress) {
        progressBar.progress = progress;
    }

    public void SetProgressTotal(float total) {
        progressBar.total = total;
    }

    public void SetProgressBarEnabled(bool enabled) {
        progressBarGameObject.SetActive(enabled);
    }

    protected virtual void StopAllRequests() {
        progressBarGameObject.SetActive(false);
        StopAllCoroutines();
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
