using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ramp :Tile {

	public Ramp(int index) {
		spriteIndex = 96;
	}

	public override bool isWalkable() {
		return true;
	}

	public override int getSpriteIndex()
	{
		return spriteIndex;
	}
		
}
