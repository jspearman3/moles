using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBar : MonoBehaviour {

	int position = 0;
	const int NUM_SLOTS = 10;
	float dist_between_slots = 0;
	float startingX = 0;
	public bool inputEnable = true;

	public Transform selecticle;

	private ItemInventorySlot[] slots;

	public GameObject inventorySlotPrefab;

	// Use this for initialization
	void Start () {
		

		RectTransform r = GetComponent<RectTransform> ();
		dist_between_slots = r.rect.width / NUM_SLOTS;
		startingX = r.rect.xMin + dist_between_slots / 2;

		slots = new ItemInventorySlot [NUM_SLOTS];

		for (int i = 0; i < NUM_SLOTS; i++) {
			GameObject iconObj = GameObject.Instantiate (inventorySlotPrefab, GetComponent<Transform> ());
			RectTransform iconTrans = iconObj.GetComponent<RectTransform> ();
			iconTrans.localPosition = new Vector2 (startingX + dist_between_slots * i, r.rect.center.y);
			slots [i] = iconObj.GetComponent<ItemInventorySlot> ();

		}
	}
	
	// Update is called once per frame
	void Update () {
		if (inputEnable)
			handleInput();
		float targetXPos = startingX + dist_between_slots * position;

		selecticle.localPosition = Vector2.Lerp (selecticle.localPosition, new Vector2 (targetXPos, selecticle.localPosition.y), .2f);

		if (Input.GetKeyDown (KeyCode.Y)) {
			addItem (new RockItem ());
			Debug.Log ("added rock");
		} else if (Input.GetKeyDown (KeyCode.U)) {
			addItem (new DirtItem ());
			Debug.Log ("added dirt");
		} else if (Input.GetKeyDown (KeyCode.H)) {
			removeFromSlot (0);
		} else if (Input.GetKeyDown (KeyCode.J)) {
			removeFromSlot (1);
		}


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

	private void updateUI() {
		for (int i = 0; i < NUM_SLOTS; i++) {
			ItemInventorySlot slot = slots [i];
			slot.updateUI();
		}
	}


		
	private ItemInventorySlot getFirstSlotForItem(Item item) {
		foreach (ItemInventorySlot s in slots) {
			if (s.isType(item)) {
				return s;
			}
		}

		foreach (ItemInventorySlot s in slots) {
			if (s.isEmpty()) {
				return s;
			}
		}

		return null;
	}

	//return null if none open
	private ItemInventorySlot getNextOpenSlot() {
		foreach (ItemInventorySlot s in slots) {
			if (s.isEmpty ()) {
				return s;
			}
		}

		return null;
	}

	public bool addItemMany(Item item, int quantity) {
		ItemInventorySlot slot = getFirstSlotForItem (item);

		if (slot == null) {
			Debug.Log ("null slot");
			return false;
		}

		return slot.addItemMany (item, quantity);
	}

	public bool addItem(Item item) {
		return addItemMany (item, 1);
	}

	public bool AddItemToSlot(Item item, int slotNum) {
		return AddItemManyToSlot (item, 1, slotNum);
	}

	public bool AddItemManyToSlot(Item item, int quantity, int slotNum) {
		if (slotNum >= NUM_SLOTS || slotNum < 0)
			return false;

		ItemInventorySlot s = slots [slotNum];

		if (s.isEmpty()) {
			return setSlot(item, quantity, slotNum);
		} else {
			if (s.isAddable (item)) {
				return s.addItem (item);
			} else {
				return false;
			}
		}

	}

	public bool removeFromSlot(int slotNum) {
		return removeManyFromSlot (1, slotNum);
	}

	public bool removeManyFromSlot(int quantity, int slotNum) {
		if (slotNum >= NUM_SLOTS || slotNum < 0)
			return false;

		ItemInventorySlot s = slots [slotNum];
		s.removeMany (quantity);
		return true;
	}

	public void clearSlot(int slotNum) {
		if (slotNum >= NUM_SLOTS || slotNum < 0)
			return;

		slots [slotNum].clear ();
	}

	//returns whether operation was successful
	public bool setSlot(Item item, int quantity, int slotNum) {
		if (quantity < 0) {
			return false;
		}

		if (slotNum >= NUM_SLOTS || slotNum < 0)
			return false;

		slots [slotNum].setSlot (item, quantity);

		return true;
	}
}
