using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BeltInventoryUI : MonoBehaviour {

	int position = 0;
	float dist_between_slots = 0;
	float startingX = 0;
	public bool inputEnable = true;

	public Transform selecticle;

	private ItemInventorySlotUI[] slots;

	public GameObject inventorySlotPrefab;

	private Inventory inventory;

	void Start() {
		loadInventory (new Inventory (10));
	}

	void Update () {
		if (inputEnable)
			handleInput();
		float targetXPos = startingX + dist_between_slots * position;

		selecticle.localPosition = Vector2.Lerp (selecticle.localPosition, new Vector2 (targetXPos, selecticle.localPosition.y), .2f);

	}

	private void handleInput() {
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			position = 0;
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			position = 1;
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			position = 2;
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			position = 3;
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			position = 4;
		} else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			position = 5;
		} else if (Input.GetKeyDown (KeyCode.Alpha7)) {
			position = 6;
		} else if (Input.GetKeyDown (KeyCode.Alpha8)) {
			position = 7;
		} else if (Input.GetKeyDown (KeyCode.Alpha9)) {
			position = 8;
		} else if (Input.GetKeyDown (KeyCode.Alpha0)) {
			position = 9;
		}

	}
		


	public void loadInventory(Inventory inventory) {
		if (inventory == null)
			return;
		destroySlotUIs ();
		this.inventory = inventory;

		RectTransform r = GetComponent<RectTransform> ();
		ItemInventorySlot[] slotBackings = inventory.getSlots ();
		dist_between_slots = r.rect.width / slotBackings.Length;
		startingX = r.rect.xMin + dist_between_slots / 2;

		slots = new ItemInventorySlotUI [slotBackings.Length];
		for (int i = 0; i < slotBackings.Length; i++) {
			GameObject iconObj = GameObject.Instantiate (inventorySlotPrefab, GetComponent<Transform> ());
			iconObj.name = "BeltSlot " + i; 
			RectTransform iconTrans = iconObj.GetComponent<RectTransform> ();
			iconTrans.localPosition = new Vector2 (startingX + dist_between_slots * i, r.rect.center.y);
			ItemInventorySlotUI slot = iconObj.GetComponent<ItemInventorySlotUI> ();
			slot.setSlotBackingInfo (slotBackings[i]);
			slots [i] = slot;
		}
		updateUI ();
		selecticle.SetAsLastSibling ();
	}

	public void destroySlotUIs() {
		if (slots == null)
			return;

		foreach (ItemInventorySlotUI s in slots) {
			if (s != null)
				GameObject.Destroy (s.gameObject);
		}
	}

	public void updateUI() {
		for (int i = 0; i < slots.Length; i++) {
			ItemInventorySlotUI slot = slots [i];
			slot.updateUI();
		}
	}

	public int getSelectedSlotNumber() {
		return position;
	}

	public Inventory getInventoryBacking() {
		return inventory;
	}
}
