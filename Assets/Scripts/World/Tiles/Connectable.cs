using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ConnectableTile : Tile {
	protected abstract Dictionary<ConnectableVariant, int>  ConnectableVariantDictionary { get; }

	public ConnectableTile(ConnectableVariant type)
	{
		spriteIndex =  ConnectableVariantDictionary[type];
	}

	public ConnectableTile(int enumNum) {
		ConnectableVariant variant = (ConnectableVariant)enumNum;
		spriteIndex = ConnectableVariantDictionary [variant];
	}

	public void setConnectableVariant(ConnectableVariant type)
	{
		spriteIndex = ConnectableVariantDictionary[type];
	}

	public virtual void updateVariant(Tile[] adjacencies) {
		ConnectableVariant variant = findConnectableVariant (adjacencies, this.GetType ());
		setConnectableVariant (variant);
	}

	private static ConnectableVariant findConnectableVariant(Tile[] adjacencies, Type type) {
		if (adjacencies.Length != 8) {
			Debug.Log ("Cannot find wall without proper ajacency array");
			return ConnectableVariant.Down;
		}

		bool[] adjacentWalls = new bool[8];

		for (int i = 0; i < 8; i++) {
			if (adjacencies [i] == null || !adjacencies [i].GetType ().Equals (type)) {
				adjacentWalls [i] = false;
			} else {
				adjacentWalls [i] = true;
			}
		}

		return findConnectableVariant (adjacentWalls);
	}


	private static ConnectableVariant findConnectableVariant(bool[] adjacencies)
	{
		if (adjacencies.Length != 8) {
			Debug.Log ("Cannot find wall without proper ajacency array");
			return ConnectableVariant.Down;
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
			return ConnectableVariant.None;
		}


		//one adjacent wall ==> 4 possibilities
		if (up && !right && !down && !left) {
			return ConnectableVariant.Protrude_Down;
		}
		if (!up && right && !down && !left) {
			return ConnectableVariant.Protrude_Left;
		}
		if (!up && !right && down && !left) {
			return ConnectableVariant.Protrude_Up;
		}
		if (!up && !right && !down && left) {
			return ConnectableVariant.Protrude_Right;
		}


		//Two adjacent walls
		//case 1: opposite each other ==> 2 possibilities
		if (up && !right && down && !left) {
			return ConnectableVariant.Strip_Vert;
		}
		if (!up && right && !down && left) {
			return ConnectableVariant.Strip_Horiz;
		}

		//case2: walls adjacent ==> 4 possibilities
		if (up && right && !down && !left) {
			if (ur) {
				return ConnectableVariant.Angle_Out_DL;
			} else {
				return ConnectableVariant.Angle_Skinny_DL;
			}

		}
		if (!up && right && down && !left) {
			if (dr) {
				return ConnectableVariant.Angle_Out_UL;
			} else {
				return ConnectableVariant.Angle_Skinny_UL;
			}
		}
		if (!up && !right && down && left) {
			if (dl) {
				return ConnectableVariant.Angle_Out_UR;
			} else {
				return ConnectableVariant.Angle_Skinny_UR;
			}
		}
		if (up && !right && !down && left) {
			if (ul) {
				return ConnectableVariant.Angle_Out_DR;
			} else {
				return ConnectableVariant.Angle_Skinny_DR;
			}
		}

		//Three walls
		if (up && right && !down && left) {
			if (ul && ur) {
				return ConnectableVariant.Up;
			} else if (ul) {
				return ConnectableVariant.T_Dark2_Up;
			} else if (ur) {
				return ConnectableVariant.T_Dark1_Up;
			} else {
				return ConnectableVariant.T_Up;
			}
		}
		if (up && right && down && !left) {
			if (ur && dr) {
				return ConnectableVariant.Right;
			} else if (ur) {
				return ConnectableVariant.T_Dark2_Right;
			} else if (dr) {
				return ConnectableVariant.T_Dark1_Right;
			} else {
				return ConnectableVariant.T_Right;
			}
		}
		if (!up && right && down && left) {
			if (dr && dl) {
				return ConnectableVariant.Down;
			} else if (dr) {
				return ConnectableVariant.T_Dark2_Down;
			} else if (dl) {
				return ConnectableVariant.T_Dark1_Down;
			} else {
				return ConnectableVariant.T_Down;
			}
		}
		if (up && !right && down && left) {
			if (dl && ul) {
				return ConnectableVariant.Left;
			} else if (dl) {
				return ConnectableVariant.T_Dark2_Left;
			} else if (ul) {
				return ConnectableVariant.T_Dark1_Left;
			} else {
				return ConnectableVariant.T_Left;
			}
		}

		if (up && right && down && left) {
			//cases where all filled and all empty. Filled first because most common situation
			if (ul && ur && dr && dl) {
				return ConnectableVariant.Full;
			} else if (!ul && !ur && !dr && !dl) {
				return ConnectableVariant.All_Way;

				//Do cases where one corner wall
			} else if (ul && !ur && !dr && !dl) {
				return ConnectableVariant.All_Way_Dark_UL;
			} else if (!ul && ur && !dr && !dl) {
				return ConnectableVariant.All_Way_Dark_UR;
			} else if (!ul && !ur && dr && !dl) {
				return ConnectableVariant.All_Way_Dark_DR;
			} else if (!ul && !ur && !dr && dl) {
				return ConnectableVariant.All_Way_Dark_DL;

				//cases with 2 same-side corners
			} else if (ul && ur && !dr && !dl) {
				return ConnectableVariant.T_DarkTop_Up;
			} else if (!ul && ur && dr && !dl) {
				return ConnectableVariant.T_DarkTop_Right;
			} else if (!ul && !ur && dr && dl) {
				return ConnectableVariant.T_DarkTop_Down;
			} else if (ul && !ur && !dr && dl) {
				return ConnectableVariant.T_DarkTop_Left;

				//cases with opposite corners
			} else if (ul && !ur && dr && !dl) {
				return ConnectableVariant.Diagnal_UL_DR;
			} else if (!ul && ur && !dr && dl) {
				return ConnectableVariant.Diagnal_UR_DL;

				//cases with 3 filled corners 
			} else if (!ul && ur && dr && dl) {
				return ConnectableVariant.Angle_In_DR;
			} else if (ul && !ur && dr && dl) {
				return ConnectableVariant.Angle_In_DL;
			} else if (ul && ur && !dr && dl) {
				return ConnectableVariant.Angle_In_UL;
			} else if (ul && ur && dr && !dl) {
				return ConnectableVariant.Angle_In_UR;
			}
		}

		Debug.Log ("COULDNT FIND VARIANT! FIXME!");
		return ConnectableVariant.None;
	}
}

public enum ConnectableVariant {
	Full,

	None,

	Protrude_Up,
	Protrude_Right,
	Protrude_Down,
	Protrude_Left,

	Angle_Out_UL,
	Angle_Out_UR,
	Angle_Out_DR,
	Angle_Out_DL,

	Angle_Skinny_UL,
	Angle_Skinny_UR,
	Angle_Skinny_DR,
	Angle_Skinny_DL,

	T_Up,
	T_Right,
	T_Down,
	T_Left,

	T_Dark1_Up,
	T_Dark1_Right,
	T_Dark1_Down,
	T_Dark1_Left,

	T_Dark2_Up,
	T_Dark2_Right,
	T_Dark2_Down,
	T_Dark2_Left,

	Up,
	Right,
	Down,
	Left,

	All_Way,

	All_Way_Dark_UL,
	All_Way_Dark_UR,
	All_Way_Dark_DR,
	All_Way_Dark_DL,

	T_DarkTop_Up,
	T_DarkTop_Right,
	T_DarkTop_Down,
	T_DarkTop_Left,

	Diagnal_UL_DR,
	Diagnal_UR_DL,

	Angle_In_UL,
	Angle_In_UR,
	Angle_In_DR,
	Angle_In_DL,

	Strip_Vert,
	Strip_Horiz

}