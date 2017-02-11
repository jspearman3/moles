using UnityEngine;
using System.Collections;
using System;

public class Stair : Tile {

	private int spriteIndex = 49;

	public override bool isWalkable() {
		return false;
	}

	public override int getSpriteIndex()
	{
		return spriteIndex;
	}

}
