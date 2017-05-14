using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InventoryOperationRequest : MessageBase {
	public enum Operation { ToCursor, FromCursor }
	public enum InventoryType { Belt, Backpack, Equip, Ground}
	public enum EquipSlots { Helmet = 0, UpperBody = 1, LowerBody = 2, Boots = 3, LeftClaw = 4, RightClaw = 5, Backpack = 6 }


	public Operation op;
	public InventoryType swapInv;
	public int swapInvSlot;
	public int quantity;

	//constructor for full stack swap
	public InventoryOperationRequest (Operation op, InventoryType swapInv, int swapInvSlot) : this (op, swapInv, swapInvSlot, 0) {}

	//constructor for splitting the stack. If quantity = 0, do full stack swap.
	public InventoryOperationRequest (Operation op, InventoryType swapInv, int swapInvSlot, int quantity) {
		this.op = op;
		this.swapInv = swapInv;
		this.swapInvSlot = swapInvSlot;
		this.quantity = quantity;
	}

	public static bool validateRequest(InventoryOperationRequest request, PlayerInfo info) {
		BackpackInventory backpack = null;
		if (info.backpack != null) {
			backpack = info.backpack.inventory;
		}
		BeltInventory belt = info.belt;
		ItemInventorySlot cursor = info.cursorSlot;

		ItemInventorySlot swapSlot = null;
		EquipmentItem swapEquipItem = null;
		bool drop = false;

		switch (request.swapInv) {
		case InventoryType.Ground:
			drop = true;
			break;
		case InventoryType.Backpack:
			swapSlot = assertSlotExists (backpack.getSlots (), request.swapInvSlot);
			if (swapSlot == null) {
				Debug.Log ("InventoryOperation validation error: trying to swap to bad backpack slot");
				return false;
			}
			break;
		case InventoryType.Belt:
			swapSlot = assertSlotExists (belt.getSlots (), request.swapInvSlot);
			if (swapSlot == null) {
				Debug.Log ("InventoryOperation validation error: trying to swap to bad belt slot");
				return false;
			}
			break;
		case InventoryType.Equip:
			swapEquipItem = getEquipItem (info, request.swapInvSlot);
			break;
		default:
			Debug.Log("InventoryOperation validation error: unsupported inventory detected");
			return false;
		}

		switch (request.op) {
		case Operation.ToCursor:
			return validateToCursorOperation (swapSlot, swapEquipItem, request.quantity, info);
		case Operation.FromCursor:
			return validateFromCursorOperation (swapSlot, swapEquipItem, request.swapInvSlot, drop, request.quantity, info);
		default:
			Debug.Log("InventoryOperation validation error: unsupported operation detected");
			return false;
		}

	}


	private static bool validateToCursorOperation(ItemInventorySlot swapSlot, EquipmentItem swapEquipItem, int quantity, PlayerInfo info) {
		//cursor is always empty for a valid tocursor operation
		if (info.cursorSlot == null) {
			Debug.Log ("InventoryOperation validation error: cursor slot is null.");
			return false;
		}

		if (!info.cursorSlot.isEmpty ()) {
			Debug.Log ("InventoryOperation validation error: cursor slot is not empty. Has " + info.cursorSlot.getQuantity() + " " + info.cursorSlot.getItem() + " items");
			return false;
		}
			

		//if not coming from anywhere, invalid operation
		if (swapSlot == null && swapEquipItem == null) {
			Debug.Log ("InventoryOperation validation error: source slot is null");
			return false;
		}

		// if quantity exceeds source quantity, invalid.
		if (swapSlot == null && quantity > swapEquipItem.stackSize) {
			Debug.Log ("InventoryOperation validation error: trying to take more than one equipment item");
			return false;
		} else if (swapSlot != null && quantity > swapSlot.getQuantity ()) {
			Debug.Log ("InventoryOperation validation error: trying to take more items than exists in source slot");
			return false;
		}
			
		return true;
	}

	private static bool validateFromCursorOperation(ItemInventorySlot swapSlot, EquipmentItem swapEquipItem, int slotNum, bool drop, int quantity, PlayerInfo info) {

		//cursor must have something in it for a valid transfer
		if (info.cursorSlot == null || info.cursorSlot.isEmpty ()) {
			Debug.Log ("InventoryOperation validation error: cursor is null or has no contents to transfer");
			return false;
		}

		Item transferItem = info.cursorSlot.getItem ();
		int cursorQuantity = info.cursorSlot.getQuantity ();

		//quantity tranferred cannot be greater than that in cursor
		if (quantity > cursorQuantity) {
			Debug.Log ("InventoryOperation validation error: cannot transfer more items than cursor has");
			return false;
		}
			


		bool handleEquipment = true;

		//if there is an equipment slot or if dropping, not performing an equipment change
		if (swapSlot != null || drop == true) 
			handleEquipment = false;	


		if (handleEquipment) {
			//equippables dont stack
			if (cursorQuantity > 1) {
				Debug.Log ("InventoryOperation validation error: cursor cannot carry more than one equipment item");
				return false;
			}
				

			EquipSlots eSlot = (EquipSlots)slotNum;

			switch (eSlot) {
			case EquipSlots.Helmet:
				//check for inconsistency. 
				if (info.helmet != null && swapEquipItem == null) {
					Debug.Log ("InventoryOperation validation error: trying to equip incorrect equipment to helmet slot or slot is occupied");
					return false;
				}
				return transferItem is HelmetItem;
			case EquipSlots.UpperBody:
				if (info.upperBody != null && swapEquipItem == null) {
					Debug.Log ("InventoryOperation validation error: trying to equip incorrect equipment to upper body slot or slot is occupied");
					return false;
				}
				return transferItem is UpperBodyItem;
			case EquipSlots.LowerBody:
				if (info.lowerBody != null && swapEquipItem == null) {
					Debug.Log ("InventoryOperation validation error: trying to equip incorrect equipment to lower body slot or slot is occupied");
					return false;
				}
				return transferItem is LowerBodyItem;
			case EquipSlots.Boots:
				if (info.boots != null && swapEquipItem == null) {
					Debug.Log ("InventoryOperation validation error: trying to equip incorrect equipment to boots slot or slot is occupied");
					return false;
				}
				return transferItem is BootsItem;
			case EquipSlots.LeftClaw:
				if (info.leftClaw != null && swapEquipItem == null) {
					Debug.Log ("InventoryOperation validation error: trying to equip incorrect equipment to left claw slot or slot is occupied");
					return false;
				}
				return transferItem is ClawItem;
			case EquipSlots.RightClaw:
				if (info.rightClaw != null && swapEquipItem == null) {
					Debug.Log ("InventoryOperation validation error: trying to equip incorrect equipment to right claw slot or slot is occupied");
					return false;
				}
				return transferItem is ClawItem;
			case EquipSlots.Backpack:
				if (info.backpack != null && swapEquipItem == null) {
					Debug.Log ("InventoryOperation validation error: trying to equip incorrect equipment to backpack slot or slot is occupied");
					return false;
				}
				return transferItem is BackpackItem;
			default:
				Debug.Log ("InventoryOperation validation error: unrecognized equipment type");
				return false;
			}
		} else {
			if (swapSlot != null) {
				//if enough space and item is same as destination, success.
				if (swapSlot.isAddableItems (transferItem, quantity)) {
					return true;
				} else {
					Debug.Log ("InventoryOperation validation error: trying to unequip non-existing helmet");
					return false;
				}

			} else {
				//if not a swap slot, must be drop. Return drop success.
				return drop;
			}
		}
	}

	private static ItemInventorySlot assertSlotExists(ItemInventorySlot[] slots, int index) {
		if (slots.Length <= index || index < 0) {
			return null;
		}
		return slots [index];
	}

	private static EquipmentItem getEquipItem(PlayerInfo info, int slotNum) {
		if (System.Enum.GetValues (typeof(EquipSlots)).Length <= slotNum || slotNum < 0) {
			return null;
		}

		EquipSlots slot = (EquipSlots)slotNum;

		switch (slot) {
		case EquipSlots.Helmet:
			return info.helmet;
		case EquipSlots.UpperBody:
			return info.upperBody;
		case EquipSlots.LowerBody:
			return info.lowerBody;
		case EquipSlots.Boots:
			return info.boots;
		case EquipSlots.LeftClaw:
			return info.leftClaw;
		case EquipSlots.RightClaw:
			return info.rightClaw;
		case EquipSlots.Backpack:
			return info.backpack;
		default:
			Debug.Log("unsupported inventory detected");
			return null;
		}
	}

	//returns items to be dropped/spawned
	public static ItemInventorySlot performInventoryRequest(InventoryOperationRequest request, Player p) {
		switch (request.op) {
		case InventoryOperationRequest.Operation.ToCursor:
			switch (request.swapInv) {
			case InventoryOperationRequest.InventoryType.Belt:
				ItemInventorySlot beltslot = p.info.belt.getSlots () [request.swapInvSlot];
				ItemInventorySlot tempSlot = new ItemInventorySlot ();
				if (request.quantity <= 0) {
					tempSlot.setSlot (beltslot.getItem (), beltslot.getQuantity ());
					beltslot.setSlot (p.info.cursorSlot.getItem (), p.info.cursorSlot.getQuantity ());
					p.info.cursorSlot.setSlot (tempSlot.getItem (), tempSlot.getQuantity ());
				} else {
					p.info.cursorSlot.addItemMany (beltslot.getItem (), request.quantity);
					beltslot.removeMany (request.quantity);
				}
				break;
			case InventoryOperationRequest.InventoryType.Backpack:
				ItemInventorySlot backpackslot = p.info.backpack.inventory.getSlots () [request.swapInvSlot];
				ItemInventorySlot tempSlot2 = new ItemInventorySlot ();
				if (request.quantity <= 0) {
					tempSlot2.setSlot (backpackslot.getItem (), backpackslot.getQuantity ());
					backpackslot.setSlot (p.info.cursorSlot.getItem (), p.info.cursorSlot.getQuantity ());
					p.info.cursorSlot.setSlot (tempSlot2.getItem (), tempSlot2.getQuantity ());

				} else {
					p.info.cursorSlot.addItemMany (backpackslot.getItem (), request.quantity);
					backpackslot.removeMany (request.quantity);
				}
				break;
			case InventoryOperationRequest.InventoryType.Equip:
				switch ((InventoryOperationRequest.EquipSlots)request.swapInvSlot) {
				case InventoryOperationRequest.EquipSlots.Helmet:
					p.info.cursorSlot.setSlot (p.info.helmet, 1);
					p.info.helmet = null;
					break;
				case InventoryOperationRequest.EquipSlots.UpperBody:
					p.info.cursorSlot.setSlot (p.info.upperBody, 1);
					p.info.upperBody = null;
					break;
				case InventoryOperationRequest.EquipSlots.LowerBody:
					p.info.cursorSlot.setSlot (p.info.lowerBody, 1);
					p.info.lowerBody = null;
					break;
				case InventoryOperationRequest.EquipSlots.Boots:
					p.info.cursorSlot.setSlot (p.info.boots, 1);
					p.info.boots = null;
					break;
				case InventoryOperationRequest.EquipSlots.RightClaw:
					p.info.cursorSlot.setSlot (p.info.rightClaw, 1);
					p.info.rightClaw = null;
					break;
				case InventoryOperationRequest.EquipSlots.LeftClaw:
					p.info.cursorSlot.setSlot (p.info.leftClaw, 1);
					p.info.leftClaw = null;
					break;
				case InventoryOperationRequest.EquipSlots.Backpack:
					p.info.cursorSlot.setSlot (p.info.backpack, 1);
					p.info.backpack = null;
					break;
				}
				break;
			}
			break;
		case InventoryOperationRequest.Operation.FromCursor:

			Item cursorItem = p.info.cursorSlot.getItem ();

			switch (request.swapInv) {
			//if moving to an inventory slot, slot is either empty or has the item in it already. 
			case InventoryOperationRequest.InventoryType.Belt:
				ItemInventorySlot beltslot = p.info.belt.getSlots () [request.swapInvSlot];

				if (request.quantity == 0) {
					if (!p.info.cursorSlot.getItem().IsSameType(beltslot.getItem())) {
						ItemInventorySlot tempSlot = new ItemInventorySlot ();
						tempSlot.setSlot (beltslot.getItem (), beltslot.getQuantity ());
						beltslot.setSlot (p.info.cursorSlot.getItem (), p.info.cursorSlot.getQuantity ());
						p.info.cursorSlot.setSlot (tempSlot.getItem (), tempSlot.getQuantity ());
					} else {
						int remainder = beltslot.addItemMany (cursorItem, p.info.cursorSlot.getQuantity ());
						p.info.cursorSlot.removeMany (p.info.cursorSlot.getQuantity () - remainder);
					}


				} else {
					beltslot.addItemMany (cursorItem, request.quantity);
					p.info.cursorSlot.removeMany (request.quantity);
				}
				break;
			case InventoryOperationRequest.InventoryType.Backpack:
				ItemInventorySlot backpackslot = p.info.backpack.inventory.getSlots () [request.swapInvSlot];


				if (request.quantity == 0) {

					if (!p.info.cursorSlot.getItem().IsSameType(backpackslot.getItem())) {
						ItemInventorySlot tempSlot2 = new ItemInventorySlot ();
						tempSlot2.setSlot (backpackslot.getItem (), backpackslot.getQuantity ());
						backpackslot.setSlot (p.info.cursorSlot.getItem (), p.info.cursorSlot.getQuantity ());
						p.info.cursorSlot.setSlot (tempSlot2.getItem (), tempSlot2.getQuantity ());
					} else {
						int remainder = backpackslot.addItemMany (cursorItem, p.info.cursorSlot.getQuantity ());
						p.info.cursorSlot.removeMany (p.info.cursorSlot.getQuantity () - remainder);
					}

				} else {
					backpackslot.addItemMany (cursorItem, request.quantity);
					p.info.cursorSlot.removeMany (request.quantity);
				}
				break;
			case InventoryOperationRequest.InventoryType.Equip:
				switch ((InventoryOperationRequest.EquipSlots)request.swapInvSlot) {
				case InventoryOperationRequest.EquipSlots.Helmet:
					Item tempHelmet = p.info.helmet;
					p.info.helmet = p.info.cursorSlot.getItem () as HelmetItem;
					p.info.cursorSlot.setSlot (tempHelmet, 1);
					break;
				case InventoryOperationRequest.EquipSlots.UpperBody:
					Item tempUpperBody = p.info.upperBody;
					p.info.upperBody = p.info.cursorSlot.getItem () as UpperBodyItem;
					p.info.cursorSlot.setSlot (tempUpperBody, 1);
					break;
				case InventoryOperationRequest.EquipSlots.LowerBody:
					Item tempLowerBody = p.info.lowerBody;
					p.info.lowerBody = p.info.cursorSlot.getItem () as LowerBodyItem;
					p.info.cursorSlot.setSlot (tempLowerBody, 1);
					break;
				case InventoryOperationRequest.EquipSlots.Boots:
					Item tempBoots = p.info.boots;
					p.info.boots = p.info.cursorSlot.getItem () as BootsItem;
					p.info.cursorSlot.setSlot (tempBoots, 1);
					break;
				case InventoryOperationRequest.EquipSlots.RightClaw:
					Item tempRightClaw = p.info.rightClaw;
					p.info.rightClaw = p.info.cursorSlot.getItem () as ClawItem;
					p.info.cursorSlot.setSlot (tempRightClaw, 1);
					break;
				case InventoryOperationRequest.EquipSlots.LeftClaw:
					Item tempLeftClaw = p.info.leftClaw;
					p.info.leftClaw = p.info.cursorSlot.getItem () as ClawItem;
					p.info.cursorSlot.setSlot (tempLeftClaw, 1);
					break;
				case InventoryOperationRequest.EquipSlots.Backpack:
					Item tempBackpack = p.info.backpack;
					p.info.backpack = p.info.cursorSlot.getItem () as BackpackItem;
					p.info.cursorSlot.setSlot (tempBackpack, 1);
					break;
				}
				break;
			case InventoryOperationRequest.InventoryType.Ground:
				ItemInventorySlot dropped = new ItemInventorySlot ();
				if (request.quantity == 0) {
					dropped.setSlot (p.info.cursorSlot.getItem (), p.info.cursorSlot.getQuantity ());
					p.info.cursorSlot.removeMany (p.info.cursorSlot.getQuantity ());
				} else {
					dropped.setSlot (p.info.cursorSlot.getItem (), request.quantity);
					p.info.cursorSlot.removeMany (request.quantity);
				}
				return dropped;
			}
			break;
		}
		return new ItemInventorySlot();
	}

	public override void Deserialize(NetworkReader reader)
	{
		op = (Operation) reader.ReadPackedUInt32();
		swapInv = (InventoryType) reader.ReadPackedUInt32();
		swapInvSlot = (int) reader.ReadPackedUInt32();
		quantity = (int) reader.ReadPackedUInt32();
	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		writer.WritePackedUInt32((uint) op);
		writer.WritePackedUInt32((uint) swapInv);
		writer.WritePackedUInt32((uint) swapInvSlot);
		writer.WritePackedUInt32((uint) quantity);
	}
}



