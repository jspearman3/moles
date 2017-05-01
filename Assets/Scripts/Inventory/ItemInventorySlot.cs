using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemInventorySlot : MessageBase {

	private Item item;
	private int quantity;

	public bool isEmpty() {
		return item == null;
	}

	public bool isFull() {
		if (item == null) {
			return false;
		}

		return quantity == item.stackSize;
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

	public bool isAddableItem(Item item) {
		if (isEmpty ()) {
			return true;
		} else {
			return isType(item);
		}
	}

	public Sprite getIcon() {
		if (isEmpty ()) {
			return null;
		} else {
			return item.getIcon ();
		}
	}

	public void clear() {
		Debug.Log ("clearing...");
		item = null;
		quantity = 0;
	}

	public int addItemMany(Item item, int quantity) {
		if (isAddableItem (item)) {

			if (this.item == null) {
				this.item = item;
			}

			this.quantity += quantity;
			if (this.quantity <= 0) {
				clear ();
			} else if (this.quantity > item.stackSize){
				int remainder = this.quantity - item.stackSize;
				this.quantity = item.stackSize;
				return remainder;
			}
			return 0;
		}
		return -1;
	}

	public int addItem(Item item) {
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
	}

	public Item getItem() {
		return item;
	}

	public int getQuantity() {
		return quantity;
	}
		
	public override void Deserialize(NetworkReader reader)
	{
		string itemString = System.Text.Encoding.Default.GetString (reader.ReadBytesAndSize ());

		if (itemString.Equals ("null")) {
			item = null;
		} else {
			item = new RockItem ().Decode (itemString);
		}

		quantity = (int) reader.ReadPackedUInt32 ();
	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		if (item == null) {
			writer.WriteBytesFull (System.Text.Encoding.Default.GetBytes ("null"));
		} else {
			writer.WriteBytesFull (System.Text.Encoding.Default.GetBytes (item.Encode()));
		}
		writer.WritePackedUInt32 ((uint) quantity);
	}
}
