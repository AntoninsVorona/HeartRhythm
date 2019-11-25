using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Inventory/Items/Item", fileName = "Item")]
public class Item : ScriptableObject
{
	public string itemName;
	public int maxStackCount = 1;
	public Sprite spriteIcon;
}