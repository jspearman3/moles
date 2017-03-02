using UnityEngine;
using System.Collections;

public class MapData {
	
	public Tile[,,] tiles;


	public MapData() {
		tiles = buildDefaultMap ().tiles;
	}

	public MapData(int length, int width, int depth) {

		tiles = new Tile[width, length, depth];

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < width; j++) {
				for (int k = 0; k < depth; k++) {
					tiles [i, j, k] = new Wall (ConnectableVariant.Full);
				}
			}
		}
	}


	//default 3x3 area in 40x40 map
	public static MapData buildDefaultMap() {
		MapData returnMap = new MapData (40, 40, 10);

		returnMap.tiles [3,3,1] = new Dirt ();
		returnMap.tiles [3,4,1] = new Dirt ();
		returnMap.tiles [3,5,1] = new Dirt ();
		returnMap.tiles [4,3,1] = new Dirt ();
		returnMap.tiles [4,4,1] = new Dirt ();
		returnMap.tiles [4,5,1] = new Dirt ();
		returnMap.tiles [5,3,1] = new Dirt ();
		returnMap.tiles [5,4,1] = new Dirt ();
		returnMap.tiles [5,5,1] = new Dirt ();


		returnMap.tiles[2, 2,1] = new Wall(ConnectableVariant.Angle_In_UL);
		returnMap.tiles[3, 2,1] = new Wall(ConnectableVariant.Up);
		returnMap.tiles[4, 2,1] = new Wall(ConnectableVariant.Up);
		returnMap.tiles[5, 2,1] = new Wall(ConnectableVariant.Up);
		returnMap.tiles[6, 2,1] = new Wall(ConnectableVariant.Angle_In_UR);



		returnMap.tiles[2, 3,1] = new Wall(ConnectableVariant.Left);
		returnMap.tiles[2, 4,1] = new Wall(ConnectableVariant.Left);
		returnMap.tiles[2, 5,1] = new Wall(ConnectableVariant.Left);
		returnMap.tiles[2, 6,1] = new Wall(ConnectableVariant.Angle_In_DL);

		returnMap.tiles[3, 6,1] = new Wall(ConnectableVariant.Down);
		returnMap.tiles[4, 6,1] = new Wall(ConnectableVariant.Down);
		returnMap.tiles[5, 6,1] = new Wall(ConnectableVariant.Down);
		returnMap.tiles[6, 6,1] = new Wall(ConnectableVariant.Angle_In_DR);

		returnMap.tiles[6, 3,1] = new Wall(ConnectableVariant.Right);
		returnMap.tiles[6, 4,1] = new Wall(ConnectableVariant.Right);
		returnMap.tiles[6, 5,1] = new Wall(ConnectableVariant.Right);

		returnMap.tiles[3, 3,1] = new Ladder();
		returnMap.tiles[5, 3,1] = new Ramp(96);
		returnMap.tiles[5, 2,1] = new Ramp(100);

		returnMap.tiles[3, 3,0] = new Air(ConnectableVariant.None);


		returnMap.tiles [3,2,0] = new Dirt ();
		returnMap.tiles [4,2,0] = new Dirt ();
		returnMap.tiles [4,3,0] = new Dirt ();

		returnMap.tiles[2, 2,0] = new Wall(ConnectableVariant.Left);
		returnMap.tiles[2, 3,0] = new Wall(ConnectableVariant.Left);
		returnMap.tiles[3, 4,0] = new Wall(ConnectableVariant.Down);
		returnMap.tiles[4, 4,0] = new Wall(ConnectableVariant.Down);
		returnMap.tiles[5, 2,0] = new Wall(ConnectableVariant.Right);
		returnMap.tiles[5, 3,0] = new Wall(ConnectableVariant.Right);
		returnMap.tiles[3, 1,0] = new Wall(ConnectableVariant.Up);
		returnMap.tiles[4, 1,0] = new Wall(ConnectableVariant.Up);
		returnMap.tiles[2, 1,0] = new Wall(ConnectableVariant.Angle_In_UL);
		returnMap.tiles[5, 1,0] = new Wall(ConnectableVariant.Angle_In_UR);
		returnMap.tiles[2, 4,0] = new Wall(ConnectableVariant.Angle_In_DL);
		returnMap.tiles[5, 4,0] = new Wall(ConnectableVariant.Angle_In_DR);

		returnMap.tiles[5, 4,1] = new Air(ConnectableVariant.Protrude_Up);
		returnMap.tiles[5, 5,1] = new Air(ConnectableVariant.Protrude_Down);
		returnMap.tiles [4,4,2] = new Dirt ();
		returnMap.tiles [5,4,2] = new Ladder ();
		returnMap.tiles [5,5,2] = new Dirt ();
		returnMap.tiles [4,5,2] = new Dirt ();

		returnMap.tiles[3, 4,2] = new Wall(ConnectableVariant.Left);
		returnMap.tiles[3, 5,2] = new Wall(ConnectableVariant.Left);
		returnMap.tiles[4, 6,2] = new Wall(ConnectableVariant.Down);
		returnMap.tiles[5, 6,2] = new Wall(ConnectableVariant.Down);
		returnMap.tiles[6, 4,2] = new Wall(ConnectableVariant.Right);
		returnMap.tiles[6, 5,2] = new Wall(ConnectableVariant.Right);
		returnMap.tiles[4, 3,2] = new Wall(ConnectableVariant.Up);
		returnMap.tiles[5, 3,2] = new Wall(ConnectableVariant.Up);
		returnMap.tiles[3, 3,2] = new Wall(ConnectableVariant.Angle_In_UL);
		returnMap.tiles[6, 3,2] = new Wall(ConnectableVariant.Angle_In_UR);
		returnMap.tiles[3, 6,2] = new Wall(ConnectableVariant.Angle_In_DL);
		returnMap.tiles[6, 6,2] = new Wall(ConnectableVariant.Angle_In_DR);

		return returnMap;
	}

	public Tile getTile(MapCoords coords) {
		return getTile (coords.x, coords.y, coords.depth);
	}

	public Tile getTile(int x, int y, int depth)
    {
		if (!validCoords(x, y, depth))
        {
            return null;
        }

        return tiles[x, y, depth];
    }

	private bool validCoords(int x, int y, int depth) {
		if (x >= tiles.GetLength(0) || y >= tiles.GetLength(1) || depth >= tiles.GetLength(2) || x < 0 || y < 0 || depth < 0)
		{
			return false;
		}

		return true;
	}

	public bool smartSet(MapCoords coords, Tile tile) {
		return smartSet (coords.x, coords.y, coords.depth, tile);
	}

	//return whether redraw is needed
    public bool smartSet(int x, int y, int depth, Tile newTile)
    {
        Tile tile = getTile(x, y, depth);

        if (tile == null || newTile == null)
        {
            return false;
        }

		setTile(x, y, depth, newTile);


		//if sprite is the same, no need to do graphical update
		if (tile.getSpriteIndex () == newTile.getSpriteIndex ()) {
			return false;
		}

		smartUpdate(x, y, depth);

		//update all surrounding tiles
		smartUpdate(x - 1, y - 1, depth);
		smartUpdate(x, y - 1, depth);
		smartUpdate(x + 1, y - 1, depth);
		smartUpdate(x - 1, y, depth);
		smartUpdate(x + 1, y, depth);
		smartUpdate(x - 1, y + 1, depth);
		smartUpdate(x, y + 1, depth);
		smartUpdate(x + 1, y + 1, depth);

		return true;
    }
		
    private void smartUpdate(int x, int y, int depth)
    {
		ConnectableTile tile = getTile(x, y, depth) as ConnectableTile;

		if (tile == null)
        {
            return;
        }

        //order: up right down left
        Tile[] adjacencies = new Tile[8];

        adjacencies[0] = getTile(x - 1, y - 1, depth);
		adjacencies[1] = getTile(x, y - 1, depth);
		adjacencies[2] = getTile(x + 1, y - 1, depth);
		adjacencies[3] = getTile(x + 1, y, depth);
		adjacencies[4] = getTile(x + 1, y + 1, depth);
		adjacencies[5] = getTile(x, y + 1, depth);
		adjacencies[6] = getTile(x - 1, y + 1, depth);
		adjacencies[7] = getTile(x - 1, y, depth);

        tile.updateVariant(adjacencies);

    }

	public void setTile(int x, int y, int depth, Tile tile)
    {
		if (!validCoords(x, y, depth))
        {
            return;
        }

        tiles[x, y, depth] = tile;
    }
}
