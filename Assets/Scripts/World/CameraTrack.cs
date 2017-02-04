using UnityEngine;
using System.Collections;

public class CameraTrack : MonoBehaviour {

	public Transform player;

	Transform trans;

	// Use this for initialization
	void Start () {
		trans = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 playerPos = player.position;
		trans.position = new Vector3 (playerPos.x, playerPos.y, trans.position.z);
	}
}
