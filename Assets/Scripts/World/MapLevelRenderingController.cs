using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevelRenderingController : MonoBehaviour {

	const float DEPTH_VISION = 5.0f;
	const float HEIGHT_VISION = 1.0f;


	GameObject player = MoleController.localPlayer;
	SpriteRenderer spriteRenderer;
	public float levelDepth;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (player == null) {
			player = MoleController.localPlayer;
			return;
		}


		float playerDepth = player.GetComponent<MoleController> ().gamePos.depth;

		if (levelDepth > playerDepth) {
			float darkness = getIntensity(levelDepth - playerDepth, DEPTH_VISION);
			spriteRenderer.color = new Color (darkness, darkness, darkness, 1);

		} else {
			float alpha = getIntensity(playerDepth - levelDepth, HEIGHT_VISION);
			spriteRenderer.color = new Color (1, 1, 1, alpha);
		}
		
	}

	private float getIntensity(float diff, float max) {

		float ratio = diff / max;

		float norm = 1 - ratio;

		if (norm > 1) {
			
			return 1;
		} else if (norm < 0) {
			return 0;
		} else {
			return norm;
		}
	}

}
