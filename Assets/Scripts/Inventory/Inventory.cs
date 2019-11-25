using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	[Serializable]
	public class ItemInformation
	{
		public Item item;
		public List<SlotItemInformation> slotItemInformation;

		public ItemInformation(Item item, List<SlotItemInformation> slotItemInformation)
		{
			this.item = item;
			this.slotItemInformation = slotItemInformation;
		}
	}

	[Serializable]
	public class SlotItemInformation
	{
		public int slotId;
		public int itemCount;

		public SlotItemInformation(int slotId, int itemCount)
		{
			this.slotId = slotId;
			this.itemCount = itemCount;
		}
	}

	[SerializeField]
	private Backpack currentBackpack;

	private List<ItemInformation> itemInformation;

	public void Initialize()
	{
		LoadItemInformation();
		GameUI.Instance.uiInventory.InitializeSlots(currentBackpack);
		GameUI.Instance.uiInventory.InitializeItems(itemInformation);
	}

	private void LoadItemInformation()
	{
		itemInformation = new List<ItemInformation>();
		itemInformation.Add(new ItemInformation(ItemManager.Instance.GetItemByName("Sword"),
			new List<SlotItemInformation> {new SlotItemInformation(0, 2)}));
	}
}