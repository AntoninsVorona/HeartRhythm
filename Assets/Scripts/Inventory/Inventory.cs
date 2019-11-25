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
		GameUI.Instance.uiInventory.InitializeSlots(currentBackpack);
		LoadItemInformation();
		GameUI.Instance.uiInventory.InitializeItems(itemInformation);
	}

	private void LoadItemInformation()
	{
		itemInformation = new List<ItemInformation>();
//		AddItem("Sword", 0, 1);
//		AddItem("Sword", 1, 5555);
	}

	public (bool pickedUpAll, int amountLeft) PickUpItem(Item item, int amount)
	{
		var (pickedUpAll, amountCanBePicked) = CanPickUpItem(item, amount);

		AddItem(item, amountCanBePicked);

		return (pickedUpAll, amount - amountCanBePicked);
	}

	private (bool canPickUpAll, int amount) CanPickUpItem(Item item, int amount)
	{
		var information = GetItemInformation(item);
		var canAdd = 0;
		if (information != null)
		{
			var canAddToExisting = information.slotItemInformation.Sum(slotItemInformation =>
				item.maxStackCount - slotItemInformation.itemCount);
			if (canAddToExisting >= amount)
			{
				return (true, amount);
			}

			canAdd = canAddToExisting;
		}

		canAdd += FreeSlotsAmount() * item.maxStackCount;
		return canAdd >= amount ? (true, amount) : (false, canAdd);
	}

	private void AddItem(Item item, int itemCount)
	{
		var information = GetItemInformation(item);
		if (information != null)
		{
			foreach (var slotItemInformation in information.slotItemInformation)
			{
				var canAdd = item.maxStackCount - slotItemInformation.itemCount;
				if (canAdd >= itemCount)
				{
					AddItem(item, slotItemInformation.slotId, itemCount);
					return;
				}

				AddItem(item, slotItemInformation.slotId, canAdd);
				itemCount -= canAdd;
			}
		}

		while (itemCount > 0)
		{
			var freeSlot = GameUI.Instance.uiInventory.GetFreeSlot();
			if (!freeSlot)
			{
				Debug.LogError($"Something is wrong. Not enough space to add item {item.itemName}!");
				return;
			}

			AddItem(item, freeSlot.slotId, itemCount - item.maxStackCount > 0 ? item.maxStackCount : itemCount);
			itemCount -= item.maxStackCount;
		}
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
		var draggedSlotInformation = GetSlotItemInformation(draggedItem, oldSlotId);
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

	private int FreeSlotsAmount()
	{
		return GameUI.Instance.uiInventory.FreeSlotsAmount();
	}
}