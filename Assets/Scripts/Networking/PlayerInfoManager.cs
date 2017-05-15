using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInfoManager : NetworkBehaviour {

	Dictionary<string,PlayerInfo> usernameToPlayerInfo;
	List<PlayerInfo> loggedInPlayers;

	void Start() {
		usernameToPlayerInfo = new Dictionary<string, PlayerInfo> ();
		loggedInPlayers = new List<PlayerInfo> ();

		BeltInventory b = new BeltInventory (10);
		b.AddItemManyToSlot (new RockItem (), 42, 5);
		PlayerInfo p = new PlayerInfo (b);
		usernameToPlayerInfo.Add ("chip", p);
	}

	public PlayerInfo getPlayerInfo(NetworkConnection conn) {
		return conn.playerControllers [1].gameObject.GetComponent<Player> ().info;
	}

	public void userLogin(string username, GameObject player) {
		PlayerInfo playerInfo;


		if (!usernameToPlayerInfo.TryGetValue (username, out playerInfo)) {
			playerInfo = new PlayerInfo (new BeltInventory(10));
			playerInfo.cursorSlot = new ItemInventorySlot ();
			usernameToPlayerInfo.Add (username, playerInfo);


		}

		MoleController controller = player.GetComponent<MoleController> ();
		if (playerInfo.lastLogoutPos == null) {
			//first time logging in. spawn at default position
			controller.syncPos = new GamePosition (new Vector2 (3.5f, 3.5f), 1).toStruct ();
		} else {
			controller.syncPos = playerInfo.lastLogoutPos.toStruct ();
		}

		Player playerplayer = player.GetComponent<Player> ();
		playerplayer.info = playerInfo;
	}

	public void userLogout(NetworkConnection conn) {
		GameObject playerObj = conn.playerControllers [1].gameObject;
		PlayerInfo info = playerObj.GetComponent<Player> ().info;
		info.lastLogoutPos = playerObj.GetComponent<MoleController> ().gamePos;
	}


	public void ValidateAndPerformInventoryRequest(InventoryOperationRequest request, Player p) {
		if (InventoryOperationRequest.validateRequest (request, p.info)) {
			InventoryOperationRequest.performInventoryRequest (request, p);
		}
	}

}
