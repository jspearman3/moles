using UnityEngine;

public class RockItem : Item {
	public RockItem() {
		Sprite s = Resources.Load<Sprite> ("Textures/Items/rockIcon");
		this.gameSprite = s;
		this.icon = s;
	}
}
