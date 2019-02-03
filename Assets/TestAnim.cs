using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestAnim : NetworkBehaviour {
	private Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;
		Vector2 mousePos = (Input.mousePosition - new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0)).normalized;
		anim.SetFloat ("x", mousePos.x);
		anim.SetFloat ("y", mousePos.y);

		float vert = Input.GetAxisRaw ("Vertical");
		float horiz = Input.GetAxisRaw ("Horizontal");

		if (vert == 0 && horiz == 0) {
			anim.SetBool ("walking", false);
		} else {
			anim.SetBool ("walking", true);
		}

		anim.SetFloat ("vert", vert);
		anim.SetFloat ("horiz", horiz);


		if (Input.GetKey (KeyCode.Mouse0)) {
			anim.SetTrigger ("leftswipe");
		} else {
			anim.ResetTrigger ("leftswipe");
		}

		if (Input.GetKey (KeyCode.Mouse1)) {
			anim.SetTrigger ("rightswipe");
		} else {
			anim.ResetTrigger ("rightswipe");
		}

	}


}
