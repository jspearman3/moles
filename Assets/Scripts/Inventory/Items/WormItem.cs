using UnityEngine;

public class WormItem : Item {
	public override int stackSize {
		get {
			return 10;
		}
	}

	public override int spriteIndex 
	{ 
		get { return 3; }
	}
}