using System;
using UnityEngine;

public class ItemOnGround : Obstacle
{
	public Item item;
	public int amount = 1;

	public override void Initialize(Vector2Int location)
	{
		base.Initialize(location);
		sprite.sprite = item.spriteIcon;
	}
}