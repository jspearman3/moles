using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//[RequireComponent(typeof(MeshFilter))]
//[RequireComponent(typeof(MeshRenderer))]
public class TiledMap : NetworkBehaviour {
	public const float TILE_LENGTH = 1.0f;
	public const float LEVEL_OFFSET = 0.6f;

	public int mapWidth = 100;
	public int mapHeight = 100;
	public int mapDepth = 100;

	public Sprite[] terrain;

	public MapData mapData;

	public GameObject itemPrefab;

    private Vector2 topLeftPoint;
	public GameObject[] worldLevels;

	// Use this for initialization
	void Start () {
        topLeftPoint = GetComponent<Transform>().position;

		if (isServer) {
			//mapData = MapData.buildDefaultMap ();
			//BuildGameWorld ();
		}
	}

	private void BuildGameWorld() {
		mapWidth = mapData.tiles.GetLength(0);
		mapHeight = mapData.tiles.GetLength(1);
		mapDepth = mapData.tiles.GetLength(2);

		worldLevels = new GameObject[mapDepth];

		for (int i = 0; i < mapDepth; i++) {
			GameObject worldLevel = new GameObject ("World depth " + i);
			worldLevels [i] = worldLevel;
			SpriteRenderer renderer = worldLevel.AddComponent<SpriteRenderer> ();
			MapLevelRenderingController mlrc = worldLevel.AddComponent<MapLevelRenderingController> ();
			mlrc.levelDepth = i;
			worldLevel.transform.parent = gameObject.transform;
			worldLevel.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, -90));
			worldLevel.transform.position = getLevelTopLeftRefPoint(i);
			DrawPlane (i, renderer);
		}

	}

	private void DrawPlane(int depth, SpriteRenderer renderer) {
		renderer.sortingOrder = (mapDepth - depth - 1) * 10;
		int texPixelWidth = mapData.tiles.GetLength(0) * Tile.tileWidthInPixels;
		int texPixelHeight = mapData.tiles.GetLength(1) * Tile.tileWidthInPixels;



		Texture2D tex = new Texture2D (texPixelWidth, texPixelHeight);

		for (int i = 0; i < mapData.tiles.GetLength(0); i++) {
			for (int j = 0; j < mapData.tiles.GetLength(1); j++) {
				Tile t = mapData.tiles [i, j, depth];

				drawTile (i, j, t, tex);
			}
		}

		tex.filterMode = FilterMode.Point;

		tex.Apply ();

		renderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero, Tile.tileWidthInPixels / TILE_LENGTH);
		//GetComponent<Transform> ().rotation = Quaternion.Euler (new Vector3 (0, 0, -90));
	}
	
	private void drawTile(int x, int y, Tile tile, Texture2D tex) {
		int xStart = x * Tile.tileWidthInPixels;

		int yStart = y * Tile.tileWidthInPixels;

        Texture2D tileTex = Tile.getTile(tile.getSpriteIndex());

		for (int i = 0; i < Tile.tileWidthInPixels; i++) {
			for (int j = 0; j < Tile.tileWidthInPixels; j++) {

                Color c = tileTex.GetPixel(j, i);

				tex.SetPixel (yStart + j, xStart + i, c);
			}
		}

	}

	public Tile[,] getNonaTile(MapCoords coords) {
		return getNonaTile (coords.x, coords.y, coords.depth);
	}

    //Get tile at x,y and surrounding tiles
	public Tile[,] getNonaTile(int x, int y, int depth)
    {
        Tile[,] returnTiles = new Tile[3, 3];

		returnTiles[0, 0] = mapData.getTile(x - 1, y - 1, depth);
		returnTiles[1, 0] = mapData.getTile(x, y - 1, depth);
		returnTiles[2, 0] = mapData.getTile(x + 1, y - 1, depth);
		returnTiles[0, 1] = mapData.getTile(x - 1, y, depth);
		returnTiles[1, 1] = mapData.getTile(x, y, depth);
		returnTiles[2, 1] = mapData.getTile(x + 1, y, depth);
		returnTiles[0, 2] = mapData.getTile(x, y + 1, depth);
		returnTiles[1, 2] = mapData.getTile(x - 1, y + 1, depth);
		returnTiles[2, 2] = mapData.getTile(x + 1, y + 1, depth);

        return returnTiles;
    }

	public Tile getTileFromGamePosition(GamePosition pos)
    {
		MapCoords coords = pos.toMapCoords();
		if (mapData == null) {
			return new Wall (ConnectableVariant.All_Way);
		}
        return mapData.getTile(coords);

    }

	public void setTileFromWorldSpace(Vector2 location, int depth, Tile tile)
    {
        Vector2 worldCoord = location - new Vector2(topLeftPoint.x, topLeftPoint.y);

        int x = (int)Mathf.Floor(worldCoord.x);
        int y = (int)Mathf.Floor(-worldCoord.y);

		mapData.setTile(x, y, depth, tile);

		Texture2D t = worldLevels[depth].GetComponent<SpriteRenderer>().sprite.texture;

        drawTile(x, y, tile, t);
        t.Apply();

    }


	public void dig(GamePosition pos) {
		MapCoords mapCoords = pos.toMapCoords();
	
		Tile tile = mapData.getTile (mapCoords);

		if (tile.GetType ().Equals (typeof(Wall))) {
			GameObject item = GameObject.Instantiate (itemPrefab);
			ItemEntity ie = item.GetComponent<ItemEntity> ();
			ie.gamePos = new GamePosition (mapCoords.toGamePosition ().planePosition + Random.insideUnitCircle * 0.25f, mapCoords.toGamePosition ().depth - 0.5f);
			ie.setIdentity (new RockItem());
			ie.syncPos = ie.gamePos.toStruct ();
			NetworkServer.Spawn (item);

			GameObject item2 = GameObject.Instantiate (itemPrefab);
			ItemEntity ie2 = item2.GetComponent<ItemEntity> ();
			ie2.gamePos = new GamePosition (mapCoords.toGamePosition ().planePosition + Random.insideUnitCircle * 0.25f, mapCoords.toGamePosition ().depth - 0.5f);
			ie2.setIdentity(new DirtItem());
			ie2.syncPos = ie2.gamePos.toStruct ();
			NetworkServer.Spawn (item2);
		}
			
		RpcDig (pos.toStruct());
	}

	public void place(GamePosition pos) {
		RpcPlace (pos.toStruct());
	}

	[ClientRpc]
	public void RpcPlace(GamePosStruct posStruct) {

		GamePosition pos = GamePosition.ParseStruct (posStruct);
		MapCoords mapCoords = pos.toMapCoords();


		Tile tile = mapData.getTile (mapCoords);

		bool redraw = false;

		switch (tile.GetType().Name)
		{
		case "Wall":
			Wall wall = (Wall)tile;

			if (wall.getSpriteIndex() == 26 ) {

				MapCoords upperTileCoords = mapCoords.add (0, 1, -1);
				if (upperTileCoords.depth < 0) {
					return;
				}
				bool wasChange = mapData.smartSet (upperTileCoords, new Air (ConnectableVariant.None));
				if (wasChange) {
					redrawNonaTile (upperTileCoords);
				}

				mapData.smartSet (mapCoords.add(y:1), new Ladder ());
				redrawNonaTile (mapCoords);


			}

			break;
		case "Dirt":
			mapData.smartSet (mapCoords, new Wall (ConnectableVariant.None));
			redrawNonaTile (mapCoords);

			MapCoords upperCoord = mapCoords.add (depth: -1);
			Tile upperTile = getNonaTile (upperCoord) [1, 1];
			if (upperTile == null) {
				return;
			}
			if (upperTile.GetType().Equals(typeof(Air))) {
				mapData.smartSet (upperCoord, new Dirt());
				redrawNonaTile (upperCoord);
			}

			break;
		case "Air":
			mapData.smartSet (mapCoords, new Dirt());
			redrawNonaTile (mapCoords);
			break;
		case "Ladder":
			mapData.smartSet (mapCoords, new Dirt());
			redrawNonaTile (mapCoords);
			break;
		default:
			return;
		}
			

	}

	[ClientRpc]
	public void RpcDig(GamePosStruct posStruct) {

		GamePosition pos = GamePosition.ParseStruct (posStruct);
		MapCoords mapCoords = pos.toMapCoords();


		Tile tile = mapData.getTile (mapCoords);

		bool redraw = false;
		if (tile.GetType ().Equals (typeof(Wall))) {
			redraw = mapData.smartSet (mapCoords, new Dirt ());
		} else if (tile.GetType ().Equals (typeof(Dirt))) {
			MapCoords lowerCoords = mapCoords.add (depth: 1);
			Tile lowTile = mapData.getTile (lowerCoords);
			if (lowTile != null && lowTile.GetType ().Equals (typeof(Wall))) {
				bool redrawLower = mapData.smartSet (lowerCoords, new Dirt());

				if (redrawLower) {
					redrawNonaTile (lowerCoords);
				}


			}

			if (lowTile != null) {
				redraw = mapData.smartSet (mapCoords, new Air (ConnectableVariant.None));
			}


		}

		if (redraw) {
			redrawNonaTile (mapCoords);
		}

	}

	private void redrawNonaTile(MapCoords mapCoords) {
		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				redrawTile (mapCoords.x - 1 + i, mapCoords.y - 1 + j,  mapCoords.depth);
			}
		}
	}

	private void redrawTile(int x, int y, int depth) {
		Tile t = mapData.getTile (x, y, depth);
		if (t == null) {
			return;
		}

		Texture2D tex = worldLevels[depth].GetComponent<SpriteRenderer>().sprite.texture;
		drawTile (x, y, t, tex);
		tex.Apply ();
	}
		

	private Vector2 getLevelTopLeftRefPoint(int depth) {
		return topLeftPoint + depth * LEVEL_OFFSET * Vector2.down;
	}

	public Tile[,] getNonaTileFromGamePosition(GamePosition pos)
    {
        Tile[,] nonaTile = new Tile[3, 3];

		nonaTile[0, 0] = getTileFromGamePosition(pos.add(new Vector2(-1, -1)));
		nonaTile[1, 0] =  getTileFromGamePosition(pos.add(new Vector2(0, -1)));
		nonaTile[2, 0] =  getTileFromGamePosition(pos.add(new Vector2(1, -1)));
		nonaTile[0, 1] = getTileFromGamePosition(pos.add(new Vector2(-1, 0)));
		nonaTile[1, 1] = getTileFromGamePosition(pos.add(new Vector2(0, 0)));
		nonaTile[2, 1] = getTileFromGamePosition(pos.add(new Vector2(1, 0)));
		nonaTile[0, 2] = getTileFromGamePosition(pos.add(new Vector2(-1, 1)));
		nonaTile[1, 2] = getTileFromGamePosition(pos.add(new Vector2(0, 1)));
		nonaTile[2, 2] = getTileFromGamePosition(pos.add(new Vector2(1, 1)));

        return nonaTile;
    }

	[TargetRpc]
	public void TargetSetAndApplyMap(NetworkConnection conn, byte[] serverMapData) {
		string dirt = new Dirt ().Encode ();
		Debug.Log ("applying server map " + dirt);
		NetworkReader reader = new NetworkReader (serverMapData);
		MapData newMapData = new MapData();
		newMapData.Deserialize (reader);
		mapData = newMapData;
		BuildGameWorld ();
	}
	/*
	[Command]
	void CmdFetchMapData() {
		TiledMap serverMap = GameObject.FindGameObjectWithTag ("WorldMap").GetComponent<TiledMap>();
		MapData serverMapData = serverMap.mapData;

		NetworkWriter writer = new NetworkWriter ();
		serverMapData.Serialize (writer);

		TargetSetAndApplyMap (GetComponent<NetworkIdentity>().connectionToClient, writer.AsArray());
	}

	bool phase1Init = false;
	bool phase2Init = false;

	void Update() {
		if (isServer)
			return;
		
		if (!phase1Init) {
			Debug.Log ("fetching map data");
			CmdFetchMapData ();
			phase1Init = true;
			return;
		}

		if (!phase2Init) {
			if (mapData == null) {
				return;
			}
			Debug.Log ("recieved map data");
			BuildGameWorld ();
			phase2Init = true;
		}

	}*/
}
