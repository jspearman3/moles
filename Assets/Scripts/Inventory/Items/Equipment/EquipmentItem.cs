using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EquipmentItem : Item {

	protected int armor = 0;

	public override int stackSize 
	{ 
		get { return 1; }
	}
}
