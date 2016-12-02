using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MoleController : NetworkBehaviour {
	[SyncVar]
	private Vector3 syncPos;

	public float speed = 1;

	public float lerpRate = 15;

	Animator anim;
	Transform trans;
	Rigidbody2D rigid;

	string idleAnimation;

	// Use this for initialization
	void Start () {
		idleAnimation = "idle_down";
		anim = GetComponent<Animator> ();
		trans = GetComponent<Transform> (); 
		rigid = GetComponent<Rigidbody2D> ();

	}
	
	// Update is called once per frame
	void Update () {
		
		if (!isLocalPlayer) {

			Debug.Log ("first: " + trans.position);

			trans.position = Vector3.Lerp (trans.position, syncPos, Time.deltaTime * lerpRate);

			Debug.Log ("second: " + trans.position);
			Debug.Log ("syncpos: " + syncPos);
			return;
		}

		CmdTransmitPosition (trans.position);
		move ();
	}

	[Command]
	void CmdTransmitPosition (Vector3 pos) {
		syncPos = pos;
	}


	private void move() {

		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");

		trans.Translate(new Vector3 (h, v).normalized * speed * Time.deltaTime);

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
			newAnim = "walk_up";
		} else if (v < 0) {
			idleAnimation = "idle_down";
			newAnim = "walk_down";
		}

		if (h > 0) {
			idleAnimation = "idle_right";
			newAnim = "walk_right";
		} else if (h < 0) {
			idleAnimation = "idle_left";
			newAnim = "walk_left";
		}

		return newAnim;



	}
		
}
