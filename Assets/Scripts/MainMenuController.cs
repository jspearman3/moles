using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
	MolesClient molesClient;
	public InputField usernameField;
	public InputField addressField;
	public InputField portField;
	public Text errorText;


	void Start() {
        molesClient = GameObject.Find("MolesClient").GetComponent<MolesClient>();
    }

	public void joinGame() {
		string address = addressField.text;
		ushort port = ushort.Parse(portField.text);
		string username = usernameField.text;

		if (username.Length < 3) {
			errorText.text = "Error. Username must be at least 3 characters.";
			return;
		}

		if (!(System.Text.RegularExpressions.Regex.IsMatch (username, @"^[a-zA-Z0-9]+$"))) {
			errorText.text = "Error. Username must contain alphanumeric characters only.";
			return;
		}

		Debug.Log ("attempting connection with " + address + " on port " + port + "...");
		molesClient.connect (address, port, username);
	}


}
