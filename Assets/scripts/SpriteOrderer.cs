using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteOrderer : MonoBehaviour {

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        spriteRenderer.sortingOrder = Mathf.FloorToInt(-4 * transform.position.y);
    }
}
