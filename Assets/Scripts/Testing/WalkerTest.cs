using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerTest : MonoBehaviour {

	protected TiledMapTest map;
	protected float width = 0;
	protected float height = 0;
	protected Transform trans;

	void Awake() {
		trans = GetComponent<Transform> ();
		map = GameObject.FindGameObjectWithTag("WorldMap").GetComponent<TiledMapTest>();
	}
		

	protected void Move(Vector2 translation)
	{

		Vector2 pos = new Vector2(trans.position.x, trans.position.y);

		Tile destTile = map.getTileFromWorldSpace(pos + translation);

		if (destTile.isWalkable())
		{
			trans.Translate(translation);
		}
	}

	protected Vector2 get2DPos()
	{
		return new Vector2(trans.position.x, trans.position.y);
	}
}
