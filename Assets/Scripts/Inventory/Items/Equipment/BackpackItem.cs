using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class BackpackItem : EquipmentItem {
	public BackpackInventory inventory;
	public virtual int numSlots 
	{ 
		get { throw new System.NotImplementedException(); }
	}
	public BackpackItem() {
		this.inventory = new BackpackInventory(numSlots);
	}

	public override void Deserialize(NetworkReader reader)
	{
		base.Deserialize (reader);
		inventory = new BackpackInventory (0);
		inventory.Deserialize (reader);
	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		base.Serialize (writer);
		inventory.Serialize (writer);
	}
}
