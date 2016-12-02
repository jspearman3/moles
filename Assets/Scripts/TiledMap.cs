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

	// Use this for initialization
	void Start () {
		mapData = new MapData();
		width = mapData.tiles.GetLength(0);
		height = mapData.tiles.GetLength(0);

		BuildMesh ();
	}

	private void BuildMesh() {

		float mapWidth = width * tileLength;
		float mapHeight = height * tileLength;

		Vector3 topLeft = new Vector3 (-mapWidth / 2, mapHeight / 2, 0);

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
		int texPixelWidth = mapData.tiles.GetLength(0) * Tile.pixelWidth;

		Debug.Log ("len: " + Tile.sprites.Length);


		Texture2D tex = new Texture2D (texPixelWidth, texPixelWidth);

		for (int i = 0; i < mapData.tiles.GetLength(0); i++) {
			for (int j = 0; j < mapData.tiles.GetLength(0); j++) {
				Tile t = mapData.tiles [i, j];

				drawTile (i, j, t, tex);
			}
		}

		tex.filterMode = FilterMode.Point;

		tex.Apply ();

		GetComponent<MeshRenderer> ().sharedMaterials [0].mainTexture = tex;
	}
	
	private void drawTile(int x, int y, Tile tile, Texture2D tex) {
		int xStart = x * Tile.pixelWidth;
		int xEnd = x * (Tile.pixelWidth + 1);

		int yStart = y * Tile.pixelWidth;
		int yEnd = y * (Tile.pixelWidth + 1);

		Texture spriteTex = tile.getSprite ().texture;
		Vector2 offset = tile.getSprite ().textureRectOffset;

		for (int i = 0; i < Tile.pixelWidth; i++) {
			for (int j = 0; j < Tile.pixelWidth; j++) {
				Vector2 spritePixel = offset + new Vector2 (i, j);
				tex.SetPixel (xStart + i, yStart + j, tile.getSprite ().texture.GetPixel ((int)spritePixel.x, (int)spritePixel.y));
			}
		}

	}
}
