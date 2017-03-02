using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp :Tile {
	protected int spriteIndex = 96;

	public Ramp(int index) {
		spriteIndex = index;
	}

	public override bool isWalkable() {
		return true;
	}

	public override int getSpriteIndex()
	{
		return spriteIndex;
	}
		
}
