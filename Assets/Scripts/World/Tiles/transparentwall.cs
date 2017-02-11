using UnityEngine;
using System.Collections;
using System;

public class transparentwall : Tile {

	private int spriteIndex = 50;

	public override bool isWalkable() {
		return false;
	}

	public override int getSpriteIndex()
	{
		return spriteIndex;
	}

}
