﻿using UnityEngine;
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
        bool addToSelection = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!addToSelection) {
            InputController.Instance.DeselectAll(false);
            selected = true;
        } else if (addToSelection && selected) {
            selected = false;
        } else {
            selected = true;
        }
        GUIController.Instance.RefreshForSelection();
    }
}
