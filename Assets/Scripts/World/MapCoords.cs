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

	public MapCoords add(int x=0, int y=0, int depth=0) {
		return new MapCoords (this.x + x, this.y + y, this.depth + depth);
	}

	override
	public string ToString() {
		return "[" + x + ", " + y + ", " + depth + "]";
	}
}
