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
	
	public List<ItemActionsUI.ItemActionType> accessibleActions = new List<ItemActionsUI.ItemActionType>
	{
		ItemActionsUI.ItemActionType.Drop,
		ItemActionsUI.ItemActionType.Use,
		ItemActionsUI.ItemActionType.Move
	};
}