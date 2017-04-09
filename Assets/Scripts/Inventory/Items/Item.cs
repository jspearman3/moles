using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item {

	protected Sprite gameSprite;
	protected Sprite icon;

	//override if item performs an action on right click. Return whether item is consumed.
	public virtual bool use () {
		return false;
	}

	public Sprite getIcon() {
		return icon;
	}

}
