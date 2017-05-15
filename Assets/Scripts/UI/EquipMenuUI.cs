using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipMenuUI : MonoBehaviour {

	public ItemInventorySlotUI helmet;
	public ItemInventorySlotUI upperBody;
	public ItemInventorySlotUI lowerBody;
	public ItemInventorySlotUI boots;
	public ItemInventorySlotUI leftClaw;
	public ItemInventorySlotUI rightClaw;
	public ItemInventorySlotUI backpack;

	public bool opened;
	private Vector3 closedPosition;
	private Vector3 openPosition;

	public RectTransform trans;
	void Start() {
		trans = GetComponent<RectTransform> ();
		openPosition = trans.localPosition;
		closedPosition = openPosition - new Vector3 (2 * trans.rect.width, 0);
	}

	void Update() {
		if (opened) {
			trans.localPosition = Vector3.Lerp(trans.localPosition, openPosition, .2f);
		} else {
			trans.localPosition = Vector3.Lerp(trans.localPosition, closedPosition, .2f);
		}
	}

	public void updateUI(PlayerInfo info) {
		setBackpack (info.backpack);
		setHelmet (info.helmet);
		setUpperBody (info.upperBody);
		setLowerBody (info.lowerBody);
		setBoots (info.boots);
		setLeftClaw (info.leftClaw);
		setRightClaw (info.rightClaw);
	}
		
	public void setHelmet(HelmetItem equipment) {
		ItemInventorySlot slot = new ItemInventorySlot ();

		if (equipment == null) {
			slot.setSlot (null, 0);
		} else {
			slot.setSlot (equipment, 1);
		}
			
		this.helmet.setSlotBackingInfo (slot);
	}

	public void setUpperBody(UpperBodyItem equipment) {
		ItemInventorySlot slot = new ItemInventorySlot ();

		if (equipment == null) {
			slot.setSlot (null, 0);
		} else {
			slot.setSlot (equipment, 1);
		}

		this.upperBody.setSlotBackingInfo (slot);
	}

	public void setLowerBody(LowerBodyItem equipment) {
		ItemInventorySlot slot = new ItemInventorySlot ();

		if (equipment == null) {
			slot.setSlot (null, 0);
		} else {
			slot.setSlot (equipment, 1);
		}

		this.lowerBody.setSlotBackingInfo (slot);
	}

	public void setBoots(BootsItem equipment) {
		ItemInventorySlot slot = new ItemInventorySlot ();

		if (equipment == null) {
			slot.setSlot (null, 0);
		} else {
			slot.setSlot (equipment, 1);
		}

		this.boots.setSlotBackingInfo (slot);
	}

	public void setLeftClaw(ClawItem equipment) {
		ItemInventorySlot slot = new ItemInventorySlot ();

		if (equipment == null) {
			slot.setSlot (null, 0);
		} else {
			slot.setSlot (equipment, 1);
		}

		this.leftClaw.setSlotBackingInfo (slot);
	}

	public void setRightClaw(ClawItem equipment) {
		ItemInventorySlot slot = new ItemInventorySlot ();

		if (equipment == null) {
			slot.setSlot (null, 0);
		} else {
			slot.setSlot (equipment, 1);
		}

		this.rightClaw.setSlotBackingInfo (slot);
	}

	public void setBackpack(BackpackItem equipment) {
		ItemInventorySlot slot = new ItemInventorySlot ();

		if (equipment == null) {
			slot.setSlot (null, 0);
		} else {
			slot.setSlot (equipment, 1);
		}

		this.backpack.setSlotBackingInfo (slot);
	}




}
