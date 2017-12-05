using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(menuName = "realm/StructureModel")]
public class StructureModel : ScriptableObject {
    public int width = 2;

    public int length = 2;

    public Sprite sprite;

    public int area {
        get { return width * length; }
    }
}
