using UnityEngine;
using System.Collections.Generic;

public class Wall : Tile {
    private static Dictionary<WallType, int> wallTypeDictionary = new Dictionary<WallType, int>()
    {
        {WallType.Up,  1},
        {WallType.Right, 8},
        {WallType.Down, 13},
        {WallType.Left, 6},

        {WallType.UpLeft, 0},
        {WallType.UpRight, 2},
        {WallType.DownLeft, 12},
        {WallType.DownRight, 14},

        {WallType.UpLeft_Inv, 3},
        {WallType.UpRight_Inv, 5},
        {WallType.DownLeft_Inv, 15},
        {WallType.DownRight_Inv, 17}
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
