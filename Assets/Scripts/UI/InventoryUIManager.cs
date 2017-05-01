using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour {

	public BackpackInventoryUI backpackUI;
	public EquipMenuUI equipUI;
	public BeltInventoryUI beltUI;

	bool opened = false;

	// Use this for initialization
	void Start () {
		setUIOpened ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space))
			toggleUIs ();
	}

	void toggleUIs() {
		opened = !opened;
		setUIOpened ();
	}

	void setUIOpened() {
		backpackUI.opened = opened;
		equipUI.opened = opened;
	}


}
