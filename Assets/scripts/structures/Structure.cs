using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Use this for initialization
    void Start() {
        GetComponent<SpriteRenderer>().sprite = structureModel.sprite;
        Register();
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
