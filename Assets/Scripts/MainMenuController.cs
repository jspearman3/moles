using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	NetManager networkManager;
	public InputField addressField;
	public InputField portField;


	void Start() {
		networkManager = NetManager.getInstance ();
	}

	public void StartAsHost() {
		startGame ("host");
	}

	public void StartAsClient() {
		startGame ("client");
	}

	private void startGame(string clientType) {
		string address = addressField.text;
		int port = int.Parse(portField.text);


		if (clientType.Equals("client")) {
			Debug.Log ("attempting connection with " + address + " on port " + port + "...");
			networkManager.startAsClient (address, port);
		} else {
			Debug.Log ("starting server on port " + port + "...");
			networkManager.startAsHost (port);
		}
	}


}
