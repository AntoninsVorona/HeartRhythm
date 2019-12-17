using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Inventory/Items/Custom/Headset", fileName = "Headset")]
public class Headset : Item
{
	public override List<ItemActionsUI.ItemActionType> AccessibleActions()
	{
		return new List<ItemActionsUI.ItemActionType>
		{
			ItemActionsUI.ItemActionType.Move
		};
	}
}