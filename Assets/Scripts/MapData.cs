using UnityEngine;
using System.Collections;

public class MapData {

	public Tile[,] tiles;

	//default 9x9 map
	public MapData() {
		int length = 9;
		tiles = new Tile[length,length];

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {

				if (i == 3) {
					tiles [i, j] = new Dirt ();
				} else {
					
					tiles [i, j] = new Darkness ();
				}
			}
		}

		tiles [3,3] = new Dirt ();
		tiles [3,4] = new Dirt ();
		tiles [3,5] = new Dirt ();
		tiles [4,3] = new Dirt ();
		tiles [4,4] = new Dirt ();
		tiles [4,5] = new Dirt ();
		tiles [5,3] = new Dirt ();
		tiles [5,4] = new Dirt ();
		tiles [5,5] = new Dirt ();
	}
}
