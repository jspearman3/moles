using UnityEngine;
using System.Collections;

public class Walker : GameEntity {

    protected TiledMap map;
    protected float width = 0;
    protected float height = 0;

    protected override void InitializeObject()
    {
        map = GameObject.FindGameObjectWithTag("WorldMap").GetComponent<TiledMap>();
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
}
