using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class Chat : NetworkBehaviour {
	public Text serverChat;
	public InputField input;
	public Button sendButton;
	public GameObject panel;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			panel.SetActive (!panel.activeSelf);
		}
	}

	public void sendMessage() {
		string message = input.text;
		input.text = "";
		if (!isServer) {
			CmdSend ("Bob", message);
		} else {
			RpcUpdateChat("Bob", message);
		}
	}

	[Command]
	private void CmdSend(string player, string message) {
		RpcUpdateChat (player, message);
	}

	private void RpcUpdateChat(string player, string message) {
		string formattedMsg = string.Format ("[{0}] - {1}\n", player, message);
		serverChat.text = serverChat.text + formattedMsg; 
	}
}
