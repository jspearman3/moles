using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerInfo : MessageBase {

	public BeltInventory belt;

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

	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		belt.Serialize (writer);
	}
}
