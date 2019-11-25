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
		AddItem("Sword", 0, 2);
		AddItem("Sword", 1, 5555);
	}

	private void AddItem(string itemName, int slotId, int itemCount)
	{
		AddItem(ItemManager.Instance.GetItemByName(itemName), slotId, itemCount);
	}

	private void AddItem(Item item, int slotId, int itemCount)
	{
		var itemExists = itemInformation.FirstOrDefault(i => i.item == item);
		if (itemExists == null)
		{
			itemInformation.Add(new ItemInformation(item,
				new List<SlotItemInformation> {new SlotItemInformation(slotId, itemCount)}));
			GameUI.Instance.uiInventory.AddNewItem(item, slotId, itemCount);
		}
		else
		{
			var slotItemInformationExists = itemExists.slotItemInformation.FirstOrDefault(si => si.slotId == slotId);
			if (slotItemInformationExists != null)
			{
				slotItemInformationExists.itemCount += itemCount;
				GameUI.Instance.uiInventory.UpdateItemCount(slotId, slotItemInformationExists.itemCount);
			}
			else
			{
				itemExists.slotItemInformation.Add(new SlotItemInformation(slotId, itemCount));
				GameUI.Instance.uiInventory.AddNewItem(item, slotId, itemCount);
			}
		}
	}

	public void ChangeSlots(InventorySlot draggedInventorySlot, InventorySlot slotToExchangeWith)
	{
		var oldSlotId = draggedInventorySlot.slotId;
		var newSlotId = slotToExchangeWith.slotId;
		var draggedItem = draggedInventorySlot.itemInside;
		var draggedSlotInformation = GetSlotItemInformation(GetItemInformation(draggedItem), oldSlotId);
		var draggedItemCount = draggedSlotInformation.itemCount;
		var slotToExchangeItem = slotToExchangeWith.itemInside;
		if (slotToExchangeItem)
		{
			var slotToExchangeInformation = GetSlotItemInformation(GetItemInformation(slotToExchangeItem), newSlotId);
			var slotToExchangeItemCount = slotToExchangeInformation.itemCount;
			slotToExchangeInformation.slotId = oldSlotId;
			draggedInventorySlot.Initialize(slotToExchangeItem, slotToExchangeItemCount);
		}
		else
		{
			draggedInventorySlot.Initialize();
		}

		draggedSlotInformation.slotId = newSlotId;
		slotToExchangeWith.Initialize(draggedItem, draggedItemCount);
	}

	private SlotItemInformation GetSlotItemInformation(Item item, int slotId)
	{
		var information = GetItemInformation(item);
		return GetSlotItemInformation(information, slotId);
	}

	private SlotItemInformation GetSlotItemInformation(ItemInformation information, int slotId)
	{
		var slotItemInformation = information.slotItemInformation.FirstOrDefault(si => si.slotId == slotId);
		return slotItemInformation;
	}

	private ItemInformation GetItemInformation(Item item)
	{
		return itemInformation.FirstOrDefault(i => i.item == item);
	}
}