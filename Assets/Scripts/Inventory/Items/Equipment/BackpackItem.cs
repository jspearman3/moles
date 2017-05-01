using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BackpackItem : EquipmentItem {
	public BackpackInventory inventory;
	public virtual int numSlots 
	{ 
		get { throw new System.NotImplementedException(); }
	}
	public BackpackItem() {
		this.inventory = new BackpackInventory(numSlots);
	}

	public override bool use (Player player)
	{
		player.EquipBackpack (this);
		return true;
	}
}
