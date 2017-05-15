using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class InventoryUIManager : MonoBehaviour {

	public BackpackInventoryUI backpackUI;
	public EquipMenuUI equipUI;
	public BeltInventoryUI beltUI;

	public ItemInventorySlotUI cursorSlot;

	bool opened = false;

	// Use this for initialization
	void Start () {
		setUIOpened ();
	}

	// Update is called once per frame
	void Update () {

		cursorSlot.gameObject.GetComponent<RectTransform> ().position = Input.mousePosition;

		if (Input.GetKeyDown (KeyCode.Space))
			toggleUIs ();


		if (Input.GetKeyDown (KeyCode.Mouse0))
			handleMouseClick (0);

		if (Input.GetKeyDown (KeyCode.Mouse1))
			handleMouseClick (1);

	}

	bool init = false;
	Player localPlayer;
	void handleMouseClick(int quantity) {

		if (!init) {
			if (MoleController.localPlayer.GetComponent<Player> ().info.cursorSlot == null) 
				return;
			localPlayer = MoleController.localPlayer.GetComponent<Player> ();
			cursorSlot.setSlotBackingInfo (MoleController.localPlayer.GetComponent<Player> ().info.cursorSlot);
			init = true;
		}



		PointerEventData pointerData = new PointerEventData (EventSystem.current);
		pointerData.position = Input.mousePosition;

		cursorSlot.setSlotBackingInfo (localPlayer.info.cursorSlot);

		List<RaycastResult> results = new List<RaycastResult> ();
		EventSystem.current.RaycastAll (pointerData, results);

		foreach (RaycastResult r in results) {
			if (r.gameObject.name.Equals ("SlotUIBackground")) {
				ItemInventorySlotUI slotUI = r.gameObject.GetComponentInParent<ItemInventorySlotUI>();
				if (slotUI != null) {
					string[] splitName = slotUI.gameObject.name.Split (' ');
					string menuIdentifier = splitName [0];
					string slotIdentifier = splitName [1];


					//pickup half stack if right click
					if (cursorSlot.getSlotBackingInfo ().isEmpty () && quantity == 1) {
						quantity = Mathf.CeilToInt (slotUI.getSlotBackingInfo ().getQuantity () / 2.0f);
					}

					handleSlotSelect (slotUI, menuIdentifier, slotIdentifier, quantity);
					return;
				}
			}
			return; //If clicking on a UI component at all, don't drop item. Must explicitly be dropping on game area
		}
		handleSlotSelect (null, "", "0", quantity); //drop items in cursor
	}

		void handleSlotSelect(ItemInventorySlotUI slotUI, string menuIdentifier, string slotIdentifier, int quantity) {

		InventoryOperationRequest.InventoryType menuType = InventoryOperationRequest.InventoryType.Ground;
		InventoryOperationRequest req = null;

		if (menuIdentifier.Equals("BeltSlot")) {
			menuType = InventoryOperationRequest.InventoryType.Belt;
		} else if (menuIdentifier.Equals("BackpackSlot")) {
			menuType = InventoryOperationRequest.InventoryType.Backpack;
		} else if (menuIdentifier.Equals("EquipSlot")) {
			menuType = InventoryOperationRequest.InventoryType.Equip;
		}


		if (slotUI == null) { //THREW IT ON THE GROUND
			req = new InventoryOperationRequest (InventoryOperationRequest.Operation.FromCursor,  InventoryOperationRequest.InventoryType.Ground, System.Int32.Parse (slotIdentifier), quantity);
		} else if (cursorSlot.getSlotBackingInfo ().isEmpty ()) {
			req = new InventoryOperationRequest (InventoryOperationRequest.Operation.ToCursor, menuType, System.Int32.Parse (slotIdentifier), quantity);
		} else {
			req = new InventoryOperationRequest (InventoryOperationRequest.Operation.FromCursor, menuType, System.Int32.Parse (slotIdentifier), quantity);
		}

		//perform local changes before sending info to server. Annoying to wait on server for ui change. If sync error, server should let us know eventually and fix
		if (InventoryOperationRequest.validateRequest (req, localPlayer.info)) { //perform check server will do to verify before performing locally
			ItemInventorySlot dropped = InventoryOperationRequest.performInventoryRequest (req, localPlayer);

			if (!localPlayer.isServer) {
				sendInventoryUpdateToServer (req);
			} else {
				//host drops items
				if (!dropped.isEmpty()) {
					SpawnManager spawnManager = GameObject.FindGameObjectWithTag("SpawnManager").GetComponent<SpawnManager>();
					spawnManager.SpawnPlayerDroppedItem(dropped.getItem(), dropped.getQuantity(), localPlayer.controller);
				}
			}
				
			cursorSlot.updateUI ();

			switch (menuType) {
			case InventoryOperationRequest.InventoryType.Belt:
				beltUI.updateUI ();
				break;
			case InventoryOperationRequest.InventoryType.Backpack:
				backpackUI.updateUI ();					
				break;
			case InventoryOperationRequest.InventoryType.Equip:
				if (slotIdentifier.Equals ("6")) {
					if (localPlayer.info.backpack == null) {
						backpackUI.loadInventory (null);
					} else {
						backpackUI.loadInventory (localPlayer.info.backpack.inventory);
					}
				}
				equipUI.updateUI (localPlayer.info);
				break;
			}
		}
	}

	private void sendInventoryUpdateToServer(InventoryOperationRequest request) {
		Debug.Log ("sending inventory request\nop:" + request.op + "\nquant: " + request.quantity + "\nswapinv: " + request.swapInv + "\nswapslot: " + request.swapInvSlot);
		NetworkWriter writer = new NetworkWriter ();
		request.Serialize (writer);
		localPlayer.GetComponent<Player> ().CmdRequestInventoryActions (writer.AsArray ());
	}

	void toggleUIs() {
		opened = !opened;
		setUIOpened ();
	}

	void setUIOpened() {
		backpackUI.opened = opened;
		equipUI.opened = opened;
	}


}
