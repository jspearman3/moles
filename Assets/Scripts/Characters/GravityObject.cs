using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : GameEntity {
	protected const float FALL_SPEED = 4;
	public bool gravityEnabled = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	override
	protected void GameUpdate() { 
		base.GameUpdate ();
		if (gravityEnabled)
			applyGravity ();
	}

	private void applyGravity() {
		GamePosition pos = null;
		if (isLocalPlayer) {
			pos = gamePos;
			applyGravity (pos);
		} else {
			pos = GamePosition.ParseStruct (syncPos);
			applyGravity (pos);
			syncPos = pos.toStruct ();
		}
	}

	private void applyGravity(GamePosition pos) {
		
		Tile currentTile = map.getTileFromGamePosition (pos);
		float floorDepth = Mathf.Ceil(pos.depth);

		if (currentTile.GetType().Equals(typeof(Air))) {
			fall(pos);
		} else {
			if (pos.toMapCoords().depth - pos.depth > 0.0) {
				fall(pos);
			}

			if (pos.depth > floorDepth) {
				pos.depth = floorDepth;
			}
		}
	}

	private void fall(GamePosition pos) {
		pos.depth += FALL_SPEED * Time.deltaTime;
	}
}
