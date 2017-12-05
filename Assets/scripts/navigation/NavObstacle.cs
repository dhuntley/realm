using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavObstacle : MonoBehaviour {

    /* Navigation Layer (i.e., who am I "friendly" with)
     *  Enum? Can be:
     *      - Obstacle - Static
     *      - Player - Dynamic
     *      - Enemy - Dynamic
     *  Expandable as necessary
     *  Make editable via dropdown in Editor
    */
    public enum NavigationLayer {
        Obstacle,
        Player,
        Enemy
    };

    public NavigationLayer navigationLayer = NavigationLayer.Obstacle;

    // NavController
    protected NavController navController {
        get {
            return mapController.navController;
        }
    }

    protected MapController mapController;

    protected virtual void OnEnable() {
        GameObject mapControllerGameObject = GameObject.FindWithTag("MapController");

        if (mapControllerGameObject == null) {
            Debug.LogError("Could not find mapController by tag to initialize NavObstacle.");
        }

        mapController = mapControllerGameObject.GetComponent<MapController>();
        if (mapController == null) {
            Debug.LogError("Could not find mapController by tag to initialize NavObstacle.");
        }
    }
}
