using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MoleController : Walker {
	public static GameObject localPlayer;

	public const float WALKING_SPEED = 2f;
	public const float SPRINTING_SPEED = 3f;

	public float speed = WALKING_SPEED;
    public float digRange = 0.5f;

	Rigidbody2D rigid;

    public Direction facing;

	// Use this for initialization
	void Start () {
		if (isLocalPlayer) {
			localPlayer = gameObject;
		}
			
        facing = Direction.South;
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
		if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1))
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

	private float getCursorAngle() {
		Vector2 mousePos = (Input.mousePosition - new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0)).normalized;
		Vector3 cross = Vector3.Cross (mousePos, Vector2.right);
		float angle = Vector2.Angle (mousePos, Vector2.right);
		if (cross.z > 0)
			angle = 360 - angle;
		return angle;
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

		if (Input.GetKey (KeyCode.LeftShift)) {
			speed = SPRINTING_SPEED;
		} else {
			speed = WALKING_SPEED;
		}

		Move(new Vector2 (h, v).normalized * speed * Time.deltaTime);

		setFacing();
	}

	private void setFacing() {
		float cursorAngle = getCursorAngle ();
		facing = Direction.South;
		if (cursorAngle > 315 || cursorAngle <= 45) {
			facing = Direction.East;
		} else if (cursorAngle > 45 && cursorAngle <= 135) {
			facing = Direction.North;
		} else if (cursorAngle > 135 && cursorAngle <= 225) {
			facing = Direction.West;
		} else if (cursorAngle > 225 && cursorAngle <= 315) {
			facing = Direction.South;
		}
	
		CmdSetDirection (facing);
	}

	[Command]
	private void CmdSetDirection(Direction d) {
		facing = d;
	}
		
}
