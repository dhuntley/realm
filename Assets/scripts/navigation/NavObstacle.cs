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
            return MapController.Instance.navController;
        }
    }
}
