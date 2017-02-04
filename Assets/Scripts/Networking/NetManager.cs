using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class NetManager : NetworkManager {

	Dictionary<short, GameObject> playerMap = new Dictionary<short, GameObject>();

	public Text debug;
	public string hostScene;
	public string clientScene;

	// Use this for initialization
	void Start () {
      
		//startAsHost();

	}

	public static NetManager getInstance() {
		return GameObject.Find ("NetworkManager").GetComponent<NetManager>();
	}



	//////////////////////
	// Connection Logic //
	//////////////////////

	public void startAsHost(int port) {
		Connect (null, port);
	}


	public void startAsClient(string address, int port) {
		Connect (address, port);
	}
		

	NetworkClient client;
	void Connect(string address, int port) {

		if (address != null && port > 0) {
			client = this.StartClient();

		} else {
			address = this.networkAddress;
			this.networkPort = port;
			client = this.StartHost ();
		}

		registerHandlers (client);
		client.Connect (address, port);

	}




	////////////////////////////
	// Client/Server Messages //
	////////////////////////////
	private void registerHandlers(NetworkClient client) {
		client.RegisterHandler(MsgType.Connect, OnConnected);
		client.RegisterHandler(MsgType.Error, OnError);
	}

	override
	public void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
	{
		Debug.Log ("playerControllerId = " + playerControllerId + " logged in!");
		GameObject player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector3(3.5f,-3.5f,0), Quaternion.identity);
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





	public void OnConnected(NetworkMessage message) {
		Debug.Log("Connected to server1. " + client.isConnected);


		StartCoroutine(loadIn(.1f, message));

	}

	IEnumerator loadIn(float waitTime, NetworkMessage message) {
		SceneManager.LoadScene (hostScene.ToString());
		yield return new WaitForSeconds(waitTime);
		ClientScene.AddPlayer(message.conn, 1);
	}

	public void OnClientConnected(NetworkMessage message) {
		Debug.Log("Connected to server2. " + client.isConnected);


		ClientScene.AddPlayer(message.conn, 2);

	}

	public void OnError(NetworkMessage message) {
		Debug.Log("Error. " + message.ToString());

	}


	// Update is called once per frame
	void Update () {

	}
}
