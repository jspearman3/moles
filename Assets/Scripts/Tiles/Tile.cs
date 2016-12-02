using UnityEngine;
using System.Collections;

public abstract class Tile {
	public static int pixelWidth = 16;
	public static Sprite[] sprites = Resources.LoadAll<Sprite> ("Textures/Terrain/Terrain");

	public abstract bool isWalkable ();
	public abstract Sprite getSprite();
}
