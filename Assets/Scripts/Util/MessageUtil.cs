using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MessageUtil {

	public static MessageBase Read(MessageBase messageable, byte[] bytes) {
		NetworkReader reader = new NetworkReader (bytes);
		messageable.Deserialize (reader);
		return messageable;
	}

	public static NetworkWriter getWriter(MessageBase messageable) {
		NetworkWriter writer = new NetworkWriter ();
		messageable.Serialize (writer);
		return writer;
	}

	public static byte[] ToArray(MessageBase messageable) {
		return getWriter (messageable).ToArray ();
	}


}
