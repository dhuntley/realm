using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "realm/UnitModel")]
public class UnitModel : ScriptableObject {
    public float constructionTime = 4.0f;
    
    public GameObject prefab;

    private Dictionary<string, int> _resourceCostMap;

    public Dictionary<string, int> resourceCostMap {
        get {
            if (_resourceCostMap == null) {
                Debug.Assert(resourceKeys.Length == resourceCosts.Length);
                _resourceCostMap = new Dictionary<string, int>();
                for (int i = 0; i < resourceKeys.Length; i++) {
                    resourceCostMap[resourceKeys[i]] = resourceCosts[i];
                }
            }
            return _resourceCostMap;
        }
    }

    public string[] resourceKeys;

    public int[] resourceCosts;

    public int GetResourceCost(string resourceKey) {
        return resourceCostMap[resourceKey];
    }

    public bool HasResourceCost(string resourceKey) {
        return resourceCostMap.ContainsKey(resourceKey);
    }
}
