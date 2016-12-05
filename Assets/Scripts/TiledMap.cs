using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TiledMap : MonoBehaviour {
	public int width = 100;
	public int height = 100;

	public Sprite[] terrain;

	private MapData mapData;

	public float tileLength = 1.0f;

    private Vector3 topLeftPoint;
    private MeshRenderer renderer;

	// Use this for initialization
	void Start () {
        topLeftPoint = GetComponent<Transform>().position;
        renderer = GetComponent<MeshRenderer>();
		mapData = new MapData();
		width = mapData.tiles.GetLength(0);
		height = mapData.tiles.GetLength(0);

		BuildMesh ();
	}

	private void BuildMesh() {

		float mapWidth = width * tileLength;
		float mapHeight = height * tileLength;

        //GetComponent<Transform>().position = new Vector3(-width * tileLength / 2f, height * tileLength / 2);

		Vector3 topLeft = new Vector3 (0, 0, 0);

		//make vertices for mesh. Simple square. No need for height info in 2D game
		//  0           1
		//  o---------o
		//  |       / |
		//  |    /    |
		//  | /       |
		//  o---------o
		//  2         3
		//
		Vector3[] vertices = new Vector3[4];
		vertices [0] = topLeft;
		vertices [1] = topLeft + new Vector3 (mapWidth, 0, 0);
		vertices [2] = topLeft + new Vector3 (0, -mapHeight, 0);
		vertices [3] = topLeft + new Vector3 (mapWidth, -mapHeight, 0);

		int[] triangles = new int[6];

		//triangle 1
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;

		//triangle 2
		triangles[3] = 1;
		triangles[4] = 3;
		triangles[5] = 2;

		Vector3[] normals = new Vector3[4];
		normals [0] = new Vector3(0, 0, 1);
		normals [1] = new Vector3(0, 0, 1);
		normals [2] = new Vector3(0, 0, 1);
		normals [3] = new Vector3(0, 0, 1);

		//set texture uv coords
		Vector2[] uv = new Vector2[4];

		uv[0] = new Vector2(0, 0); 
		uv[1] = new Vector2(0, 1); 
		uv[2] = new Vector2(1, 0); 
		uv[3] = new Vector2(1, 1); 

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;

		GetComponent<MeshFilter> ().mesh = mesh;

		DrawMap ();



	}

	private void DrawMap() {
		int texPixelWidth = mapData.tiles.GetLength(0) * Tile.tileWidth;



		Texture2D tex = new Texture2D (texPixelWidth, texPixelWidth);

		for (int i = 0; i < mapData.tiles.GetLength(0); i++) {
			for (int j = 0; j < mapData.tiles.GetLength(1); j++) {
				Tile t = mapData.tiles [i, j];

				drawTile (i, j, t, tex);
			}
		}

		tex.filterMode = FilterMode.Point;

		tex.Apply ();

		renderer.sharedMaterials [0].mainTexture = tex;
	}
	
	private void drawTile(int x, int y, Tile tile, Texture2D tex) {
		int xStart = x * Tile.tileWidth;

		int yStart = y * Tile.tileWidth;

        Texture2D tileTex = Tile.getTile(tile.getSpriteIndex());

		for (int i = 0; i < Tile.tileWidth; i++) {
			for (int j = 0; j < Tile.tileWidth; j++) {

                Color c = tileTex.GetPixel(j, i);

				tex.SetPixel (yStart + j, xStart + i, c);
			}
		}

	}

    //Get tile at x,y and surrounding tiles
    public Tile[,] getNonaTile(int x, int y)
    {
        Tile[,] returnTiles = new Tile[3, 3];

        returnTiles[0, 0] = mapData.getTile(x - 1, y - 1);
        returnTiles[1, 0] = mapData.getTile(x, y - 1);
        returnTiles[2, 0] = mapData.getTile(x + 1, y - 1);
        returnTiles[0, 1] = mapData.getTile(x - 1, y);
        returnTiles[1, 1] = mapData.getTile(x, y);
        returnTiles[2, 1] = mapData.getTile(x + 1, y);
        returnTiles[0, 2] = mapData.getTile(x, y + 1);
        returnTiles[1, 2] = mapData.getTile(x - 1, y + 1);
        returnTiles[2, 2] = mapData.getTile(x + 1, y + 1);

        return returnTiles;
    }

    public Tile getTileFromWorldSpace(Vector2 location)
    {
        Vector2 worldCoord = location - new Vector2(topLeftPoint.x, topLeftPoint.y);

        int x = (int) Mathf.Floor(worldCoord.x);
        int y = (int) Mathf.Floor(-worldCoord.y);
        return mapData.getTile(x, y);

    }

    public void setTileFromWorldSpace(Vector2 location, Tile tile)
    {
        Vector2 worldCoord = location - new Vector2(topLeftPoint.x, topLeftPoint.y);

        int x = (int)Mathf.Floor(worldCoord.x);
        int y = (int)Mathf.Floor(-worldCoord.y);

        mapData.setTile(x, y, tile);

		Texture2D t = (Texture2D)renderer.sharedMaterials[0].mainTexture;

        drawTile(x, y, tile, t);
        t.Apply();

    }



	public void digFromWorldSpace(Vector2 location) {
		Vector2 mapCoords = getMapCoordsFromWorldSpace (location);

		Tile[,] nona = getNonaTile ((int)mapCoords.x, (int)mapCoords.y);
		Tile tile = nona [1, 1];

		if (tile == null || !tile.GetType().Equals(typeof(Wall)))
			{
				return;
			}

		Texture2D t = (Texture2D)renderer.sharedMaterials[0].mainTexture;

		mapData.digWall ((int)mapCoords.x, (int)mapCoords.y);

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				redrawTile ((int)mapCoords.x - 1 + i, (int)mapCoords.y - 1 + j);
			}
		}
	}

	private void redrawTile(int x, int y) {
		Tile t = mapData.getTile (x, y);
		if (t == null) {
			return;
		}

		Texture2D tex = (Texture2D)renderer.sharedMaterials [0].mainTexture;
		drawTile (x, y, t, tex);
		tex.Apply ();
	}

	public Vector2 getMapCoordsFromWorldSpace(Vector2 location) {

		Vector2 worldCoord = location - new Vector2(topLeftPoint.x, topLeftPoint.y);

		int x = (int)Mathf.Floor(worldCoord.x);
		int y = (int)Mathf.Floor(-worldCoord.y);

		return new Vector2 (x, y);
	}

    public Tile[,] getNonaTileFromWorldspace(Vector2 location)
    {
        Tile[,] nonaTile = new Tile[3, 3];

        nonaTile[0, 0] = getTileFromWorldSpace(location + new Vector2(-1, -1));
        nonaTile[1, 0] = getTileFromWorldSpace(location + new Vector2(0, -1));
        nonaTile[2, 0] = getTileFromWorldSpace(location + new Vector2(1, -1));
        nonaTile[0, 1] = getTileFromWorldSpace(location + new Vector2(-1, 0));
        nonaTile[1, 1] = getTileFromWorldSpace(location + new Vector2(0, 0));
        nonaTile[2, 1] = getTileFromWorldSpace(location + new Vector2(1, 0));
        nonaTile[0, 2] = getTileFromWorldSpace(location + new Vector2(-1, 1));
        nonaTile[1, 2] = getTileFromWorldSpace(location + new Vector2(0, 1));
        nonaTile[2, 2] = getTileFromWorldSpace(location + new Vector2(1, 1));

        return nonaTile;
    }
}
