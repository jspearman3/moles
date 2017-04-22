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
	public GameObject mapPrefab;
	public TiledMap serverMap;

	private PlayerInfoManager playerInfoManager;
	private string username;

	// Use this for initialization
	void Start () {
		playerInfoManager = GetComponentInChildren<PlayerInfoManager> ();
	}

	public static NetManager getInstance() {
		return GameObject.Find ("NetworkManager").GetComponent<NetManager>();
	}



	//////////////////////
	// Connection Logic //
	//////////////////////

	public void startAsHost(int port, string username) {
		Connect (null, port, username);
	}


	public void startAsClient(string address, int port, string username) {
		Connect (address, port, username);
	}
		

	NetworkClient client;
	void Connect(string address, int port, string username) {
		this.username = username;

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

	override
	public void OnStartHost() {
		StartCoroutine (initializeHost (0.1f));
	}

	IEnumerator initializeHost(float waitTime) {

		SceneManager.LoadScene (hostScene.ToString());

		yield return new WaitForSeconds(waitTime);

		GameObject map = (GameObject) Instantiate (mapPrefab, Vector3.zero, Quaternion.identity);
		serverMap = map.GetComponent<TiledMap> ();
		serverMap.mapData = MapData.buildDefaultMap ();
		NetworkServer.Spawn (map);
	}

	////////////////////////////
	// Client/Server Messages //
	////////////////////////////
	private void registerHandlers(NetworkClient client) {
		client.RegisterHandler(MsgType.Connect, OnConnected);
		client.RegisterHandler(MsgType.Error, OnError);
	}

	override
	public void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
	{
		LoginMessage login = new LoginMessage ();
		login.Deserialize (extraMessageReader);

		Debug.Log ("playerControllerId = " + login.username + " with player id " + playerControllerId + " logged in!");
		GameObject player = (GameObject)GameObject.Instantiate(playerPrefab);
		playerInfoManager.userLogin (login.username, player);

		NetworkWriter writer = new NetworkWriter ();
		serverMap.mapData.Serialize (writer);
		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
		serverMap.TargetSetAndApplyMap (conn, writer.AsArray ());
	}


	override
	public void OnServerRemovePlayer(NetworkConnection conn, PlayerController playerController)
	{
		Debug.Log ("playerControllerId = " + playerController.playerControllerId + " disconnected!");
		GameObject player = playerMap[playerController.playerControllerId];

		GameObject.Destroy (player);

	}

	override
	public void OnServerDisconnect(NetworkConnection conn) {
		playerInfoManager.userLogout (conn);
		NetworkServer.Destroy (conn.playerControllers [1].gameObject);
	}


	public void OnConnected(NetworkMessage message) {
		Debug.Log("Connected to server1. " + client.isConnected);

		StartCoroutine(loadIn(.1f, message));

	}

	IEnumerator loadIn(float waitTime, NetworkMessage message) {

		SceneManager.LoadScene (hostScene.ToString());

		yield return new WaitForSeconds(waitTime);

		LoginMessage login = new LoginMessage (username);
		ClientScene.AddPlayer(message.conn, 1, login);
	}

	public void OnClientConnected(NetworkMessage message) {
		Debug.Log("Connected to server2. " + client.isConnected);

		LoginMessage login = new LoginMessage (username);
		ClientScene.AddPlayer(message.conn, 2, login);

	}

	public void OnError(NetworkMessage message) {
		Debug.Log("Error. " + message.ToString());

	}


	// Update is called once per frame
	void Update () {

	}
}
