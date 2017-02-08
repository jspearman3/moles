using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCoords {

	public int x;
	public int y;
	public int depth;

	public MapCoords(int x, int y, int depth) {
		this.x = x;
		this.y = y;
		this.depth = depth;
	}

	override
	public string ToString() {
		return "[" + x + ", " + y + ", " + depth + "]";
	}
}
