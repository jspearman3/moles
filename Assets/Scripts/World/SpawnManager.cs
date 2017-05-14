using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

	public GameObject itemPrefab;

	public const int MAX_SPAWNED_ITEMS = 100;
	public const float RANDOM_SPAWN_AMPLITUDE = 0.4f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnItem(Item item, int quantity, GamePosition position) {
		if (quantity <= 0)
			return;
		GameObject itemGo = GameObject.Instantiate (itemPrefab);
		ItemEntity ie = itemGo.GetComponent<ItemEntity> ();
		ie.gamePos = position;
		ie.setIdentity(item);
		ie.syncPos = ie.gamePos.toStruct ();
		ie.setQuantity (quantity);
		NetworkServer.Spawn (itemGo);
	}

	public void SpawnPlayerDroppedItem(Item item, int quantity, MoleController playerController) {
		GamePosition spawnPosition = playerController.gamePos.add (DirectionUtil.getDirectionUnitVector (playerController.facing) * Player.DROP_DISTANCE);
		spawnPosition.descend (-0.5f);
		SpawnItem(item, quantity, spawnPosition);
	}

	public void SpawnItemFromTile(Item item, int quantity, GamePosition position) {
		GameObject itemGo = GameObject.Instantiate (itemPrefab);
		ItemEntity ie = itemGo.GetComponent<ItemEntity> ();
		ie.gamePos = new GamePosition (position.planePosition + Random.insideUnitCircle * RANDOM_SPAWN_AMPLITUDE, position.depth - 0.5f);
		ie.setIdentity(item);
		ie.syncPos = ie.gamePos.toStruct ();
		ie.setQuantity (quantity);
		NetworkServer.Spawn (itemGo);

	}




}
