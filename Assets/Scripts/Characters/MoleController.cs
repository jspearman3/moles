using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MoleController : Walker {
	public static GameObject localPlayer;

	public float speed = 1;
    public float digRange = 0.5f;

	Animator anim;
	Rigidbody2D rigid;

    Direction facing;
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
        movement ();
        actions();
    }

    private void actions()
    {
        if (Input.GetKeyDown("e"))
        {
            //begin playing some animation and cooldown then:

            tryDig();
        }
			
		if (Input.GetKeyDown ("r")) {
			if (gamePos.depth > 0) {
				gamePos.depth -= 1;
				CmdDig (gamePos.toStruct());
			}

		}

		if (Input.GetKeyDown ("f")) {
			if (gamePos.depth < map.mapDepth - 1) {
				gamePos.depth += 1;
				CmdDig (gamePos.toStruct());
			}
		}
    }


    private void tryDig()
    {
        Vector2 digOffset;
        switch (facing)
        {
            case Direction.North:
                digOffset = new Vector2(0, digRange);
                break;
            case Direction.South:
                digOffset = new Vector2(0, -digRange);
                break;
            case Direction.East:
                digOffset = new Vector2(digRange, 0);
                break;
            case Direction.West:
                digOffset = new Vector2(-digRange, 0);
                break;
            default:
                return;
        }

		GamePosition digSpot = gamePos.addWorld(digOffset);

		CmdDig (digSpot.toStruct());

    }

	[Command]
	private void CmdDig(GamePosStruct digSpot) {
		map.dig (GamePosition.ParseStruct(digSpot));
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

		return newAnim;



	}
		
}
