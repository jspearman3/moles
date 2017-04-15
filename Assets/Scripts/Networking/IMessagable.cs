using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMessagable<T> {
	string Encode();

	T Decode (string s);

}
