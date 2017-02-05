using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MoleController : Walker {
	public float speed = 1;
    public float digRange = 0.5f;

	Animator anim;
	Rigidbody2D rigid;
    Camera mainCamera;

    Direction facing;
	string idleAnimation;

	// Use this for initialization
	void Start () {
		idleAnimation = "idle_down";
        facing = Direction.South;
		anim = GetComponent<Animator> ();
		rigid = GetComponent<Rigidbody2D> ();
        mainCamera = Camera.main;
	}
	
	// Update is called once per frame
	protected override void GameUpdate () {
        movement ();
        actions();
        updateCamera();
    }

    private void updateCamera()
    {
        mainCamera.GetComponent<Transform>().position = new Vector3(trans.position.x, trans.position.y, 
            -1);
    }

    private void actions()
    {
        if (Input.GetKeyDown("e"))
        {
            //begin playing some animation and cooldown then:

            tryDig();
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

        Vector2 digSpot = get2DPos() + digOffset;

		CmdDig (digSpot);

    }

	[Command]
	private void CmdDig(Vector2 digSpot) {
		map.digFromWorldSpace (digSpot);
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
