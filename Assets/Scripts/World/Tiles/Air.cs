using UnityEngine;
using System.Collections;
using System;

public class Air : Tile {

	private int spriteIndex = 48;

	public override bool isWalkable() {
		return true;
	}

	public override int getSpriteIndex()
	{
		return spriteIndex;
	}

}
