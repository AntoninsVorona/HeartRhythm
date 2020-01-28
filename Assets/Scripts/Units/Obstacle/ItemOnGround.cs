using System;
using UnityEngine;

public class ItemOnGround : Obstacle
{
	[Serializable]
	public class ItemOnGroundData : UnitData
	{
		public int amount;

		public ItemOnGroundData(string identifierName, Vector2Int currentPosition, int amount) : base(identifierName,
			currentPosition)
		{
			this.amount = amount;
		}
	}

	public Item item;
	public int amount = 1;

	public override void Initialize(Vector2Int location)
	{
		base.Initialize(location);
		sprite.sprite = item.spriteIcon;
	}

	public override UnitData GetUnitData()
	{
		return new ItemOnGroundData(item.itemName, currentPosition, amount);
	}
}