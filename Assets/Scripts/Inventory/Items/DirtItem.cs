using UnityEngine;

public class DirtItem : Item {
	public DirtItem() {
		Sprite s = Resources.Load<Sprite> ("Textures/Items/dirtIcon");
		this.gameSprite = s;
		this.icon = s;
	}
}