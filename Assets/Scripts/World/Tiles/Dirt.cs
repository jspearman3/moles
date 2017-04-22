using UnityEngine;
using System.Collections;
using System;

public class Dirt : Tile {

	public Dirt() {
		spriteIndex = 47;
	}

	public override bool isWalkable() {
		return true;
	}

    public override int getSpriteIndex()
    {
        return spriteIndex;
    }

}
