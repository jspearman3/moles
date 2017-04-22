using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	NetManager networkManager;
	public InputField usernameField;
	public InputField addressField;
	public InputField portField;
	public Text errorText;


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
		string username = usernameField.text;

		if (username.Length < 3) {
			errorText.text = "Error. Username must be at least 3 characters.";
			return;
		}

		if (!(System.Text.RegularExpressions.Regex.IsMatch (username, @"^[a-zA-Z0-9]+$"))) {
			errorText.text = "Error. Username must contain alphanumeric characters only.";
			return;
		}

		if (clientType.Equals("client")) {
			Debug.Log ("attempting connection with " + address + " on port " + port + "...");
			networkManager.startAsClient (address, port, username);
		} else {
			Debug.Log ("starting server on port " + port + "...");
			networkManager.startAsHost (port, username);
		}
	}


}
