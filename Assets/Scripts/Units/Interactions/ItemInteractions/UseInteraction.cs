using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Item Interactions/Use Interaction",
	fileName = "UseInteraction")]
public class UseInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		var itemOnGround = (ItemOnGround) owner;
		Player.Instance.UseItem((Consumable) itemOnGround.item);
		itemOnGround.amount -= 1;
		if (itemOnGround.amount == 0)
		{
			itemOnGround.Die();
		}

		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}