using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Tile {
	public static int tileWidthInPixels = 16;
    public static Sprite sprite = Resources.Load<Sprite> ("Textures/Terrain/TerrainTiles");

	protected int spriteIndex = 0; 

	public abstract bool isWalkable ();
    public abstract int getSpriteIndex();

    public Sprite getSprite()
    {
        return sprite;
    }

    public static Vector2 getTexCoordsByIndex(int index)
    {
		int tilesPerRow = sprite.texture.width / tileWidthInPixels;
		int tilesPerColumn = sprite.texture.height / tileWidthInPixels;

        int row = tilesPerColumn - (index / tilesPerRow) - 1;
        int column = index % tilesPerRow;

        return new Vector2(row * tileWidthInPixels, column * tileWidthInPixels);
    }

    public static Texture2D getTile(int index)
    {
        Vector2 texCoords = getTexCoordsByIndex(index);

        Texture2D tex = new Texture2D(tileWidthInPixels, tileWidthInPixels);

        //get top left corner. GetPixel reads from bottom-left. Want to read from top left.
        int x = (int) texCoords.x + tileWidthInPixels - 1;
        int y = (int) texCoords.y;

        for (int i = 0; i <tileWidthInPixels; i ++)
        {
            for (int j = 0; j < tileWidthInPixels; j++)
            {
                Color c = sprite.texture.GetPixel(y + j, x - i);
                tex.SetPixel(i, j, c);
            }
        }

        return tex;
    }

	virtual public Dictionary<Item, int> droppedItemsOnDestroy() {
		return new Dictionary<Item,int> ();
	}
}
