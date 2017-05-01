using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventorySlotUI : MonoBehaviour {
	public static Sprite transparent;

	public ItemInventorySlot slot;
	public Image image;
	public Text quantityText;

	void Start () {
		image = GetComponentInChildren<Image> ();
		quantityText = GetComponentInChildren<Text> ();
		transparent = Resources.Load<Sprite> ("Textures/transparent");
		updateUI ();
	}

	public void setSlotBackingInfo(ItemInventorySlot slot) {
		this.slot = slot;
	}

	public void setSlot(Item item, int quantity) {
		slot.setSlot (item, quantity);
	}

	public ItemInventorySlot getSlotBackingInfo() {
		return slot;
	}

	public Sprite getIcon() {
		if (slot == null) {
			return transparent;
		} else if (slot.isEmpty ()) {
			return transparent;
		} else {
			return slot.getIcon ();
		}
	}

	public void updateUI() {
		if (image == null) {
			image = GetComponentInChildren<Image> ();
		}

		image.overrideSprite = getIcon ();

		string s;

		if (slot == null) {
			slot = new ItemInventorySlot ();
		}

		if (quantityText == null)
			quantityText = GetComponentInChildren<Text> ();
		
		int q = slot.getQuantity ();
		if (q < 2) {
			quantityText.text = "";
		} else if (slot.getQuantity() < 100) {
			quantityText.text = q.ToString ();
		} else {
			quantityText.text = "99+";
		}
	}

}
