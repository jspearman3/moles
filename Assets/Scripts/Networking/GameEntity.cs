using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameEntity : NetworkBehaviour {

    [SyncVar]
	public GamePosStruct syncPos;

	public GamePosition gamePos = new GamePosition (new Vector2 (3.5f, 3.5f), 1.5f);

    public float lerpRate = 15;
	protected TiledMap map;
    protected Transform trans;

	protected SpriteRenderer rend;

    // Use this for initialization
    void Awake()
    {
        trans = GetComponent<Transform>();
		rend = GetComponent<SpriteRenderer> ();
        InitializeObject();
    }

    protected virtual void InitializeObject()
    {

    }


    void Update()
    {
		trans.position = gamePos.getRenderingPosition ();

		if (rend != null) {
			rend.sortingOrder = map.mapDepth - gamePos.toMapCoords().depth - 1;
		}

        if (!isLocalPlayer)
        {

			GamePosition syncGamePos = GamePosition.ParseStruct (syncPos);
			gamePos.planePosition = Vector2.Lerp(gamePos.planePosition, syncGamePos.planePosition, Time.deltaTime * lerpRate);
			gamePos.depth = syncGamePos.depth;

			if (rend.sortingOrder > MoleController.localPlayer.GetComponent<MoleController> ().rend.sortingOrder) {
				rend.color = new Color (1, 1, 1, 0);
			} else {
				rend.color = new Color (1, 1, 1, 1);
			}


            return;
        }

		CmdTransmitPosition(gamePos.toStruct());

        GameUpdate();
    }

    protected virtual void GameUpdate() { }


    [Command]
    void CmdTransmitPosition(GamePosStruct pos)
    {
		syncPos = pos;
	}

}
