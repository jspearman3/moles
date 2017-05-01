using UnityEngine;

public class GrubItem : Item {
	public override int stackSize {
		get {
			return 3;
		}
	}

	public override int spriteIndex 
	{ 
		get { return 2; }
	}

	public override bool use (Player p)
	{
		return true;
	}
}