using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class WorldInventoryManager : NetworkBehaviour {

	private Dictionary<PlayerInfo, Inventory> playerInventoryMap = new Dictionary<PlayerInfo,Inventory>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Inventory getInventory(PlayerInfo p) {
		Inventory inv = null;

		if (playerInventoryMap.TryGetValue(p, out inv)) {
			return inv;
		} else {
			return registerNewPlayer(p);
		}
	}

	private Inventory registerNewPlayer(PlayerInfo p) {
		Inventory newInv = new Inventory (10);
		playerInventoryMap.Add (p, newInv);
		return newInv;
	}
}
