using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is needed for sending position data across network
public struct GamePosStruct
{
	public float x;
	public float y;
	public float depth;
}

public class GamePosition {
	

	public Vector2 planePosition;
	public float depth;

	public GamePosition(Vector2 planePosition, float depth) {
		this.planePosition = planePosition;
		this.depth = depth;
	}

	//every level is 1 square offset from adjacent ones
	public Vector2 getRenderingPosition() {
		Vector2 worldPos = new Vector2 (planePosition.x, -planePosition.y);
		return (worldPos + TiledMap.LEVEL_OFFSET * new Vector2 (0, -depth));
	}

	public void descend(float amt) {
		depth += amt;
	}

	public void planeTranslate(Vector2 translation) {
		planePosition += translation;
	}

	public void planeTranslateWorld(Vector2 translation) {
		planePosition += new Vector2(translation.x, -translation.y);
	}

	public GamePosition add(Vector2 v) {
		return add(new Vector3(v.x, v.y, 0));
	}

	public GamePosition addWorld(Vector2 v) {
		return add (new Vector2 (v.x, -v.y));
	}

	public GamePosition add(Vector3 v) {
		Vector2 newPlanePos = planePosition + new Vector2 (v.x, v.y); 

		return new GamePosition (newPlanePos, depth - v.z);
	}

	public MapCoords toMapCoords() {
		int x = (int)Mathf.Floor(planePosition.x);
		int y = (int)Mathf.Floor(planePosition.y);
		int newDepth = (int)Mathf.Ceil(depth);

		return new MapCoords(x, y, newDepth);
	}

	public GamePosStruct toStruct() {
		GamePosStruct gpStruct = new GamePosStruct();
		gpStruct.x = planePosition.x;
		gpStruct.y = planePosition.y;
		gpStruct.depth = depth;

		return gpStruct;
	}

	public static GamePosition ParseStruct(GamePosStruct gpStruct) {
		return new GamePosition (new Vector2 (gpStruct.x, gpStruct.y), gpStruct.depth);
	}
}
