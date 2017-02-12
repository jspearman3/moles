using UnityEngine;
using System.Collections.Generic;
using System;

public class Air : ConnectableTile {
	protected static Dictionary<ConnectableVariant, int>  airTypeDictionary = new Dictionary<ConnectableVariant, int>()
	{

		{ConnectableVariant.Full,  48},

		{ConnectableVariant.None,  49},

		{ConnectableVariant.Protrude_Up,  50},
		{ConnectableVariant.Protrude_Right,  51},
		{ConnectableVariant.Protrude_Down,  52},
		{ConnectableVariant.Protrude_Left,  53},

		{ConnectableVariant.Angle_Out_UL,  54},
		{ConnectableVariant.Angle_Out_UR,  55},
		{ConnectableVariant.Angle_Out_DR,  56},
		{ConnectableVariant.Angle_Out_DL,  57},

		{ConnectableVariant.Angle_Skinny_UL,  58},
		{ConnectableVariant.Angle_Skinny_UR,  59},
		{ConnectableVariant.Angle_Skinny_DR,  60},
		{ConnectableVariant.Angle_Skinny_DL,  61},

		{ConnectableVariant.T_Up,  62},
		{ConnectableVariant.T_Right,  63},
		{ConnectableVariant.T_Down,  64},
		{ConnectableVariant.T_Left,  65},

		{ConnectableVariant.T_Dark1_Up,  66},
		{ConnectableVariant.T_Dark1_Right,  67},
		{ConnectableVariant.T_Dark1_Down,  68},
		{ConnectableVariant.T_Dark1_Left,  69},

		{ConnectableVariant.T_Dark2_Up,  70},
		{ConnectableVariant.T_Dark2_Right,  71},
		{ConnectableVariant.T_Dark2_Down,  72},
		{ConnectableVariant.T_Dark2_Left,  73},

		{ConnectableVariant.Up,  74},
		{ConnectableVariant.Right,  75},
		{ConnectableVariant.Down,  76},
		{ConnectableVariant.Left,  77},

		{ConnectableVariant.All_Way,  78},

		{ConnectableVariant.All_Way_Dark_UL,  79},
		{ConnectableVariant.All_Way_Dark_UR,  80},
		{ConnectableVariant.All_Way_Dark_DR,  81},
		{ConnectableVariant.All_Way_Dark_DL,  82},

		{ConnectableVariant.T_DarkTop_Up,  83},
		{ConnectableVariant.T_DarkTop_Right,  84},
		{ConnectableVariant.T_DarkTop_Down,  85},
		{ConnectableVariant.T_DarkTop_Left,  86},

		{ConnectableVariant.Diagnal_UL_DR,  87},
		{ConnectableVariant.Diagnal_UR_DL,  88},

		{ConnectableVariant.Angle_In_UL,  89},
		{ConnectableVariant.Angle_In_UR,  90},
		{ConnectableVariant.Angle_In_DR,  91},
		{ConnectableVariant.Angle_In_DL,  92},

		{ConnectableVariant.Strip_Vert,  93},
		{ConnectableVariant.Strip_Horiz,  94}

	};

	public Air (ConnectableVariant type) : base (type){}

	override
	protected Dictionary<ConnectableVariant, int> ConnectableVariantDictionary 
	{
		get { return airTypeDictionary; }
	}


	public override bool isWalkable() {
		return true;
	}

	public override int getSpriteIndex()
	{
		return spriteIndex;
	}

}
