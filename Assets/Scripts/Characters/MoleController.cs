using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MoleController : Walker {
	public static GameObject localPlayer;

	public float speed = 1;
    public float digRange = 0.5f;

	Animator anim;
	Rigidbody2D rigid;

    public Direction facing;

	string idleAnimation;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			localPlayer = gameObject;
		}

		idleAnimation = "idle_down";
        facing = Direction.South;
		anim = GetComponent<Animator> ();
		rigid = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
		if (!isLocalPlayer)
			return;

		base.GameUpdate ();
        movement ();
        actions();
    }

    private void actions()
    {
        if (Input.GetKeyDown("e"))
        {

            tryDig();
        }

		float ascendSensitivity = 1;
			
		if (Input.GetKeyDown ("r")) {
			tryPlaceDirt ();
		}

		if (Input.GetKeyDown ("p")) {
			runtest ();
		}
    }

	private void runtest() {
		LeatherBackpackItem i = new LeatherBackpackItem ();
		MessageUtil.ToArray (i);
		Item item = Item.ReadItem(MessageUtil.ToArray (i));
		Debug.Log ("item: " + item);
	}


    private void tryDig()
    {
		Vector2 digOffset = getActionOffset ();

		GamePosition digSpot = gamePos.addWorld(digOffset);

		CmdDig (digSpot.toStruct());

    }

	private void tryPlaceDirt()
	{
		Vector2 placeOffset = getActionOffset ();

		GamePosition placeSpot = gamePos.addWorld(placeOffset);

		CmdPlace (placeSpot.toStruct());
	}

	private Vector2 getActionOffset() {
		Vector2 actionOffset = Vector2.zero;
		switch (facing)
		{
		case Direction.North:
			actionOffset = new Vector2(0, digRange);
			break;
		case Direction.South:
			actionOffset = new Vector2(0, -digRange);
			break;
		case Direction.East:
			actionOffset = new Vector2(digRange, 0);
			break;
		case Direction.West:
			actionOffset = new Vector2(-digRange, 0);
			break;
		default:
			return actionOffset;
		}
		return actionOffset;
	}

	[Command]
	private void CmdDig(GamePosStruct digSpot) {
		map.dig (GamePosition.ParseStruct(digSpot));
	}

	[Command]
	private void CmdPlace(GamePosStruct placeSpot) {
		map.place (GamePosition.ParseStruct(placeSpot));
	}


	private void movement() {

		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		Move(new Vector2 (h, v).normalized * speed * Time.deltaTime);

		string animation = selectAnimation (h, v);
		anim.Play (animation, 0);
	}

	private string selectAnimation(float h, float v) {

		if (h == 0 && v == 0) {
			return idleAnimation;
		}

		string newAnim = null;
		if (v > 0) {
			idleAnimation = "idle_up";
            facing = Direction.North;
			newAnim = "walk_up";
		} else if (v < 0) {
			idleAnimation = "idle_down";
            facing = Direction.South;
            newAnim = "walk_down";
		}

		if (h > 0) {
			idleAnimation = "idle_right";
            facing = Direction.East;
            newAnim = "walk_right";
		} else if (h < 0) {
			idleAnimation = "idle_left";
            facing = Direction.West;
            newAnim = "walk_left";
		}
		CmdSetDirection (facing);
		return newAnim;

	}

	[Command]
	private void CmdSetDirection(Direction d) {
		facing = d;
	}
		
}
