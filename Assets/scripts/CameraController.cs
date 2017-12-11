using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    
    private bool isPanning = false;

    private Vector3 panStartPosition;

	// Update is called once per frame
	void Update () {

        Vector3 position = transform.position;

        if (Input.GetMouseButtonDown(2)) {
            isPanning = true;
            panStartPosition = Input.mousePosition;
        } else if (Input.GetMouseButtonUp(2)) {
            isPanning = false;
        } else if (isPanning) {
            if (Input.mousePosition.x < panStartPosition.x - 20) {
                position.x = position.x - 0.2f;
            } else if (Input.mousePosition.x > panStartPosition.x + 20) {
                position.x = position.x + 0.2f;
            }

            if (Input.mousePosition.y < panStartPosition.y - 20) {
                position.y = position.y - 0.2f;
            } else if (Input.mousePosition.y > panStartPosition.y + 20) {
                position.y = position.y + 0.2f;
            }
        } else {
            if (Input.mousePosition.x < 20) {
                position.x = position.x - 0.2f;
            } else if (Screen.width - Input.mousePosition.x < 20) {
                position.x = position.x + 0.2f;
            }

            if (Input.mousePosition.y < 20) {
                position.y = position.y - 0.2f;
            } else if (Screen.height - Input.mousePosition.y < 20) {
                position.y = position.y + 0.2f;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            if (InputController.Instance.selectedUnit) {
                position = InputController.Instance.selectedUnit.transform.position;
            }
        }

        position.z = -10;

        transform.position = position;
    }
}
