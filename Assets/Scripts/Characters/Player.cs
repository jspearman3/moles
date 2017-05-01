using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public PlayerInfo info;
	public string serverdebugtext;
	public string serverdebugtext2;
	NetworkIdentity identity;
	PlayerInfoManager playerInfoManager;
	public BeltInventoryUI itembar;
	public BackpackInventoryUI backpackUI;

	// Use this for initialization
	void Start () {
		identity = GetComponent<NetworkIdentity> ();
	}

	[TargetRpc]
	public void TargetSetPlayerInfo(NetworkConnection conn, byte[] serverInfo) {
		NetworkReader reader = new NetworkReader (serverInfo);
		PlayerInfo newInfo = new PlayerInfo (null);
		newInfo.Deserialize (reader);
		this.info = newInfo;
		serverdebugtext2 = serverInfo + "poop"; 
	}

	[Command]
	void CmdFetchPlayerInfo() {
		PlayerInfoManager pim = GameObject.Find ("PlayerInfoManager").GetComponent<PlayerInfoManager>();
		PlayerInfo serverInfo = pim.getPlayerInfo(identity.connectionToClient);

		NetworkWriter writer = new NetworkWriter ();
		serverInfo.Serialize (writer);

		TargetSetPlayerInfo (identity.connectionToClient, writer.AsArray());
	}

	bool phase1Init = false;
	bool phase2Init = false;

	void Update() {
		if (!isLocalPlayer)
			return;

		if (!phase1Init) {
			CmdFetchPlayerInfo ();
			phase1Init = true;
			return;
		}

		if (!phase2Init) {
			if (info == null) {
				return;
			}

			itembar = GameObject.FindGameObjectWithTag ("ItemBar").GetComponent<BeltInventoryUI> ();
			itembar.loadInventory (info.belt);


			backpackUI = GameObject.FindGameObjectWithTag ("BackpackUI").GetComponent<BackpackInventoryUI> ();
			if (info.backpack != null)
				backpackUI.loadInventory (info.backpack.inventory);

			phase2Init = true;
		}

		if (Input.GetKeyDown (KeyCode.F)) {
			int slotNum = itembar.getSelectedSlotNumber ();
			Item item = info.belt.getSlots () [slotNum].getItem ();
			if (item != null) {
				CmdUseItem (slotNum, item.Encode ());
			}
		}
			
	}
	
	[TargetRpc]
	public void TargetPickUpItem(NetworkConnection conn, string itemCode) {
		Item dummy = new RockItem();
		pickUpItem (dummy.Decode (itemCode));
	}

	public int pickUpItem(Item item) {
		int remainder = info.belt.addItem (item);
		if (isLocalPlayer)
			itembar.updateUI ();
		if (remainder > 0) {
			remainder = info.backpack.inventory.addItem (item);
			if (isLocalPlayer)
				backpackUI.updateUI ();
		}
		return remainder;
	}

	[Command]
	public void CmdUseItem(int slotNum, string itemcode) {
		ItemInventorySlot s = info.belt.getSlots ()[slotNum];
		Item item = new RockItem ().Decode (itemcode);
		if (s.getItem ().IsSameType (item)) {
			if (s.getItem () != null) {

				if (s.getItem ().use (this)) {
					s.removeOne ();
				}


				if (!isLocalPlayer) {
					NetworkIdentity iden = GetComponent<NetworkIdentity> ();
					TargetUseItem (iden.connectionToClient, slotNum);
				} else {
					itembar.updateUI();
				}
			}
		} else {
			Debug.LogError ("Client had item " + item.ToString() + " while we had " + s.getItem().ToString());
		}
	}

	[TargetRpc]
	public void TargetUseItem(NetworkConnection conn, int slotNum) {
		ItemInventorySlot s = itembar.getInventoryBacking ().getSlots ()[slotNum];
		if (s.getItem() != null && s.getItem ().use (this)) {
			s.removeOne ();
		}
		itembar.updateUI ();
	}

	public void EquipBackpack(BackpackItem backpack) {
		info.backpack = backpack;

		if (isLocalPlayer)
			backpackUI.loadInventory (backpack.inventory);
	}
}
