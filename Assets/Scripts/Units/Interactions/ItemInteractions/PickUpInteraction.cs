using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Item Interactions/Pick Up Interaction",
	fileName = "PickUpInteraction")]
public class PickUpInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		var itemOnGround = (ItemOnGround) owner;
		var (pickedUpAll, amountLeft) = Player.Instance.PickUpItem(itemOnGround.item, itemOnGround.amount);
		Debug.Log(pickedUpAll);
		Debug.Log(amountLeft);
		if (pickedUpAll)
		{
			itemOnGround.Die();
		}
		else
		{
			itemOnGround.amount = amountLeft;
		}

		return true;
	}
}