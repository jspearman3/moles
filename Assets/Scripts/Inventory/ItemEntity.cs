using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemEntity : GravityObject {

	private const float PICK_UP_DISTANCE = 0.3f;
	private const float HEIGHT_CUTOFF = 0.1f;

	[SyncVar]
	public Item identity;

	protected override void InitializeObject ()
	{
		base.InitializeObject ();
	}

	
	// Update is called once per frame
	protected override void GameUpdate () {
		base.GameUpdate();
		if (identity != null) {
			rend.sprite = identity.getIcon ();
		} else {
			Debug.Log ("WTF NULL");
		}
		checkIfPickedUp ();
	}

	private void checkIfPickedUp() {
		if (!isServer)
			return;

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		foreach (GameObject player in players) {
			MoleController playerController = player.GetComponent<MoleController> ();
			float heightDiff = Mathf.Abs(gamePos.depth - playerController.gamePos.depth);
			float planeDiff = (gamePos.planePosition - playerController.gamePos.planePosition).magnitude;

			if (heightDiff < HEIGHT_CUTOFF && planeDiff < PICK_UP_DISTANCE) {
				playerController.RpcPickUpItem (identity.Encode());

				NetworkServer.Destroy (gameObject);
			}
		}
	}
		

	public void setIdentity(Item item) {
		this.identity = item;
		if (item != null) {
			rend.sprite = item.getIcon ();
		}
	}
		
}
