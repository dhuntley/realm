using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "realm/ResourceModel")]
public class ResourceModel : ScriptableObject {
    public string key;

    public string displayName;

    public Color color;

    // Add fields for Sprites, resource-specific behaviours etc.
    // (e.g., negative values allowed? Pool resource (like food, metal) or spendable resource (like gold, wood)
}
