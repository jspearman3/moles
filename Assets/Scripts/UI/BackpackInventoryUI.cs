using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackInventoryUI : MonoBehaviour {

	public const int NUM_SLOTS_WIDTH = 5;

	private Text noBackpackText;
	ItemInventorySlotUI[] slots;
	BackpackInventory inventory;
	public RectTransform slotSeed;
	public GameObject inventorySlotPrefab;
	public RectTransform trans;

	public bool opened;
	private Vector3 closedPosition;
	private Vector3 openPosition;


	void Start () {
		noBackpackText = GetComponentInChildren<Text> ();
		trans = GetComponent<RectTransform> ();
		openPosition = trans.localPosition;
		closedPosition = openPosition + new Vector3 (2 * trans.rect.width, 0);
	}

	void Update() {		
		if (opened) {
			trans.localPosition = Vector3.Lerp(trans.localPosition, openPosition, .2f);
		} else {
			trans.localPosition = Vector3.Lerp(trans.localPosition, closedPosition, .2f);
		}

	}

	public Inventory getInventoryBacking() {
		return inventory;
	}

	public void loadInventory(BackpackInventory inventory) {
		if (inventory == null) {
			this.inventory = null;
			noBackpackText.color = new Color (noBackpackText.color.r, noBackpackText.color.g, noBackpackText.color.b, 1);
			destroySlots ();
			return;
		}

		destroySlots ();
		this.inventory = inventory;
		buildSlots ();
		updateUI();

		noBackpackText.color = new Color(noBackpackText.color.r, noBackpackText.color.g, noBackpackText.color.b, 0);
	}

	void buildSlots() {
		if (inventory == null) {
			Debug.Log ("inventory null... returning");
			return;
		}

		ItemInventorySlot[] slotBackings = inventory.getSlots ();
		Debug.Log ("num slots in inv: " + slotBackings.Length);
		slots = new ItemInventorySlotUI[slotBackings.Length];
		Debug.Log ("num mod 5: " + slots.Length % NUM_SLOTS_WIDTH);
		Debug.Log ("num div 5: " + slots.Length / NUM_SLOTS_WIDTH);

		for (int i = 0; i < slots.Length; i++) {

			int col = i % NUM_SLOTS_WIDTH;
			int row = i / NUM_SLOTS_WIDTH;

			Debug.Log ("building slot");
			GameObject iconObj = GameObject.Instantiate (inventorySlotPrefab, GetComponent<Transform> ());
			RectTransform iconTrans = iconObj.GetComponent<RectTransform> ();
			iconTrans.localPosition = slotSeed.localPosition + new Vector3 (slotSeed.rect.size.x * col, slotSeed.rect.size.y * row);
			ItemInventorySlotUI slot = iconObj.GetComponent<ItemInventorySlotUI> ();
			slot.setSlotBackingInfo (slotBackings[i]);
			slots [i] = slot;

		}
	}

	void destroySlots() {
		if (slots == null) {
			return;
		}
		foreach (ItemInventorySlotUI s in slots) {
			if (s == null) {
				continue;
			}
			Destroy (s.gameObject);
		}
		slots = null;
	}

	public void updateUI() {
		if (slots == null) {
			return;
		}
		foreach (ItemInventorySlotUI s in slots) {
			if (s == null) {
				Debug.Log ("skipping update because stupid");
				continue;
			}
			s.updateUI();
		}
	}
}
