using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager> {

    public ResourceModel[] resources;

    public int[] startingValues;

    private Dictionary<string, int> resourceValues = new Dictionary<string, int>();

    private Dictionary<string, ResourcePanel> resourcePanels = new Dictionary<string, ResourcePanel>();

    protected ResourceManager() { }

    // Use this for initialization
    void Start() {
        Debug.Assert(resources.Length == startingValues.Length);
        for (int i = 0; i < resources.Length; i++) {
            string key = resources[i].key;
            resourceValues[key] = startingValues[i];
            if (resourcePanels.ContainsKey(key)) {
                resourcePanels[key].UpdateResourceValue(startingValues[i]);
            }
        }
    }

    public bool HasResources(string key, int value) {
        return resourceValues[key] >= value;
    }

    public bool HasResources(StructureModel structureModel) {
        foreach (string resourceKey in structureModel.resourceCostMap.Keys) {
            if (!HasResources(resourceKey, structureModel.GetResourceCost(resourceKey))) {
                return false;
            }
        }
        return true;
    }

    public bool HasResources(UnitModel unitModel) {
        foreach (string resourceKey in unitModel.resourceCostMap.Keys) {
            if (!HasResources(resourceKey, unitModel.GetResourceCost(resourceKey))) {
                return false;
            }
        }
        return true;
    }

    public bool SpendResources(string key, int value) {
        if (HasResources(key, value)) {
            resourceValues[key] = resourceValues[key] - value;
            if (resourcePanels.ContainsKey(key)) {
                resourcePanels[key].UpdateResourceValue(resourceValues[key]);
            }
            return true;
        }
        return false;
    }

    public bool SpendResources(StructureModel structureModel) {
        if (HasResources(structureModel)) {
            foreach (string resourceKey in structureModel.resourceCostMap.Keys) {
                SpendResources(resourceKey, structureModel.GetResourceCost(resourceKey));
            }
            return true;
        }
        return false;
    }

    public bool SpendResources(UnitModel unitModel) {
        if (HasResources(unitModel)) {
            foreach (string resourceKey in unitModel.resourceCostMap.Keys) {
                SpendResources(resourceKey, unitModel.GetResourceCost(resourceKey));
            }
            return true;
        }
        return false;
    }

    public void AddResources(string key, int value) {
        resourceValues[key] = resourceValues[key] + value;
        if (resourcePanels.ContainsKey(key)) {
            resourcePanels[key].UpdateResourceValue(resourceValues[key]);
        }
    }

    public void RegisterResourcePanel(string key, ResourcePanel panel) {
        resourcePanels[key] = panel;
    }

    public void DeregisterResourcePanel(string key) {
        resourcePanels.Remove(key);
    }
}
