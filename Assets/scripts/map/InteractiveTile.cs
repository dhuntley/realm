using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "realm/InteractiveTile")]
public class InteractiveTile : Tile {

    private void Awake() {
        Debug.Log("Woo!");
    }

    // Update is called once per frame
    public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
        base.RefreshTile(position, tilemap);
        //Debug.Log("Woo!");
    }
}
