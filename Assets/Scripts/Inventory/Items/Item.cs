using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Item : MessageBase {
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
		if (other == null) {
			return false;
		}
		if (other.GetType().Equals(this.GetType())) {
			Item o = other as Item;
			return o.spriteIndex == this.spriteIndex;
		}
		return false;
	}

	public Sprite getIcon() {
		return itemToIconMap[spriteIndex];
	}

	public static Item ReadItem(byte[] bytes) {
		NetworkReader reader = new NetworkReader (bytes);
		return ReadItem (reader);
	}

	public static Item ReadItem(NetworkReader reader) {
		string itemCode = System.Text.Encoding.Default.GetString (reader.ReadBytesAndSize ());
		Item item = Decode (itemCode);
		if (item != null)
			item.Deserialize (reader);
		return item;
	}

	private static Item Decode(string s) {
		int res = -1;
		if (Int32.TryParse (s, out res)) {
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
		} else {
			return null;
		}


	}

	// should be overriden if any item metadata is to be saved. If unknown item, must decode
	public override void Deserialize(NetworkReader reader){}

	// should be overriden with initial call to base if any item metadata is to be saved
	public override void Serialize(NetworkWriter writer)
	{
		writer.WriteBytesFull (System.Text.Encoding.Default.GetBytes (spriteIndex.ToString()));
	}

}
