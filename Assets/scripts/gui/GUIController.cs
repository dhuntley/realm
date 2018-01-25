using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

// Orchestration class for deciding which GUI options should be visible based on current game conditions
public class GUIController : Singleton<GUIController> {
    
    public GameObject workerBuildPanel;

    public GameObject unitPanel;

    public GameObject buildStructureButtonPrefab;
    
    protected GUIController() { }

    private void Awake() {
        unitPanel.SetActive(false);
        workerBuildPanel.SetActive(false);
    }

    public void RefreshForUnitSelection() {
        Unit selectedUnit = InputController.Instance.selectedUnit;
        if (selectedUnit == null || InputController.Instance.selectedUnits.Length > 1) {
            unitPanel.SetActive(false);
            for (int i=0; i<workerBuildPanel.transform.childCount; i++) {
                Destroy(workerBuildPanel.transform.GetChild(i).gameObject);
            }
            workerBuildPanel.SetActive(false);
            return;
        }
        
        MethodInfo[] methodInfos = selectedUnit.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        foreach (MethodInfo methodInfo in methodInfos) {
            GUIPanelAttribute[] guiPanelAttributes = methodInfo.GetCustomAttributes(typeof(GUIPanelAttribute), false) as GUIPanelAttribute[];
            
            if (guiPanelAttributes.Length > 0) {
                GameObject button = GameObject.Instantiate(buildStructureButtonPrefab, workerBuildPanel.transform);
                Button buttonComponent = button.GetComponent<Button>();

                buttonComponent.GetComponentInChildren<Text>().text = guiPanelAttributes[0].buttonText;

                buttonComponent.onClick.RemoveAllListeners();
                buttonComponent.onClick.AddListener(() => {
                    selectedUnit.Invoke(methodInfo.Name, 0.0f);
                });
                
                unitPanel.SetActive(true);
            }
        }
    }

    public void ToggleWorkerBuildPanel() {
        workerBuildPanel.SetActive(!workerBuildPanel.activeSelf);
    }
}
