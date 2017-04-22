using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoginMessage : MessageBase
{
	public string username;

	public LoginMessage() {
		this.username = "";
	}

	public LoginMessage(string username) {
		this.username = username;
	}

	public override void Deserialize(NetworkReader reader)
	{
		username = System.Text.Encoding.Default.GetString (reader.ReadBytesAndSize ());
	}

	// This method would be generated
	public override void Serialize(NetworkWriter writer)
	{
		writer.WriteBytesFull (System.Text.Encoding.Default.GetBytes (username));
	}
}
