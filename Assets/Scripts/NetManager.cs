using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NetManager : NetworkManager {

	Dictionary<short, GameObject> playerMap = new Dictionary<short, GameObject>();

	public string address;
	public int port;
	public Text debug;

	// Use this for initialization
	void Start () {

		startAsHost();

	}

	override
	public void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		Debug.Log ("playerControllerId = " + playerControllerId + " logged in!");
		GameObject player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
		//playerMap.Add (playerControllerId, player);
	
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

	}


	override
	public void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController)
	{
		Debug.Log ("playerControllerId = " + playerController.playerControllerId + " disconnected!");
		GameObject player = playerMap[playerController.playerControllerId];

		GameObject.Destroy (player);

	}

	NetworkClient client;

	public void startAsHost() {
		client = this.StartHost ();
		client.RegisterHandler(MsgType.Connect, OnConnected);
		client.RegisterHandler(MsgType.Error, OnError);
		client.Connect (address, port);
	}
		

	public void startAsClient() {
		client = this.StartClient ();
		client.RegisterHandler(MsgType.Connect, OnClientConnected);
		client.RegisterHandler(MsgType.Error, OnError);
		client.Connect (address, port);
	}

	public void OnConnected(NetworkMessage message) {
		Debug.Log("Connected to server. " + client.isConnected);


		ClientScene.AddPlayer(message.conn, 1);

	}

	public void OnClientConnected(NetworkMessage message) {
		Debug.Log("Connected to server. " + client.isConnected);


		ClientScene.AddPlayer(message.conn, 2);

	}

	public void OnError(NetworkMessage message) {
		Debug.Log("Error. " + message.ToString());



	}


	// Update is called once per frame
	void Update () {
		
	}
}
