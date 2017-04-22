using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventorySlotUI : MonoBehaviour {
	public static Sprite transparent;

	public ItemInventorySlot slot;
	public Image image;
	public Text quantityText;
	private ItemBar itemBar;

	void Start () {
		image = GetComponent<Image> ();
		quantityText = GetComponentInChildren<Text> ();
		transparent = Resources.Load<Sprite> ("Textures/transparent");
		updateUI ();
	}

	public void setSlotBackingInfo(ItemInventorySlot slot) {
		this.slot = slot;
	}

	public ItemInventorySlot getSlotBackingInfo() {
		return slot;
	}

	public void setItemBar(ItemBar itemBar) {
		this.itemBar = itemBar;
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
		image.overrideSprite = getIcon ();

		string s;

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
