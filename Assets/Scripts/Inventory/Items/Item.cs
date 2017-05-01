using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : IMessagable<Item> {
	private const int DEFAULT_STACK_SIZE = 50;

	public virtual int stackSize 
	{ 
		get { return DEFAULT_STACK_SIZE; }
	}

	public static Dictionary<int, Sprite> itemToIconMap = new Dictionary<int, Sprite>() {
		{0, Resources.Load<Sprite> ("Textures/Items/dirtIcon")},
		{1, Resources.Load<Sprite> ("Textures/Items/rockIcon")},
		{2, Resources.Load<Sprite> ("Textures/Items/grubIcon")},
		{3, Resources.Load<Sprite> ("Textures/Items/wormIcon")},
		{4, Resources.Load<Sprite> ("Textures/Items/leatherBackpackIcon")}
	};

	public virtual int spriteIndex 
	{ 
		get { throw new System.NotImplementedException(); }
	}

	public Item() {
	}

	//override if item performs an action on right click. Return whether item is consumed.
	public virtual bool use (Player p) {
		return false;
	}

	public bool IsSameType(object other) {
		if (other.GetType().Equals(this.GetType())) {
			Item o = other as Item;
			return o.spriteIndex == this.spriteIndex;
		}
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
		} else if (res == 2) {
			return new GrubItem ();
		} else if (res == 3) {
			return new WormItem ();
		} else if (res == 4) {
			return new LeatherBackpackItem ();
		} else {
			return null;
		}
		
	}

}
