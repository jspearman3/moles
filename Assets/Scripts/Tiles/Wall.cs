using UnityEngine;
using System.Collections;

public class Wall : Tile {

	private Sprite sprite = Tile.sprites[3];

	public override bool isWalkable() {
		return false;
	}

	public override Sprite getSprite() {
		return sprite;
	}
}
