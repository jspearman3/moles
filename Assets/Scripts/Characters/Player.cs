using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	public PlayerInfo info;
	public string serverdebugtext;
	public string serverdebugtext2;
	NetworkIdentity identity;
	PlayerInfoManager playerInfoManager;
	public ItemBar itembar;

	// Use this for initialization
	void Start () {
		identity = GetComponent<NetworkIdentity> ();
	}

	[TargetRpc]
	public void TargetSetPlayerInfo(NetworkConnection conn, byte[] serverInfo) {
		NetworkReader reader = new NetworkReader (serverInfo);
		PlayerInfo newInfo = new PlayerInfo (null);
		newInfo.Deserialize (reader);
		this.info = newInfo;
		serverdebugtext2 = serverInfo + "poop"; 
	}

	[Command]
	void CmdFetchPlayerInfo() {
		PlayerInfoManager pim = GameObject.Find ("PlayerInfoManager").GetComponent<PlayerInfoManager>();
		PlayerInfo serverInfo = pim.getPlayerInfo(identity.connectionToClient);

		NetworkWriter writer = new NetworkWriter ();
		serverInfo.Serialize (writer);

		TargetSetPlayerInfo (identity.connectionToClient, writer.AsArray());
	}

	bool phase1Init = false;
	bool phase2Init = false;

	void Update() {
		if (!isLocalPlayer)
			return;

		if (!phase1Init) {
			CmdFetchPlayerInfo ();
			phase1Init = true;
			return;
		}

		if (!phase2Init) {
			if (info == null) {
				return;
			}

			itembar = GameObject.FindGameObjectWithTag ("ItemBar").GetComponent<ItemBar> ();
			itembar.loadInventory (info.belt);
			phase2Init = true;
		}
			
	}
	
	[TargetRpc]
	public void TargetPickUpItem(NetworkConnection conn, string itemCode) {
		Item dummy = new RockItem();
		info.belt.addItem (dummy.Decode (itemCode));
		itembar.updateUI ();
	}
}
