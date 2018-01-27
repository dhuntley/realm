using UnityEngine;
using System.Collections;

public class Selectable : MonoBehaviour {

    public bool isBusy = false;

    private GameObject selectionCircle;

    public bool selected {
        get {
            return selectionCircle.activeSelf;
        }
        set {
            selectionCircle.SetActive(value);
            GUIController.Instance.RefreshForSelection();
        }
    }

    public void SetSelectedNoGUIRefresh(bool selected) {
        selectionCircle.SetActive(selected);
    }

    private void Start() {
        selectionCircle = transform.Find("SelectionCircle").gameObject;
        if (selectionCircle == null) {
            Debug.LogError("No SelectionCircle gameobject present on Unit.");
        }
    }

    private void OnMouseDown() {
        bool clearCurrentSelection = !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift);
        if (InputController.Instance.Select(this, clearCurrentSelection)) {
            selectionCircle.SetActive(true);
        }
    }
}
