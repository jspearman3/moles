using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GameEntity : NetworkBehaviour {
    [SyncVar]
    private Vector3 syncPos;

    public float lerpRate = 15;

    protected Transform trans;

    // Use this for initialization
    void Awake()
    {
        trans = GetComponent<Transform>();
        InitializeObject();
    }

    protected virtual void InitializeObject()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {

            trans.position = Vector3.Lerp(trans.position, syncPos, Time.deltaTime * lerpRate);
            return;
        }

        CmdTransmitPosition(trans.position);

        GameUpdate();
    }

    protected virtual void GameUpdate() { }

    protected Vector2 get2DPos()
    {
        return new Vector2(trans.position.x, trans.position.y);
    }

    [Command]
    void CmdTransmitPosition(Vector3 pos)
    {
        syncPos = pos;
    }

}
