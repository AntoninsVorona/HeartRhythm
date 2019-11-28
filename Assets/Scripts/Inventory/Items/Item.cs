using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Inventory/Items/Item", fileName = "Item")]
public class Item : ScriptableObject
{
	public string itemName;
	public int maxStackCount = 1;
	public Sprite spriteIcon;

	public virtual List<ItemActionsUI.ItemActionType> AccessibleActions()
	{
		return new List<ItemActionsUI.ItemActionType>
		{
			ItemActionsUI.ItemActionType.Move,
			ItemActionsUI.ItemActionType.Drop
		};
	}
}