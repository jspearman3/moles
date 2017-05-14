using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
	public const float DROP_DISTANCE = 1f;


	public PlayerInfo info;
	public string serverdebugtext;
	public string serverdebugtext2;
	NetworkIdentity identity;
	PlayerInfoManager playerInfoManager;
	public BeltInventoryUI beltUI;
	public BackpackInventoryUI backpackUI;
	public EquipMenuUI equipUI;
	public MoleController controller;

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

			beltUI = GameObject.FindGameObjectWithTag ("ItemBar").GetComponent<BeltInventoryUI> ();
			beltUI.loadInventory (info.belt);


			backpackUI = GameObject.FindGameObjectWithTag ("BackpackUI").GetComponent<BackpackInventoryUI> ();
			equipUI = GameObject.FindGameObjectWithTag ("EquipUI").GetComponent<EquipMenuUI> ();
			if (info.backpack != null)
				backpackUI.loadInventory (info.backpack.inventory);

			equipUI.updateUI (info);

			phase2Init = true;
		}

		if (Input.GetKeyDown (KeyCode.F)) {
			int slotNum = beltUI.getSelectedSlotNumber ();
			Item item = info.belt.getSlots () [slotNum].getItem ();
			if (item != null) {
				CmdUseItem (slotNum, MessageUtil.ToArray(item));
			}
		}
			
	}

	[Command]
	public void CmdRequestInventoryActions(byte[] operationInfo) {
		NetworkReader reader = new NetworkReader (operationInfo);
		InventoryOperationRequest request = new InventoryOperationRequest (InventoryOperationRequest.Operation.FromCursor, InventoryOperationRequest.InventoryType.Ground, 0);
		request.Deserialize (reader);

		Debug.Log ("recieved inventory request\nop:" + request.op + "\nquant: " + request.quantity + "\nswapinv: " + request.swapInv + "\nswapslot" + request.swapInvSlot);

		if (InventoryOperationRequest.validateRequest (request, info)) {
			ItemInventorySlot dropped = InventoryOperationRequest.performInventoryRequest (request, this);
			SpawnManager spawnManager = GameObject.FindGameObjectWithTag ("SpawnManager").GetComponent<SpawnManager> ();
			Debug.Log ("player dropping " + dropped.getQuantity() + " " + dropped.getItem () + " items");
			spawnManager.SpawnPlayerDroppedItem(dropped.getItem(), dropped.getQuantity(), controller);

		} else {
			Debug.Log ("recieved bad inventory request! client may be out of sync");
		}
	}


	[TargetRpc]
	public void TargetPerformInventoryAction(NetworkConnection conn, byte[] operationInfo, string actionType) {
		NetworkReader reader = new NetworkReader (operationInfo);
		InventoryOperation op = new InventoryOperation(InventoryOperation.Operation.Clear, null);
		op.Deserialize (reader);
		PerformInventoryAction (op, actionType);
	}


	public int PerformInventoryAction(InventoryOperation op, string actionType) {
		if (actionType.ToLower ().Equals ("backpackonly")) {
			int result = InventoryOperation.PerformOperation (info.backpack.inventory, op);
			if (isLocalPlayer)
				backpackUI.updateUI ();

			return result;
		} else if (actionType.ToLower ().Equals ("beltonly")) {
			int result = InventoryOperation.PerformOperation (info.belt, op);
			if (isLocalPlayer)
				beltUI.updateUI ();

			return result;
		} else if (actionType.ToLower ().Equals ("general")) {
			int result = InventoryOperation.PerformOperation (info.belt, op);
			if (result < 0) {
				Debug.LogError ("Error adding to belt");
				return -1;
			}

			if (isLocalPlayer)
				beltUI.updateUI ();

			if (op.op.Equals (InventoryOperation.Operation.AddItems) && info.backpack != null && result > 0) {
				Item item = Item.ReadItem(op.paramaters [0]);
				int result2 = info.backpack.inventory.addItemMany (item, result);
				if (isLocalPlayer)
					backpackUI.updateUI ();

				return result2;
			}
			return result;
		} else {
			Debug.LogError ("Recieved unrecognized inventory action type '" + actionType + "'");
			return -1;
		}
	}



	[Command]
	public void CmdUseItem(int slotNum, byte[] itemData) {
		ItemInventorySlot s = info.belt.getSlots ()[slotNum];
		Item item = Item.ReadItem (itemData);
		if (s.getItem ().IsSameType (item)) {
			if (s.getItem () != null) {

				if (s.getItem ().use (this)) {
					s.removeOne ();
				}


				if (!isLocalPlayer) {
					NetworkIdentity iden = GetComponent<NetworkIdentity> ();
					TargetUseItem (iden.connectionToClient, slotNum);
				} else {
					beltUI.updateUI();
				}
			}
		} else {
			Debug.LogError ("Item sync issue. Client had item " + item.ToString() + " while we had " + s.getItem().ToString());
		}
	}

	[TargetRpc]
	public void TargetUseItem(NetworkConnection conn, int slotNum) {
		ItemInventorySlot s = beltUI.getInventoryBacking ().getSlots ()[slotNum];
		if (s.getItem() != null && s.getItem ().use (this)) {
			s.removeOne ();
		}
		beltUI.updateUI ();
	}

	[TargetRpc]
	public void TargetPerformEquipAction(NetworkConnection conn, byte[] operationInfo) {
		NetworkReader reader = new NetworkReader (operationInfo);
		EquipmentOperation op = new EquipmentOperation(EquipmentOperation.Operation.SetHelmet, null);
		op.Deserialize (reader);
		PerformEquipAction (op);
	}

	public bool PerformEquipAction(EquipmentOperation op) {
		if (isLocalPlayer) {
			bool success = EquipmentOperation.PerformOperationWithBackpackUIUpdate (this, op, backpackUI);
			if (success)
				equipUI.updateUI (info);
			return success;
		} else {
			return EquipmentOperation.PerformOperation(this, op);
		}
	}

	public bool EquipItem(EquipmentItem equip, bool rightSide) {

		if (equip is BackpackItem) {
			BackpackItem backpack = equip as BackpackItem;
			info.backpack = backpack;

			if (isLocalPlayer) {
				backpackUI.loadInventory (backpack.inventory);
				equipUI.setBackpack (equip as BackpackItem);
			}
				

			return true;
		} else if (equip is HelmetItem) {
			info.helmet = equip as HelmetItem;

			if (isLocalPlayer)
				equipUI.setHelmet (equip as HelmetItem);

			return true;
		} else if (equip is UpperBodyItem) {
			info.upperBody = equip as UpperBodyItem;

			if (isLocalPlayer)
				equipUI.setUpperBody (equip as UpperBodyItem);
			
			return true;
		} else if (equip is LowerBodyItem) {
			info.lowerBody = equip as LowerBodyItem;

			if (isLocalPlayer)
				equipUI.setLowerBody (equip as LowerBodyItem);
			
			return true;
		} else if (equip is BootsItem) {
			info.boots = equip as BootsItem;

			if (isLocalPlayer)
				equipUI.setBoots (equip as BootsItem);
			
			return true;
		} else if (equip is ClawItem) {
			if (rightSide) {
				info.rightClaw = equip as ClawItem;

				if (isLocalPlayer)
					equipUI.setRightClaw (equip as ClawItem);		
			} else {
				info.leftClaw = equip as ClawItem;

				if (isLocalPlayer)
					equipUI.setLeftClaw (equip as ClawItem);
			}
			return true;
		} 

		return false;
	}

	public void EquipItem(EquipmentItem equip) {
		EquipItem (equip, false);
	}
		
}
