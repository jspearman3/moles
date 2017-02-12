using UnityEngine;
using System.Collections.Generic;

public class Wall : ConnectableTile {
	protected static Dictionary<ConnectableVariant, int>  wallTypeDictionary = new Dictionary<ConnectableVariant, int>()
    {
		
        {ConnectableVariant.Full,  0},
       
		{ConnectableVariant.None,  1},

		{ConnectableVariant.Protrude_Up,  2},
		{ConnectableVariant.Protrude_Right,  3},
		{ConnectableVariant.Protrude_Down,  4},
		{ConnectableVariant.Protrude_Left,  5},

		{ConnectableVariant.Angle_Out_UL,  6},
		{ConnectableVariant.Angle_Out_UR,  7},
		{ConnectableVariant.Angle_Out_DR,  8},
		{ConnectableVariant.Angle_Out_DL,  9},

		{ConnectableVariant.Angle_Skinny_UL,  10},
		{ConnectableVariant.Angle_Skinny_UR,  11},
		{ConnectableVariant.Angle_Skinny_DR,  12},
		{ConnectableVariant.Angle_Skinny_DL,  13},

		{ConnectableVariant.T_Up,  14},
		{ConnectableVariant.T_Right,  15},
		{ConnectableVariant.T_Down,  16},
		{ConnectableVariant.T_Left,  17},

		{ConnectableVariant.T_Dark1_Up,  18},
		{ConnectableVariant.T_Dark1_Right,  19},
		{ConnectableVariant.T_Dark1_Down,  20},
		{ConnectableVariant.T_Dark1_Left,  21},

		{ConnectableVariant.T_Dark2_Up,  22},
		{ConnectableVariant.T_Dark2_Right,  23},
		{ConnectableVariant.T_Dark2_Down,  24},
		{ConnectableVariant.T_Dark2_Left,  25},

		{ConnectableVariant.Up,  26},
		{ConnectableVariant.Right,  27},
		{ConnectableVariant.Down,  28},
		{ConnectableVariant.Left,  29},

		{ConnectableVariant.All_Way,  30},

		{ConnectableVariant.All_Way_Dark_UL,  31},
		{ConnectableVariant.All_Way_Dark_UR,  32},
		{ConnectableVariant.All_Way_Dark_DR,  33},
		{ConnectableVariant.All_Way_Dark_DL,  34},

		{ConnectableVariant.T_DarkTop_Up,  35},
		{ConnectableVariant.T_DarkTop_Right,  36},
		{ConnectableVariant.T_DarkTop_Down,  37},
		{ConnectableVariant.T_DarkTop_Left,  38},

		{ConnectableVariant.Diagnal_UL_DR,  39},
		{ConnectableVariant.Diagnal_UR_DL,  40},

		{ConnectableVariant.Angle_In_UL,  41},
		{ConnectableVariant.Angle_In_UR,  42},
		{ConnectableVariant.Angle_In_DR,  43},
		{ConnectableVariant.Angle_In_DL,  44},

		{ConnectableVariant.Strip_Vert,  45},
		{ConnectableVariant.Strip_Horiz,  46}

    };

	override
	protected Dictionary<ConnectableVariant, int> ConnectableVariantDictionary 
	{
		get { return wallTypeDictionary; }
	}

	public Wall (ConnectableVariant type) : base (type){}

	public override bool isWalkable() {
		return false;
	}

    public override int getSpriteIndex()
    {
        return spriteIndex;
    }
		
}
