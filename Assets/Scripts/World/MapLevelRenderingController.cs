using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevelRenderingController : MonoBehaviour {

	const float DEPTH_VISION = 5.0f; 

	GameObject player = MoleController.localPlayer;
	SpriteRenderer spriteRenderer;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null) {
			player = MoleController.localPlayer;
			return;
		}


		int order = player.GetComponent<SpriteRenderer> ().sortingOrder;

		if (spriteRenderer.sortingOrder > order) {
			spriteRenderer.color = new Color (1, 1, 1, 0);
		} else {
			spriteRenderer.color = new Color (1, 1, 1, 1);
		}
		
	}
}
