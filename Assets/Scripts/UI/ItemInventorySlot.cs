using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventorySlot : MonoBehaviour {
	public static Sprite transparent;

	private Item item;
	private int quantity;
	public Image image;
	public Text quantityText;

	void Start () {
		image = GetComponent<Image> ();
		quantityText = GetComponentInChildren<Text> ();
		item = null;
		quantity = 0;
		transparent = Resources.Load<Sprite> ("Textures/transparent");
		updateUI ();
	}

	public bool isEmpty() {
		return item == null;
	}

	public bool isType(Item item) {
		if (this.item == null) {
			if (item == null) {
				return true;
			}
			return false;
		}
		return item.GetType ().Equals (this.item.GetType ());
	}

	public bool isAddable(Item item) {
		if (isEmpty ()) {
			return true;
		} else {
			return isType(item);
		}
	}

	public Sprite getIcon() {
		if (isEmpty ()) {
			return transparent;
		} else {
			return item.getIcon ();
		}
	}

	public void clear() {
		Debug.Log ("clearing...");
		item = null;
		quantity = 0;
		updateUI ();
	}

	public void updateUI() {
		image.overrideSprite = getIcon ();

		string s;

		if (item == null) {
			s = "null";
		} else {
			s = item.ToString ();
		}

		Debug.Log ("updating ui... Item: " + s + " Icon is transparent: " + getIcon().Equals(transparent));
		if (quantity < 2) {
			quantityText.text = "";
		} else if (quantity < 100) {
			quantityText.text = quantity.ToString ();
		} else {
			quantityText.text = "99+";
		}
	}

	public bool addItemMany(Item item, int quantity) {
		if (isAddable (item)) {

			if (this.item == null) {
				Debug.Log ("setting item...");
				this.item = item;
			}

			this.quantity += quantity;
			Debug.Log ("adding " + quantity + ". Now have " + this.quantity);
			if (this.quantity <= 0) {
				clear ();
				Debug.Log ("clearing for some reason.");
			}
			updateUI ();
			return true;
		}
		Debug.Log ("not addable");
		return false;
	}

	public bool addItem(Item item) {
		return addItemMany (item, 1);
	}

	public void removeMany(int i) {
		addItemMany (item, -i);
	}

	public void removeOne() {
		removeMany (1);
	}

	public void setSlot(Item item, int quantity) {
		this.item = item;
		this.quantity = quantity;

		updateUI ();
	}


}
