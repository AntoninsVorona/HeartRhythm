using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	private const string BACKPACK_PATH = "Backpacks/";

	[Serializable]
	public class InventoryData
	{
		public string backpackName;
		public List<Inventory.ItemInformation> inventoryData;

		public InventoryData(string backpackName, List<ItemInformation> inventoryData)
		{
			this.backpackName = backpackName;
			this.inventoryData = inventoryData;
		}
	}

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

	private Backpack currentBackpack;
	private List<ItemInformation> itemInformation;

	public void Initialize()
	{
		GameUI.Instance.uiInventory.InitializeSlots(currentBackpack);
		GameUI.Instance.uiInventory.InitializeItems(itemInformation);
	}

	public void LoadData(InventoryData data)
	{
		if (data != null)
		{
			itemInformation = data.inventoryData ?? new List<ItemInformation>();
			currentBackpack = string.IsNullOrEmpty(data.backpackName)
				? null
				: Resources.Load<Backpack>($"{BACKPACK_PATH}{data.backpackName}");
		}
		else
		{
			currentBackpack = null;
			itemInformation = new List<ItemInformation>();
		}
	}

	public InventoryData GetData()
	{
		var backPackName = currentBackpack ? currentBackpack.identifierName : null;
		return new InventoryData(backPackName, itemInformation);
	}

	public (bool pickedUpAll, int amountLeft) PickUpItem(Item item, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError("Can't pickup less than or equals to 0 items!");
			return (false, 0);
		}

		var (pickedUpAll, amountCanBePicked) = CanPickUpItem(item, amount);

		AddItem(item, amountCanBePicked);

		return (pickedUpAll, amount - amountCanBePicked);
	}

	public bool DropItem(InventorySlot slot, int amount)
	{
		return DropItem(slot.itemInside, slot.slotId, amount);
	}

	public bool DropItem(Item item, int slotId, int amount)
	{
		if (amount <= 0)
		{
			Debug.LogError("Can't drop less than or equals to 0 items!");
			return false;
		}

		return RemoveItem(item, slotId, amount);
	}

	public void LoseItem(Item item, int amount)
	{
		var information = GetItemInformation(item);
		var sum = information.slotItemInformation.Sum(slotItemInformation => slotItemInformation.itemCount);
		if (sum >= amount)
		{
			var toDrop = amount;
			foreach (var slotItemInformation in information.slotItemInformation)
			{
				if (slotItemInformation.itemCount < toDrop)
				{
					RemoveItem(item, slotItemInformation.slotId, slotItemInformation.itemCount);
					toDrop -= slotItemInformation.itemCount;
				}
				else
				{
					RemoveItem(item, slotItemInformation.slotId, toDrop);
					break;
				}
			}
		}
		else
		{
			Debug.LogError("Can't lose more than there are!");
		}
	}

	public bool UseItem(InventorySlot slot)
	{
		return RemoveItem(slot.itemInside, slot.slotId, 1);
	}

	public (bool canPickUpAll, int amount) CanPickUpItem(Item item, int amount)
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

	private bool RemoveItem(string itemName, int slotId, int itemCount)
	{
		return RemoveItem(ItemManager.Instance.GetItemByName(itemName), slotId, itemCount);
	}

	private bool RemoveItem(Item item, int slotId, int itemCount)
	{
		var itemExists = itemInformation.FirstOrDefault(i => i.item == item);
		if (itemExists != null)
		{
			var slotItemInformationExists = itemExists.slotItemInformation.FirstOrDefault(si => si.slotId == slotId);
			if (slotItemInformationExists != null)
			{
				slotItemInformationExists.itemCount -= itemCount;
				if (slotItemInformationExists.itemCount > 0)
				{
					GameUI.Instance.uiInventory.UpdateItemCount(slotId, slotItemInformationExists.itemCount);
					return false;
				}

				itemExists.slotItemInformation.Remove(slotItemInformationExists);
				GameUI.Instance.uiInventory.RemoveItem(slotId);
				return true;
			}

			Debug.LogError($"Can't remove {item.name} from slot {slotId} because it doesn't exist there!");
		}
		else
		{
			Debug.LogError($"Can't remove {item.name} because it doesn't exist in the inventory!");
		}

		return false;
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
			var slotToExchangeInformation =
				GetSlotItemInformation(GetItemInformation(slotToExchangeItem), newSlotId);

			if (draggedItem == slotToExchangeItem)
			{
				var canAdd = draggedItem.maxStackCount - slotToExchangeInformation.itemCount;
				if (canAdd >= draggedItemCount)
				{
					var information = GetItemInformation(draggedItem);
					information.slotItemInformation.Remove(draggedSlotInformation);
					slotToExchangeInformation.itemCount += draggedItemCount;
					draggedInventorySlot.Initialize();
				}
				else
				{
					draggedSlotInformation.itemCount -= canAdd;
					slotToExchangeInformation.itemCount = draggedItem.maxStackCount;
					draggedInventorySlot.UpdateItemAmount(draggedSlotInformation.itemCount);
				}

				slotToExchangeWith.UpdateItemAmount(slotToExchangeInformation.itemCount);
			}
			else
			{
				var slotToExchangeItemCount = slotToExchangeInformation.itemCount;
				slotToExchangeInformation.slotId = oldSlotId;
				draggedInventorySlot.Initialize(slotToExchangeItem, slotToExchangeItemCount);
				draggedSlotInformation.slotId = newSlotId;
				slotToExchangeWith.Initialize(draggedItem, draggedItemCount);
			}
		}
		else
		{
			draggedInventorySlot.Initialize();
			draggedSlotInformation.slotId = newSlotId;
			slotToExchangeWith.Initialize(draggedItem, draggedItemCount);
		}
	}

	public void SplitItem(InventorySlot slotFrom, InventorySlot slotTo, int amount)
	{
		var item = slotFrom.itemInside;
		var slotFromInformation = GetSlotItemInformation(item, slotFrom.slotId);
		slotFromInformation.itemCount -= amount;
		slotFrom.Initialize(item, slotFromInformation.itemCount);
		AddItem(item, slotTo.slotId, amount);
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

	public int ItemsInSlot(InventorySlot slot)
	{
		return GetSlotItemInformation(slot.itemInside, slot.slotId).itemCount;
	}
}