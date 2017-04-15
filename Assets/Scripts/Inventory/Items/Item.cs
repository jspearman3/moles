using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : IMessagable<Item> {

	public static Dictionary<int, Sprite> itemToIconMap = new Dictionary<int, Sprite>() {
		{0, Resources.Load<Sprite> ("Textures/Items/dirtIcon")},
		{1, Resources.Load<Sprite> ("Textures/Items/rockIcon")}
	};

	public int spriteIndex = -1;

	public Item() {
	}

	//override if item performs an action on right click. Return whether item is consumed.
	public virtual bool use () {
		return false;
	}

	public Sprite getIcon() {
		return itemToIconMap[spriteIndex];
	}

	public string Encode() {
		return spriteIndex.ToString();
	}

	public Item Decode(string s) {
		int res = Int32.Parse (s);

		if (res == 0) {
			return new DirtItem ();
		} else if (res == 1) {
			return new RockItem ();
		} else {
			return null;
		}
		
	}

}
