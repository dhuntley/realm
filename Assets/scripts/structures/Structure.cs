using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Structure : MonoBehaviour {

    public StructureModel structureModel;

    public int width {
        get {
            return structureModel.width;
        }
    }

    public int length {
        get {
            return structureModel.length;
        }
    }

    public int area {
        get {
            return width * length;
        }
    }

    [HideInInspector]
    public Unit builder;

    public bool underConstruction = true;

    public bool producingUnit = false;

    private ProgressBar progressBar;

    private GameObject progressBarGameObject;

    // Use this for initialization
    void Start() {
        GetComponent<SpriteRenderer>().sprite = structureModel.sprite;

        progressBar = GetComponentInChildren<ProgressBar>();
        if (progressBar == null) {
            Debug.LogError("No ProgressBar gameobject present on Unit.");
        }
        progressBarGameObject = progressBar.gameObject;
        progressBarGameObject.SetActive(false);

        Register();

        if (underConstruction) {
            StartCoroutine(Construct());
        }
    }

    public void SetProgress(float progress) {
        progressBar.progress = progress;
    }

    public void SetProgressTotal(float total) {
        progressBar.total = total;
    }

    public void SetProgressBarEnabled(bool enabled) {
        progressBarGameObject.SetActive(enabled);
    }

    private IEnumerator ProduceUnit(UnitModel unitModel) {
        float startTime = Time.time;
        producingUnit = true;
        SetProgress(0.0f);
        SetProgressTotal(structureModel.constructionTime);
        while (Time.time < startTime + structureModel.constructionTime) {
            SetProgress(Time.time - startTime);
            SetProgressBarEnabled(true);
            yield return null;
        }
        SetProgressBarEnabled(false);

        producingUnit = false;

        Vector2Int spawnCell = MapController.Instance.mapModel.GetInteractionNode(this, new Vector2Int(int.MinValue,int.MinValue));
        Vector3 spawnPosition = MapController.Instance.GetCellCenterWorld(spawnCell);
        GameObject unitGameObject = GameObject.Instantiate(unitModel.prefab, spawnPosition, Quaternion.identity, GameObject.FindGameObjectWithTag("Units").transform);
    }

    public UnityAction[] GetGUIProduceUnitActions() {
        LinkedList<UnityAction> produceUnitActions = new LinkedList<UnityAction>();

        foreach (UnitModel unitModel in structureModel.buildableUnits) {
            produceUnitActions.AddLast(() => {
                PurchaseUnit(unitModel);
            });
        }

        UnityAction[] actionArray = new UnityAction[produceUnitActions.Count];
        produceUnitActions.CopyTo(actionArray, 0);
        return actionArray;
    }

    public void PurchaseUnit(UnitModel unitModel) {
        if (producingUnit) {
            Debug.Log("Structure is already producing a unit.");
            return;
        }

        if (ResourceManager.Instance.SpendResources(unitModel)) {
            StartCoroutine(ProduceUnit(unitModel));
        }
        return;
    }

    public Selectable GetSelectable() {
        return GetComponent<Selectable>();
    }

    private IEnumerator Construct() {
        float startTime = Time.time;
        GetSelectable().isBusy = true;
        SetProgress(0.0f);
        SetProgressTotal(structureModel.constructionTime);
        while (Time.time < startTime + structureModel.constructionTime) {
            SetProgress(Time.time - startTime);
            SetProgressBarEnabled(true);
            yield return null;
        }
        SetProgressBarEnabled(false);

        underConstruction = false;
        GetSelectable().isBusy = false;
        if (builder) {
            builder.HandleMoveRequest(MapController.Instance.mapModel.GetInteractionNode(this, builder.cell), () => {
                Selectable builderSelectable = builder.GetComponent<Selectable>();
                if (builderSelectable) {
                    builderSelectable.isBusy = false;
                }
            });
        }
    }

    protected virtual void OnDisable() {
        Deregister();
    }

    private void OnDrawGizmosSelected() {
        if (MapController.HasInstance) {
            Gizmos.DrawSphere(MapController.Instance.GetStructurePositionWorld(this), 0.1f);

            List<Vector3> worldPositions = MapController.Instance.GetStructurePositionsWorld(this);

            foreach (Vector3 position in worldPositions) {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(position, new Vector3(0.1f, 0.1f, 0.1f));
            }
        }
    }

    private void Register() {
        MapController.Instance.AddStructure(this);
    }

    private void Deregister() {
        if (MapController.HasInstance) {
            MapController.Instance.RemoveStructure(this);
        }
    }
}
