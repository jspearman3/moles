using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameEntity : NetworkBehaviour {

    [SyncVar]
	public GamePosStruct syncPos;

	public GamePosition gamePos;

    public float lerpRate = 15;
	protected TiledMap map;
    protected Transform trans;

	protected SpriteRenderer rend;

	private bool firstSync = false;

    // Use this for initialization
    void Awake()
    {
        trans = GetComponent<Transform>();
		rend = GetComponent<SpriteRenderer> ();
    }

    protected virtual void InitializeObject()
    {
		map = GameObject.FindGameObjectWithTag("WorldMap").GetComponent<TiledMap>();
		gamePos = GamePosition.ParseStruct (syncPos);
    }


    void Update()
    {
		if (!firstSync) {
			InitializeObject();
			firstSync = true;
		}


		trans.position = gamePos.getRenderingPosition ();

		if (rend != null) {
			rend.sortingOrder = (map.mapDepth - gamePos.toMapCoords().depth - 1) * 10 + 5;
		}

		if (!isLocalPlayer) {

			GamePosition syncGamePos = GamePosition.ParseStruct (syncPos);
			gamePos.planePosition = Vector2.Lerp (gamePos.planePosition, syncGamePos.planePosition, Time.deltaTime * lerpRate);
			gamePos.depth = syncGamePos.depth;

			if (rend.sortingOrder > MoleController.localPlayer.GetComponent<MoleController> ().rend.sortingOrder) {
				rend.color = new Color (1, 1, 1, 0);
			} else {
				rend.color = new Color (1, 1, 1, 1);
			}
				

		} else {
			CmdTransmitPosition(gamePos.toStruct());
		}

        GameUpdate();
    }

    protected virtual void GameUpdate() { }


    [Command]
    void CmdTransmitPosition(GamePosStruct pos)
    {
		syncPos = pos;
	}

}
