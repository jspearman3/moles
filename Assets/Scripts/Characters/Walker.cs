using UnityEngine;
using System.Collections;

public class Walker : GameEntity {

    protected float width = 0;
    protected float height = 0;

    protected override void InitializeObject()
    {
        map = GameObject.FindGameObjectWithTag("WorldMap").GetComponent<TiledMap>();
    }

    protected void Move(Vector2 translation)
    {

		Tile destTile = map.getTileFromGamePosition(gamePos.addWorld(translation));

        if (destTile.isWalkable())
        {
			gamePos.planeTranslateWorld(translation);
        }
    }

}
