using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Inventory : MessageBase {

	private ItemInventorySlot[] slots;

	public Inventory(int numSlots) {
		slots = new ItemInventorySlot[numSlots];

		for (int i = 0; i < numSlots; i++) {
			slots [i] = new ItemInventorySlot ();
		}
	}

	private ItemInventorySlot getFirstSlotForItem(Item item) {
		foreach (ItemInventorySlot s in slots) {
			if (s.isType(item) && !s.isFull()) {
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

	public int addItemMany(Item item, int quantity) {
		ItemInventorySlot slot = getFirstSlotForItem (item);

		if (slot == null) {
			return quantity;
		}

		int remainder = slot.addItemMany (item, quantity);

		if (remainder > 0) {
			return addItemMany (item, remainder);
		} else {
			return remainder;
		}
	}

	public int addItem(Item item) {
		return addItemMany (item, 1);
	}

	public bool AddItemToSlot(Item item, int slotNum) {
		return AddItemManyToSlot (item, 1, slotNum);
	}

	public bool AddItemManyToSlot(Item item, int quantity, int slotNum) {
		if (slotNum >= slots.Length || slotNum < 0)
			return false;

		ItemInventorySlot s = slots [slotNum];

		if (s.isEmpty()) {
			return setSlot(item, quantity, slotNum);
		} else {
			if (s.isAddableItem (item)) {
				if (s.addItem (item) == -1) {
					return false;
				}
				return true;
			} else {
				return false;
			}
		}

	}

	public bool removeFromSlot(int slotNum) {
		return removeManyFromSlot (1, slotNum);
	}

	public bool removeManyFromSlot(int quantity, int slotNum) {
		if (slotNum >= slots.Length || slotNum < 0)
			return false;

		ItemInventorySlot s = slots [slotNum];
		s.removeMany (quantity);
		return true;
	}

	public void clearSlot(int slotNum) {
		if (slotNum >= slots.Length || slotNum < 0)
			return;

		slots [slotNum].clear ();
	}

	//returns whether operation was successful
	public bool setSlot(Item item, int quantity, int slotNum) {
		if (quantity < 0) {
			return false;
		}

		if (slotNum >= slots.Length || slotNum < 0)
			return false;

		slots [slotNum].setSlot (item, quantity);

		return true;
	}

	public ItemInventorySlot[] getSlots() {
		return slots;
	}

	public void swapSlotPosition(int slotA, int slotB) {
		if (slotA < 0 || slotB < 0 || slotA >= slots.Length || slotB >= slots.Length) {
			return;
		} else {
			ItemInventorySlot temp = slots [slotA];
			slots [slotB] = slots [slotA];
			slots [slotA] = temp;
		}
	}


	public override void Deserialize(NetworkReader reader)
	{
		//get number of slots to read
		int numSlots = (int) reader.ReadPackedUInt32();

		slots = new ItemInventorySlot[numSlots];

		for (int i = 0; i < numSlots; i++) {
			slots [i] = new ItemInventorySlot ();
			slots [i].Deserialize (reader);
		}
			
	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		//write number of slots
		writer.WritePackedUInt32((uint) slots.Length);

		foreach (ItemInventorySlot s in slots) {
			s.Serialize (writer);
		}

	}

	public override string ToString ()
	{
		string returnString = "";
		foreach (ItemInventorySlot s in slots) {
			if (s.isEmpty ()) {
				returnString += "[-]";
			} else {
				Item item = s.getItem ();
				int quant = s.getQuantity ();
				returnString += "[" + item.GetType ().ToString () + ":" + quant.ToString () + "]";
			}
		}

		return returnString;
	}
}
