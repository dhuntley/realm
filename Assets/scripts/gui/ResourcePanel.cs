using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourcePanel : MonoBehaviour {

    public ResourceModel resourceModel;

    private Text amountText;

    private Text resourceText;

    void Awake() {
        ResourceManager.Instance.RegisterResourcePanel(resourceModel.key, this);
    }

    void Start() {
        Text[] textComponents = GetComponentsInChildren<Text>();
        resourceText = textComponents[0];
        amountText = textComponents[1];

        amountText.color = resourceModel.color;
        resourceText.text = resourceModel.displayName;
    }

    private void OnDestroy() {
        if (ResourceManager.HasInstance) {
            ResourceManager.Instance.DeregisterResourcePanel(resourceModel.key);
        }
    }

    public void UpdateResourceValue(int value) {
        amountText.text = "" + value;
    }
}
