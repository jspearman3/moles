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

	public bool isAddableItems(Item item, int quantity) {
		if (isEmpty () || quantity == 0) {
			return true;
		} else {
			if (isType (item)) {
				return item.stackSize - this.quantity > quantity;
			} else {
				return false;
			}
		}
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

	public int clear() {
		item = null;
		quantity = 0;
		return 0;
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

	public int removeMany(int i) {
		return addItemMany (item, -i);
	}

	public int removeOne() {
		return removeMany (1);
	}

	public int setSlot(Item item, int quantity) {
		if (item == null) {
			clear ();
			return 0;
		}

		this.item = item;

		if (quantity > item.stackSize) {
			this.quantity = item.stackSize;
		} else if (quantity <= 0) {
			clear ();
		} else {
			this.quantity = quantity;
		}

		return 0;
			
	}

	public Item getItem() {
		return item;
	}

	public int getQuantity() {
		return quantity;
	}
		
	public override void Deserialize(NetworkReader reader)
	{
		item = Item.ReadItem(reader);
		quantity = (int) reader.ReadPackedUInt32 ();
	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		if (item == null) {
			writer.WriteBytesFull (System.Text.Encoding.Default.GetBytes ("null"));
		} else {
			item.Serialize (writer);
		}
		writer.WritePackedUInt32 ((uint) quantity);
	}
}
