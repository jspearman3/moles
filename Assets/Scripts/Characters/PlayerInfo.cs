using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerInfo : MessageBase {

	public BeltInventory belt;
	public ItemInventorySlot cursorSlot;

	public BackpackItem backpack;
	public HelmetItem helmet;
	public UpperBodyItem upperBody;
	public LowerBodyItem lowerBody;
	public BootsItem boots;
	public ClawItem leftClaw;
	public ClawItem rightClaw;


	//only set on server
	public GamePosition lastLogoutPos;

	MoleController controller;

	public PlayerInfo(BeltInventory belt) {
		this.cursorSlot = new ItemInventorySlot ();
		this.belt = belt;
	}

	public override void Deserialize(NetworkReader reader)
	{

		belt = new BeltInventory (0);
		belt.Deserialize (reader);

		backpack = Item.ReadItem (reader) as BackpackItem;
		helmet = Item.ReadItem (reader) as HelmetItem;
		upperBody = Item.ReadItem (reader) as UpperBodyItem;
		lowerBody = Item.ReadItem (reader) as LowerBodyItem;
		boots = Item.ReadItem (reader) as BootsItem;
		leftClaw = Item.ReadItem (reader) as ClawItem;
		rightClaw = Item.ReadItem (reader) as ClawItem;

		cursorSlot = new ItemInventorySlot ();
		cursorSlot.Deserialize (reader);
	}

	private void serializeEquipment(EquipmentItem item, NetworkWriter writer) {
		if (item == null)
			writer.WriteBytesFull (System.Text.Encoding.Default.GetBytes ("null"));
		else {
			item.Serialize (writer);
		}
	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		belt.Serialize (writer);
		serializeEquipment (backpack, writer);
		serializeEquipment (helmet, writer);
		serializeEquipment (upperBody, writer);
		serializeEquipment (lowerBody, writer);
		serializeEquipment (boots, writer);
		serializeEquipment (leftClaw, writer);
		serializeEquipment (rightClaw, writer);
		cursorSlot.Serialize (writer);
	}
}
