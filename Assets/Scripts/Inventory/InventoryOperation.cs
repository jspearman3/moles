using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryOperation : InventoryManageMessage {
	public enum Operation { AddItems, AddItemsToSlot, RemoveItemsFromSlot, Set, Clear}

	public byte[][] paramaters;
	public Operation op;

	public InventoryOperation (Operation op, byte[][] paramaters) {
		this.op = op;
		this.paramaters = paramaters;
	}

	public static int PerformOperation(Inventory inv, InventoryOperation operation) {
		switch (operation.op) {
		case Operation.AddItems:
			return AddItems (inv, operation.paramaters);
		case Operation.AddItemsToSlot:
			return AddItemsToSlot (inv, operation.paramaters);
		case Operation.RemoveItemsFromSlot:
			return RemoveItemsFromSlot (inv, operation.paramaters);
		case Operation.Set:
			return SetSlot (inv, operation.paramaters);
		case Operation.Clear:
			return ClearSlot (inv, operation.paramaters);
		default:
			Debug.Log ("Unhandled InventoryOperation case");
			return -1;
		}
	}

	//returns true if success.
	private static bool assertParamLength(byte[][] parameters, int length, string methodName) {
		if (parameters.Length != length) {
			Debug.LogError ("Recieved bad parameter length from method " + methodName + ". Expected " + length + ", recieved " + parameters.Length);
			return false;
		}
		return true;
	}

	//returns null if error occured
	private static Item assertGoodItem(byte[] encodedItem, string methodName) {
		Item item = Item.ReadItem(encodedItem);
		if (item == null) {
			Debug.LogError ("Bad item encoding recieved for method " + methodName);
		}
		return item;
	}

	//returns true if success
	private static bool assertGoodInt(byte[] integerBytes, out int intRef, string methodName) {
		bool success = System.Int32.TryParse (System.Text.Encoding.Default.GetString(integerBytes), out intRef);
		if (!success)
			Debug.LogError ("Bad int recieved for method " + methodName);
	
		return success;
	}

	//2 parameters: Item (encoded value) and quantity (int to be parsed)
	public static int AddItems(Inventory inv, byte[][] parameters) {
		string methodName = "AddItems";

		if (!assertParamLength(parameters, 2, methodName))
			return -1;
		

		Item item = assertGoodItem(parameters[0], methodName);
		if (item == null) 
			return -1;

		int quant = 0;

		if (assertGoodInt (parameters [1], out quant, methodName))
			return inv.addItemMany (item, quant);

		return -1;
	}

	//2 parameters: Item (encoded value) and quantity (int to be parsed)
	public static int AddItemsToSlot(Inventory inv, byte[][] parameters) {
		string methodName = "AddItemsToSlot";

		if (!assertParamLength(parameters, 3, methodName))
			return -1;


		Item item = assertGoodItem(parameters[0], methodName);
		if (item == null) 
			return -1;

		int quant = 0;

		if (assertGoodInt (parameters [1], out quant, methodName)) {
			int slotNum = 0;
			if (assertGoodInt (parameters [2], out slotNum, methodName))
				return inv.AddItemManyToSlot (item, quant, slotNum);

		}

		return -1;
	}	

	public static int RemoveItemsFromSlot(Inventory inv, byte[][] parameters) {
		string methodName = "RemoveItemsFromSlot";

		if (!assertParamLength(parameters, 3, methodName))
			return -1;

		int quant = 0;

		if (assertGoodInt (parameters [0], out quant, methodName)) {
			int slotNum = 0;
			if (assertGoodInt (parameters [1], out slotNum, methodName))
				return inv.removeManyFromSlot (quant, slotNum);

		}

		return -1;
	}

	//2 parameters: Item (encoded value) and quantity (int to be parsed)
	public static int SetSlot(Inventory inv, byte[][] parameters) {
		string methodName = "SetSlot";

		if (!assertParamLength(parameters, 3, methodName))
			return -1;


		Item item = assertGoodItem(parameters[0], methodName);
		if (item == null) 
			return -1;

		int quant = 0;

		if (assertGoodInt (parameters [1], out quant, methodName)) {
			int slotNum = 0;
			if (assertGoodInt (parameters [2], out slotNum, methodName))
				return inv.setSlot (item, quant, slotNum);

		}

		return -1;
	}

	//2 parameters: Item (encoded value) and quantity (int to be parsed)
	public static int ClearSlot(Inventory inv, byte[][] parameters) {
		string methodName = "ClearSlot";

		if (!assertParamLength(parameters, 1, methodName))
			return -1;

		int slotNum = 0;

		if (assertGoodInt (parameters [1], out slotNum, methodName))
			return inv.clearSlot(slotNum);

		return -1;
	}

	public override void Deserialize(NetworkReader reader)
	{
		//get number of slots to read
		op = (Operation) reader.ReadPackedUInt32();

		int numParams = (int) reader.ReadPackedUInt32();

		paramaters = new byte[numParams][];

		for (int i = 0; i < numParams; i++) {
			paramaters [i] = reader.ReadBytesAndSize ();
		}

	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		//write operation
		writer.WritePackedUInt32((uint) op);

		writer.WritePackedUInt32((uint) paramaters.Length);
		foreach (byte[] p in paramaters) {
			writer.WriteBytesFull (p);
		}

	}
}
	


