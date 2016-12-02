using UnityEngine;
using System.Collections;

public class Dirt : Tile {

	private Sprite sprite = Tile.sprites[4];

	public override bool isWalkable() {
		return true;
	}

	public override Sprite getSprite() {
		return sprite;
	}
}
