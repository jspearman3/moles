using UnityEngine;
using System.Collections;
using System;

public class Dirt : Tile {

	private int spriteIndex = 47;

	public override bool isWalkable() {
		return true;
	}

    public override int getSpriteIndex()
    {
        return spriteIndex;
    }

}
