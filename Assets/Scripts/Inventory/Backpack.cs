using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Inventory/Backpacks/Backpack", fileName = "Backpack")]
public class Backpack : ScriptableObject
{
	public int amountOfItems;
}