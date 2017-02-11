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
					tiles [i, j, k] = new Wall (WallType.Filled);
				}
			}
		}
	}


	//default 3x3 area in 40x40 map
	public static MapData buildDefaultMap() {
		MapData returnMap = new MapData (40, 40, 3);

		returnMap.tiles [3,3,1] = new Dirt ();
		returnMap.tiles [3,4,1] = new Dirt ();
		returnMap.tiles [3,5,1] = new Dirt ();
		returnMap.tiles [4,3,1] = new Dirt ();
		returnMap.tiles [4,4,1] = new Dirt ();
		returnMap.tiles [4,5,1] = new Dirt ();
		returnMap.tiles [5,3,1] = new Dirt ();
		returnMap.tiles [5,4,1] = new Dirt ();
		returnMap.tiles [5,5,1] = new Dirt ();

		returnMap.tiles [6,6,2] = new Dirt ();
		returnMap.tiles [2,2,0] = new Dirt ();
		returnMap.tiles [3,3,0] = new Air ();
		returnMap.tiles [2,3,0] = new Air ();
		returnMap.tiles [4,3,0] = new Air ();
		returnMap.tiles [2,4,0] = new Air ();
		returnMap.tiles [3,4,0] = new Air ();
		returnMap.tiles [4,4,0] = new Air ();
		returnMap.tiles [3,3,0] = new Air ();
		returnMap.tiles [4,4,1] = new Air ();
		returnMap.tiles [4,5,1] = new Air ();
		returnMap.tiles [3,2,0] = new Dirt ();
		returnMap.tiles [2,2,0] = new Dirt ();
		returnMap.tiles [4,2,0] = new Dirt ();

		returnMap.tiles[2, 2,0] = new Wall(WallType.Left);
		returnMap.tiles[2, 3,0] = new Wall(WallType.Left);
		returnMap.tiles[2, 4,0] = new Wall(WallType.Left);
		returnMap.tiles[2, 5,0] = new Wall(WallType.Left);

		returnMap.tiles[2, 2,1] = new Wall(WallType.Angle_In_UL);
		returnMap.tiles[3, 2,1] = new Stair();
		returnMap.tiles[4, 2,1] = new Wall(WallType.Up);
		returnMap.tiles[5, 2,1] = new Wall(WallType.Up);
		returnMap.tiles[6, 2,1] = new Wall(WallType.Angle_In_UR);

		returnMap.tiles[2, 3,1] = new Wall(WallType.Left);
		returnMap.tiles[2, 4,1] = new Wall(WallType.Left);
		returnMap.tiles[2, 5,1] = new Wall(WallType.Left);
		returnMap.tiles[2, 6,1] = new Wall(WallType.Angle_In_DL);

		returnMap.tiles[3, 6,1] = new Wall(WallType.Down);
		returnMap.tiles[4, 6,1] = new Wall(WallType.Down);
		returnMap.tiles[5, 6,1] = new Wall(WallType.Down);
		returnMap.tiles[6, 6,1] = new Wall(WallType.Angle_In_DR);

		returnMap.tiles[6, 3,1] = new Wall(WallType.Right);
		returnMap.tiles[6, 4,1] = new Wall(WallType.Right);
		returnMap.tiles[6, 5,1] = new Wall(WallType.Right);

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

	public void digWall(MapCoords coords) {
		digWall (coords.x, coords.y, coords.depth);
	}

    public void digWall(int x, int y, int depth)
    {
        Tile t = getTile(x, y, depth);

        if (t == null)
        {
            return;
        }

        if (!t.GetType().Equals(typeof(Wall)))
        {
            return;
        }

		setTile(x, y, depth, new Dirt());

		//update all surrounding walls
		updateWall(x - 1, y - 1, depth);
		updateWall(x, y - 1, depth);
		updateWall(x + 1, y - 1, depth);
		updateWall(x - 1, y, depth);
		updateWall(x + 1, y, depth);
		updateWall(x - 1, y + 1, depth);
		updateWall(x, y + 1, depth);
		updateWall(x + 1, y + 1, depth);
    }

    private void updateWall(int x, int y, int depth)
    {
		Tile tile = getTile(x, y, depth);

        if (tile == null || !tile.GetType().Equals(typeof(Wall)))
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

		bool[] adjacentWalls = new bool[8];

		for (int i = 0; i < 8; i++) {
			if (adjacencies [i] == null || !adjacencies [i].GetType ().Equals (typeof(Wall))) {
				adjacentWalls [i] = false;
			} else {
				adjacentWalls [i] = true;
			}
		}
			
        Wall wall = (Wall) tile;

        wall.setWallType(findWallTypeByAjacencies(adjacentWalls));

    }

    private WallType findWallTypeByAjacencies(bool[] adjacencies)
    {
		if (adjacencies.Length != 8) {
			Debug.Log ("Cannot find wall without proper ajacency array");
			return WallType.Down;
		}

		bool ul = adjacencies [0];
		bool up = adjacencies [1];
		bool ur = adjacencies [2];
		bool right = adjacencies [3];
		bool dr = adjacencies [4];
		bool down = adjacencies [5];
		bool dl = adjacencies [6];
		bool left = adjacencies [7];


		//This can be simplified by considering only up, right, down, left first. Use corners for special cases.
		//45 total cases.

		//no direct adjacencies ==> 1 posibility
		if (!up && !right && !down && !left) {
			return WallType.Mound;
		}


		//one adjacent wall ==> 4 possibilities
		if (up && !right && !down && !left) {
			return WallType.Protrude_Down;
		}
		if (!up && right && !down && !left) {
			return WallType.Protrude_Left;
		}
		if (!up && !right && down && !left) {
			return WallType.Protrude_Up;
		}
		if (!up && !right && !down && left) {
			return WallType.Protrude_Right;
		}


		//Two adjacent walls
		//case 1: opposite each other ==> 2 possibilities
		if (up && !right && down && !left) {
			return WallType.Strip_Vert;
		}
		if (!up && right && !down && left) {
			return WallType.Strip_Horiz;
		}

		//case2: walls adjacent ==> 4 possibilities
		if (up && right && !down && !left) {
			if (ur) {
				return WallType.Angle_Out_DL;
			} else {
				return WallType.Angle_Skinny_DL;
			}

		}
		if (!up && right && down && !left) {
			if (dr) {
				return WallType.Angle_Out_UL;
			} else {
				return WallType.Angle_Skinny_UL;
			}
		}
		if (!up && !right && down && left) {
			if (dl) {
				return WallType.Angle_Out_UR;
			} else {
				return WallType.Angle_Skinny_UR;
			}
		}
		if (up && !right && !down && left) {
			if (ul) {
				return WallType.Angle_Out_DR;
			} else {
				return WallType.Angle_Skinny_DR;
			}
		}

		//Three walls
		if (up && right && !down && left) {
			if (ul && ur) {
				return WallType.Up;
			} else if (ul) {
				return WallType.T_Dark2_Up;
			} else if (ur) {
				return WallType.T_Dark1_Up;
			} else {
				return WallType.T_Up;
			}
		}
		if (up && right && down && !left) {
			if (ur && dr) {
				return WallType.Right;
			} else if (ur) {
				return WallType.T_Dark2_Right;
			} else if (dr) {
				return WallType.T_Dark1_Right;
			} else {
				return WallType.T_Right;
			}
		}
		if (!up && right && down && left) {
			if (dr && dl) {
				return WallType.Down;
			} else if (dr) {
				return WallType.T_Dark2_Down;
			} else if (dl) {
				return WallType.T_Dark1_Down;
			} else {
				return WallType.T_Down;
			}
		}
		if (up && !right && down && left) {
			if (dl && ul) {
				return WallType.Left;
			} else if (dl) {
				return WallType.T_Dark2_Left;
			} else if (ul) {
				return WallType.T_Dark1_Left;
			} else {
				return WallType.T_Left;
			}
		}

		if (up && right && down && left) {
			//cases where all filled and all empty. Filled first because most common situation
			if (ul && ur && dr && dl) {
				return WallType.Filled;
			} else if (!ul && !ur && !dr && !dl) {
				return WallType.All_Way;

				//Do cases where one corner wall
			} else if (ul && !ur && !dr && !dl) {
				return WallType.All_Way_Dark_UL;
			} else if (!ul && ur && !dr && !dl) {
				return WallType.All_Way_Dark_UR;
			} else if (!ul && !ur && dr && !dl) {
				return WallType.All_Way_Dark_DR;
			} else if (!ul && !ur && !dr && dl) {
				return WallType.All_Way_Dark_DL;

				//cases with 2 same-side corners
			} else if (ul && ur && !dr && !dl) {
				return WallType.T_DarkTop_Up;
			} else if (!ul && ur && dr && !dl) {
				return WallType.T_DarkTop_Right;
			} else if (!ul && !ur && dr && dl) {
				return WallType.T_DarkTop_Down;
			} else if (ul && !ur && !dr && dl) {
				return WallType.T_DarkTop_Left;

				//cases with opposite corners
			} else if (ul && !ur && dr && !dl) {
				return WallType.Diagnal_UL_DR;
			} else if (!ul && ur && !dr && dl) {
				return WallType.Diagnal_UR_DL;

				//cases with 3 filled corners 
			} else if (!ul && ur && dr && dl) {
				return WallType.Angle_In_DR;
			} else if (ul && !ur && dr && dl) {
				return WallType.Angle_In_DL;
			} else if (ul && ur && !dr && dl) {
				return WallType.Angle_In_UL;
			} else if (ul && ur && dr && !dl) {
				return WallType.Angle_In_UR;
			}
		}

		Debug.Log ("COULDNT FIND WALL TYPE! FIXME!");
        return WallType.Down;
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
