using UnityEngine;
using System.Collections;

public abstract class Walker : GravityObject {

    protected float width = 0;
    protected float height = 0;

	private bool onLadder = false;

    protected override void InitializeObject()
    {
		base.InitializeObject ();
    }

    protected void Move(Vector2 translation)
    {

		Tile destTile = map.getTileFromGamePosition(gamePos.addWorld(translation));
		Tile currTile = map.getTileFromGamePosition(gamePos);
		Tile tilebelow = map.getTileFromGamePosition (gamePos.add (new Vector3 (0, 0, -1)));
		if (currTile.GetType ().Equals (typeof(Ladder))) {

			float startDepth = gamePos.depth;
			gamePos.depth -= translation.y;

			float diff = gamePos.toMapCoords ().depth - gamePos.depth;

			if (diff > 0 && diff < .9) {
				translation.y = 0;
			}

			onLadder = true;
			if (gamePos.depth > Mathf.Ceil (startDepth)) {
				gamePos.depth = startDepth;
			}
		} else if (tilebelow != null && tilebelow.GetType ().Equals (typeof(Ladder))) {
			float diff = gamePos.toMapCoords ().depth - gamePos.depth;

			if (translation.y < 0 && diff < 0.1) {
				gamePos.depth -= translation.y;
				if (diff > 0 && diff < .9) {
					translation.y = 0;
				}
				onLadder = true;
			}
			onLadder = false;

		} else {
			onLadder = false;
		}

		if (destTile.isWalkable())
		{
			gamePos.planeTranslateWorld(translation);
		}


    }

	override
	protected void GameUpdate() { 
		base.GameUpdate ();

		if (onLadder) {
			gravityEnabled = false;
		} else {
			gravityEnabled = true;
		}


	
	}


}
