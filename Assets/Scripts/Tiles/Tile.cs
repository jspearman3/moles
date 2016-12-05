using UnityEngine;
using System.Collections;

public abstract class Tile {
	public static int tileWidth = 16;
    public static Sprite sprite = Resources.Load<Sprite> ("Textures/Terrain/TerrainTiles");

	public abstract bool isWalkable ();
    public abstract int getSpriteIndex();

    public Sprite getSprite()
    {
        return sprite;
    }

    public static Vector2 getTexCoordsByIndex(int index)
    {
		int tilesPerRow = sprite.texture.width / tileWidth;
		int tilesPerColumn = sprite.texture.height / tileWidth;

        int row = tilesPerColumn - (index / tilesPerRow) - 1;
        int column = index % tilesPerRow;

        return new Vector2(row * tileWidth, column * tileWidth);
    }

    public static Texture2D getTile(int index)
    {
        Vector2 texCoords = getTexCoordsByIndex(index);

        Texture2D tex = new Texture2D(tileWidth, tileWidth);

        //get top left corner. GetPixel reads from bottom-left. Want to read from top left.
        int x = (int) texCoords.x + tileWidth - 1;
        int y = (int) texCoords.y;

        for (int i = 0; i <tileWidth; i ++)
        {
            for (int j = 0; j < tileWidth; j++)
            {
                Color c = sprite.texture.GetPixel(y + j, x - i);
                tex.SetPixel(i, j, c);
            }
        }

        return tex;
    }
}
