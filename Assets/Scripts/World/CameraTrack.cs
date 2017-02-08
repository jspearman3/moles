using UnityEngine;
using System.Collections;

public class CameraTrack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject player = MoleController.localPlayer;

		if (player != null) {
			Transform playerTrans = player.GetComponent<Transform> ();
			GetComponent<Transform>().position = new Vector3(playerTrans.position.x, playerTrans.position.y, -1);
		}
			
	}
}
