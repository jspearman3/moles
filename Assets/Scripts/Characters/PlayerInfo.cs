using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerInfo : MessageBase {

	public BeltInventory belt;
	public BackpackItem backpack;

	//only set on server
	public GamePosition lastLogoutPos;

	MoleController controller;

	public PlayerInfo(BeltInventory belt) {
		this.belt = belt;
	}

	public void LoadPlayer(PlayerInfo playerInfo) {
		belt = playerInfo.belt;
	}

	public override void Deserialize(NetworkReader reader)
	{

		belt = new BeltInventory (0);
		belt.Deserialize (reader);

		string itemString = System.Text.Encoding.Default.GetString (reader.ReadBytesAndSize ());

		if (itemString.Equals ("null")) {
			backpack = null;
			return;
		} else {
			backpack = new RockItem ().Decode (itemString) as BackpackItem;
			BackpackInventory inv = new BackpackInventory (0);
			inv.Deserialize (reader);
			backpack.inventory = inv;
		}

	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		belt.Serialize (writer);
		if (backpack == null)
			writer.WriteBytesFull (System.Text.Encoding.Default.GetBytes ("null"));
		else {
			writer.WriteBytesFull (System.Text.Encoding.Default.GetBytes (backpack.Encode ()));
			BackpackInventory inv = backpack.inventory;
			inv.Serialize (writer);
		}
	}
}
