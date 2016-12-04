using UnityEngine;
using System.Collections;

public class MapData {

	public Tile[,] tiles;

	//default 9x9 map
	public MapData() {
		int length = 9;
		tiles = new Tile[length,length];

		for (int i = 0; i < length; i++) {
			for (int j = 0; j < length; j++) {

    			tiles [i, j] = new Darkness ();

			}
		}

		tiles [3,3] = new Dirt ();
		tiles [3,4] = new Dirt ();
		tiles [3,5] = new Dirt ();
		tiles [4,3] = new Dirt ();
		tiles [4,4] = new Dirt ();
		tiles [4,5] = new Dirt ();
		tiles [5,3] = new Dirt ();
		tiles [5,4] = new Dirt ();
        tiles [5,5] = new Dirt ();

        tiles[2, 2] = new Wall(WallType.UpLeft);
        tiles[3, 2] = new Wall(WallType.Up);
        tiles[4, 2] = new Wall(WallType.Up);
        tiles[5, 2] = new Wall(WallType.Up);
        tiles[6, 2] = new Wall(WallType.UpRight);

        tiles[2, 3] = new Wall(WallType.Left);
        tiles[2, 4] = new Wall(WallType.Left);
        tiles[2, 5] = new Wall(WallType.Left);
        tiles[2, 6] = new Wall(WallType.DownLeft);

        tiles[3, 6] = new Wall(WallType.Down);
        tiles[4, 6] = new Wall(WallType.Down);
        tiles[5, 6] = new Wall(WallType.Down);
        tiles[6, 6] = new Wall(WallType.DownRight);

        tiles[6, 3] = new Wall(WallType.Right);
        tiles[6, 4] = new Wall(WallType.Right);
        tiles[6, 5] = new Wall(WallType.Right);
    }

    public Tile getTile(int x, int y)
    {
        if (x >= tiles.GetLength(0) || y >= tiles.GetLength(1) || x < 0 || y < 0)
        {
            return null;
        }

        return tiles[x, y];
    }

    public void digWall(int x, int y)
    {
        Tile t = getTile(x, y);

        if (t == null)
        {
            return;
        }

        if (!t.GetType().Equals(typeof(Wall)))
        {
            return;
        }

    }

    private void updateWall(int x, int y)
    {
        Tile tile = getTile(x, y);

        if (tile == null || !tile.GetType().Equals(typeof(Wall)))
        {
            return;
        }

        //order: up right down left
        Tile[] adjacencies = new Tile[4];

        adjacencies[0] = getTile(x, y - 1);
        adjacencies[1] = getTile(x + 1, y);
        adjacencies[2] = getTile(x, y + 1);
        adjacencies[3] = getTile(x - 1, y);

        bool upIsWall = true;
        bool rightIsWall = true;
        bool downIsWall = true;
        bool leftIsWall = true;

        if (adjacencies[0] == null || !adjacencies[0].GetType().Equals(typeof(Wall)))
        {
            upIsWall = false;
        }

        if (adjacencies[1] == null || !adjacencies[1].GetType().Equals(typeof(Wall)))
        {
            rightIsWall = false;
        }

        if (adjacencies[2] == null || !adjacencies[2].GetType().Equals(typeof(Wall)))
        {
            downIsWall = false;
        }

        if (adjacencies[3] == null || !adjacencies[3].GetType().Equals(typeof(Wall)))
        {
            leftIsWall = false;
        }

        Wall wall = (Wall) tile;

        wall.setWallType(findWallTypeByAjacencies(upIsWall, rightIsWall, downIsWall, leftIsWall));

    }

    private WallType findWallTypeByAjacencies(bool upIsWall, bool rightIsWall, bool downIsWall, bool leftIsWall)
    {
        //TODO: figure out correct type
        return WallType.Down;
    }

    public void setTile(int x, int y, Tile tile)
    {
        if (x >= tiles.GetLength(0) || y >= tiles.GetLength(1) || x < 0 || y < 0)
        {
            return;
        }

        

        tiles[x, y] = tile;
    }
}
