using UnityEngine;
using System.Collections;
using System;

public class Ladder : Tile {

	private int spriteIndex = 95;

	public override bool isWalkable() {
		return true;
	}

	public override int getSpriteIndex()
	{
		return spriteIndex;
	}

}
