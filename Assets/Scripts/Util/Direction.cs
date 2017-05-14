using UnityEngine;
using System.Collections;

public enum Direction {
    North,
    South,
    East,
    West
}

public class DirectionUtil {
	public static Vector2 getDirectionUnitVector(Direction d) {
		switch (d) {
		case Direction.North:
			return new Vector2 (0, -1);
		case Direction.East:
			return new Vector2 (1, 0);
		case Direction.South:
			return new Vector2 (0, 1);
		case Direction.West:
			return new Vector2 (-1, 0);
		default:
			return new Vector2 (0, 0);
		}
	}
}
