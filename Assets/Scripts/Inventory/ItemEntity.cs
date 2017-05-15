using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemEntity : GravityObject {
	private struct ItemDataStruct
	{
		public byte[] data;
	}

	private const float PICK_UP_DISTANCE = 0.6f;
	private const float HEIGHT_CUTOFF = 0.1f;

	[SyncVar]
	private ItemDataStruct itemData = new ItemDataStruct();
	[SyncVar]
	private int quantity = 1;

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
		return Item.ReadItem(itemData.data);
	}

	public void setQuantity(int quantity) {
		if (!isServer)
			return; //quit trying to cheat!

		this.quantity = quantity;
	}

	[ClientRpc]
	public void RpcSetIdentity(byte[] itemData) {
		setIdentity(Item.ReadItem(itemData));
	}

	private void checkIfPickedUp() {
		if (!isServer)
			return;

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		foreach (GameObject player in players) {
			MoleController playerController = player.GetComponent<MoleController> ();

			if (playerController == null || playerController.gamePos == null)
				continue;

			float heightDiff = Mathf.Abs(gamePos.depth - playerController.gamePos.depth);
			float planeDiff = (gamePos.planePosition - playerController.gamePos.planePosition).magnitude;

			if (heightDiff < HEIGHT_CUTOFF && planeDiff < PICK_UP_DISTANCE) {
				Player p = player.GetComponent<Player> ();


				InventoryOperation invOp = new InventoryOperation (InventoryOperation.Operation.AddItems, new byte[][] { itemData.data, System.Text.Encoding.Default.GetBytes(quantity.ToString()) });

				int remainder = player.GetComponent<Player> ().PerformInventoryAction (invOp, "general");
				Debug.Log ("remainder: " + remainder);
				if (remainder != 0) {
					return;
				}

				if (!player.GetComponent<NetworkIdentity> ().isLocalPlayer) {
					//player.GetComponent<Player> ().beltUI.updateUI ();
				//} else {

					NetworkWriter writer = new NetworkWriter ();
					invOp.Serialize (writer);
					p.TargetPerformInventoryAction (player.GetComponent<NetworkIdentity> ().connectionToClient, writer.AsArray (), "general");
					//p.TargetPickUpItem (player.GetComponent<NetworkIdentity> ().connectionToClient, getItem().Encode ());
				}


				NetworkServer.Destroy (gameObject);

			}
		}
	}
		

	public void setIdentity(Item item) {
		if (item != null) {
			this.itemData.data = MessageUtil.ToArray(item);
			rend.sprite = item.getIcon ();
		} else {
			this.itemData.data = new byte[0];
		}
	}
		
}
