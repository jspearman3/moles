using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemEntity : GravityObject {

	private const float PICK_UP_DISTANCE = 0.3f;
	private const float HEIGHT_CUTOFF = 0.1f;

	[SyncVar]
	public string ItemIdentityCode;

	protected override void InitializeObject ()
	{
		base.InitializeObject ();
	}

	
	// Update is called once per frame
	protected override void GameUpdate () {
		base.GameUpdate();
		if (getItem() != null) {
			rend.sprite = getItem().getIcon ();
		} else {
			Debug.Log ("WTF NULL");
		}
		checkIfPickedUp ();
	}

	public Item getItem() {
		return new RockItem ().Decode (ItemIdentityCode);
	}

	[ClientRpc]
	public void RpcSetIdentity(string itemCode) {
		setIdentity(new RockItem ().Decode (itemCode));
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
				Player p = player.GetComponent<Player> ();

				int remainder = player.GetComponent<Player> ().pickUpItem (getItem ());
				Debug.Log ("remainder: " + remainder);
				if (remainder != 0) {
					return;
				}

				if (player.GetComponent<NetworkIdentity> ().isLocalPlayer) {
					player.GetComponent<Player> ().itembar.updateUI ();
				} else {
					p.TargetPickUpItem (player.GetComponent<NetworkIdentity> ().connectionToClient, getItem().Encode ());
				}


				NetworkServer.Destroy (gameObject);

			}
		}
	}
		

	public void setIdentity(Item item) {
		if (item != null) {
			this.ItemIdentityCode = item.Encode ();
			rend.sprite = item.getIcon ();
		} else {
			this.ItemIdentityCode = null;
		}
	}
		
}
