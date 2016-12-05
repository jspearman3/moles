using UnityEngine;
using System.Collections.Generic;

public class Wall : Tile {
    private static Dictionary<WallType, int> wallTypeDictionary = new Dictionary<WallType, int>()
    {
		
        {WallType.Filled,  0},
       
		{WallType.Mound,  1},

		{WallType.Protrude_Up,  2},
		{WallType.Protrude_Right,  3},
		{WallType.Protrude_Down,  4},
		{WallType.Protrude_Left,  5},

		{WallType.Angle_Out_UL,  6},
		{WallType.Angle_Out_UR,  7},
		{WallType.Angle_Out_DR,  8},
		{WallType.Angle_Out_DL,  9},

		{WallType.Angle_Skinny_UL,  10},
		{WallType.Angle_Skinny_UR,  11},
		{WallType.Angle_Skinny_DR,  12},
		{WallType.Angle_Skinny_DL,  13},

		{WallType.T_Up,  14},
		{WallType.T_Right,  15},
		{WallType.T_Down,  16},
		{WallType.T_Left,  17},

		{WallType.T_Dark1_Up,  18},
		{WallType.T_Dark1_Right,  19},
		{WallType.T_Dark1_Down,  20},
		{WallType.T_Dark1_Left,  21},

		{WallType.T_Dark2_Up,  22},
		{WallType.T_Dark2_Right,  23},
		{WallType.T_Dark2_Down,  24},
		{WallType.T_Dark2_Left,  25},

		{WallType.Up,  26},
		{WallType.Right,  27},
		{WallType.Down,  28},
		{WallType.Left,  29},

		{WallType.All_Way,  30},

		{WallType.All_Way_Dark_UL,  31},
		{WallType.All_Way_Dark_UR,  32},
		{WallType.All_Way_Dark_DR,  33},
		{WallType.All_Way_Dark_DL,  34},

		{WallType.T_DarkTop_Up,  35},
		{WallType.T_DarkTop_Right,  36},
		{WallType.T_DarkTop_Down,  37},
		{WallType.T_DarkTop_Left,  38},

		{WallType.Diagnal_UL_DR,  39},
		{WallType.Diagnal_UR_DL,  40},

		{WallType.Angle_In_UL,  41},
		{WallType.Angle_In_UR,  42},
		{WallType.Angle_In_DR,  43},
		{WallType.Angle_In_DL,  44},

		{WallType.Strip_Vert,  45},
		{WallType.Strip_Horiz,  46}

    };

	private int spriteIndex;


    public Wall(WallType type)
    {
        spriteIndex = wallTypeDictionary[type];
    }

	public override bool isWalkable() {
		return false;
	}

    public override int getSpriteIndex()
    {
        return spriteIndex;
    }

    public void setWallType(WallType type)
    {
        spriteIndex = wallTypeDictionary[type];
    }

}
