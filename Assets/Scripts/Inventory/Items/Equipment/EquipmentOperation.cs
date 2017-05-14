using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EquipmentOperation : InventoryManageMessage {
	public enum Operation { SetHelmet, SetUpperBody, SetLowerBody, SetBoots, SetLeftClaw, SetRightClaw, SetBackpack, SmartSet}

	public byte[] itemData;
	public Operation op;

	public EquipmentOperation (Operation op, byte[] itemData) {
		this.op = op;
		this.itemData = itemData;
	}

	public static bool PerformOperation(Player player, EquipmentOperation operation) {
		return PerformOperationWithBackpackUIUpdate (player, operation, null);
	}

	public static bool PerformOperationWithBackpackUIUpdate(Player player, EquipmentOperation operation, BackpackInventoryUI invUI) {
		Item item = assertGoodItem (operation.itemData);
		if (item == null) {
			return false;
		}
			
		switch (operation.op) {
		case Operation.SetHelmet:
			if (item is HelmetItem) {
				player.info.helmet = item as HelmetItem;
				return true;
			} else {
				return false;
			}
		case Operation.SetUpperBody:
			if (item is UpperBodyItem) {
				player.info.upperBody = item as UpperBodyItem;
				return true;
			} else {
				return false;
			}
		case Operation.SetLowerBody:
			if (item is LowerBodyItem) {
				player.info.lowerBody = item as LowerBodyItem;
				return true;
			} else {
				return false;
			}
		case Operation.SetBoots:
			if (item is BootsItem) {
				player.info.boots = item as BootsItem;
				return true;
			} else {
				return false;
			}
		case Operation.SetLeftClaw:
			if (item is ClawItem) {
				player.info.leftClaw = item as ClawItem;
				return true;
			} else {
				return false;
			}
		case Operation.SetRightClaw:
			if (item is ClawItem) {
				player.info.rightClaw = item as ClawItem;
				return true;
			} else {
				return false;
			}
		case Operation.SetBackpack:
			if (item is BackpackItem) {
				player.info.backpack = item as BackpackItem;
				if (invUI != null)
					invUI.loadInventory (player.info.backpack.inventory);


				return true;
			} else {
				return false;
			}
		case Operation.SmartSet:
			return smartEquip(item, player.info, invUI);
		default:
			Debug.Log ("Unhandled EquipmentOperation case");
			return false;
		}
	}
		


	//returns null if error occured
	private static Item assertGoodItem(byte[] encodedItem) {
		Item item = Item.ReadItem (encodedItem);
		if (item == null) {
			Debug.LogError ("Bad item encoding recieved for equiping item");
		}
		return item;
	}

	private static bool smartEquip(Item equip, PlayerInfo info, BackpackInventoryUI invUI) {
		if (equip is BackpackItem) {
			BackpackItem backpack = equip as BackpackItem;
			if (info.backpack == null) {
				info.backpack = backpack;
				if (invUI != null)
					invUI.loadInventory (info.backpack.inventory);
				return true;
			} 
			return false;
		} else if (equip is HelmetItem) {
			HelmetItem helmet = equip as HelmetItem;
			if (info.helmet == null) {
				info.helmet = helmet;
				return true;
			} 
			return false;
		} else if (equip is UpperBodyItem) {
			UpperBodyItem upperBody = equip as UpperBodyItem;
			if (info.upperBody == null) {
				info.upperBody = upperBody;
				return true;
			} 
			return false;
		} else if (equip is LowerBodyItem) {
			LowerBodyItem lowerBody = equip as LowerBodyItem;
			if (info.lowerBody == null) {
				info.lowerBody = lowerBody;
				return true;
			} 
			return false;
		} else if (equip is BootsItem) {
			BootsItem boots = equip as BootsItem;
			if (info.boots == null) {
				info.boots = boots;
				return true;
			} 
			return false;
		} else if (equip is ClawItem) {
			ClawItem claw = equip as ClawItem;
			if (info.rightClaw == null) {
				info.rightClaw = claw;
				return true;
			} else if (info.leftClaw == null) {
				info.leftClaw = claw;
			} else {
				return false;
			}
		} 

		return false;
	}
		


	public override void Deserialize(NetworkReader reader)
	{
		//get number of slots to read
		op = (Operation) reader.ReadPackedUInt32();
		itemData = reader.ReadBytesAndSize ();
	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		//write operation
		writer.WritePackedUInt32((uint) op);
		writer.WriteBytesFull (itemData);
	}
}



