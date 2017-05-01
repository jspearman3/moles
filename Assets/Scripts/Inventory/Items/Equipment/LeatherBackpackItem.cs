using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeatherBackpackItem : BackpackItem {
	public override int spriteIndex {
		get {
			return 4;
		}
	}

	public override int numSlots 
	{ 
		get { return 15; }
	}

}
