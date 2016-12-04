using UnityEngine;
using System.Collections;

public class Darkness : Tile {

	private int spriteIndex = 10;

	public override bool isWalkable() {
		return false;
	}

    public override int getSpriteIndex()
    {
        return spriteIndex;
    }
}
