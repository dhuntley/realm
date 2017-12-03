using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour {

    protected MapController mapController;

    public int width = 2;

    public int length = 2;

    public int area {
        get {
            return width * length;
        }
    }

    // Use this for initialization
    void Start () {
        GameObject mapControllerGameObject = GameObject.FindWithTag("MapController");

        if (mapControllerGameObject == null) {
            Debug.LogError("Could not find mapController by tag to initialize Unit.");
        }

        mapController = mapControllerGameObject.GetComponent<MapController>();
        if (mapController == null) {
            Debug.LogError("Could not find mapController by tag to initialize Unit.");
        }

        Register();
    }

    protected virtual void OnDisable() {
        // Deregister position with NavController
        Deregister();
    }

    // Update is called once per frame
    void Update () {
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Min(transform.position.x, transform.position.y));
    }

    private void OnDrawGizmosSelected() {
        if (mapController) {
            Gizmos.DrawSphere(mapController.GetStructurePositionWorld(this), 0.1f);

            List<Vector3> worldPositions = mapController.GetStructurePositionsWorld(this);

            foreach (Vector3 position in worldPositions) {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(position, new Vector3(0.1f, 0.1f, 0.1f));
            }
        }
    }

    private void Register() {
        mapController.AddStructure(this);
    }

    private void Deregister() {
        mapController.RemoveStructure(this);
    }
}
