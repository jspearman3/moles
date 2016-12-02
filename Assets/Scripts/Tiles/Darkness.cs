using UnityEngine;
using System.Collections;

public class Darkness : Tile {

	private Sprite sprite = Tile.sprites[0];

	public override bool isWalkable() {
		return false;
	}

	public override Sprite getSprite() {
		return sprite;
	}
}
