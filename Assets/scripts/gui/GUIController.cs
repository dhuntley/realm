using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Orchestration class for deciding which GUI options should be visible based on current game conditions
public class GUIController : Singleton<GUIController> {

    public GameObject workerBuildPanel;

    public GameObject unitPanel;

    public GameObject structurePanel;

    public GameObject buildButtonPrefab;

    protected GUIController() { }

    private void Awake() {
        unitPanel.SetActive(false);
        workerBuildPanel.SetActive(false);
        structurePanel.SetActive(false);
    }

    public void RefreshForSelection() {
        Unit selectedUnit = InputController.Instance.selectedUnit;
        Structure selectedStructure = InputController.Instance.selectedStructure;

        unitPanel.SetActive(false);
        for (int i = 0; i < workerBuildPanel.transform.childCount; i++) {
            Destroy(workerBuildPanel.transform.GetChild(i).gameObject);
        }
        workerBuildPanel.SetActive(false);

        for (int i = 0; i < structurePanel.transform.childCount; i++) {
            Destroy(structurePanel.transform.GetChild(i).gameObject);
        }
        structurePanel.SetActive(false);

        if (selectedUnit && InputController.Instance.selectedUnits.Length == 1) {
            InitPanelFromSelection(selectedUnit, workerBuildPanel);
            unitPanel.SetActive(true);
        }
        if (selectedStructure && InputController.Instance.selectedStructures.Length == 1) {
            // Add GUIPanelAttribute actions
            InitPanelFromSelection(selectedStructure, structurePanel);

            // Add build unit actions
            UnityAction[] produceUnitActions = selectedStructure.GetGUIProduceUnitActions();
            foreach (UnityAction action in produceUnitActions) {
                AddBuildButton(structurePanel, "Build guy", action);
            }
            
            structurePanel.SetActive(true);
        }
    }

    private void InitPanelFromSelection(MonoBehaviour selectedEntity, GameObject panel) {
        MethodInfo[] methodInfos = selectedEntity.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        foreach (MethodInfo methodInfo in methodInfos) {
            GUIPanelAttribute[] guiPanelAttributes = methodInfo.GetCustomAttributes(typeof(GUIPanelAttribute), false) as GUIPanelAttribute[];

            if (guiPanelAttributes.Length > 0) {
                AddBuildButton(panel, guiPanelAttributes[0].buttonText, () => {
                    selectedEntity.Invoke(methodInfo.Name, 0.0f);
                });
            }
        }
    }

    private void AddBuildButton(GameObject panel, string buttonText, UnityAction action) {
        GameObject button = GameObject.Instantiate(buildButtonPrefab, panel.transform);
        Button buttonComponent = button.GetComponent<Button>();

        buttonComponent.GetComponentInChildren<Text>().text = buttonText;

        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(action);
    }

    public void ToggleWorkerBuildPanel() {
        workerBuildPanel.SetActive(!workerBuildPanel.activeSelf);
    }
}
