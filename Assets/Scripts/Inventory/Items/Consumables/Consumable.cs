using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Consumable : Item
{
	public override List<ItemActionsUI.ItemActionType> AccessibleActions()
	{
		return new List<ItemActionsUI.ItemActionType>
		{
			ItemActionsUI.ItemActionType.Move,
			ItemActionsUI.ItemActionType.Use,
			ItemActionsUI.ItemActionType.Drop
		};
	}

	public abstract void ApplyEffect();
}