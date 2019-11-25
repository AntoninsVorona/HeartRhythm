using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class UIInventory : MonoBehaviour
{
	[HideInNormalInspector]
	public bool open;

	[SerializeField]
	private List<InventorySlot> inventorySlots;

	[SerializeField]
	private Transform backpackSlotHolder;

	[SerializeField]
	private GameObject backpackSlotsObject;

	[FormerlySerializedAs("backPackSlotPrefab")]
	[Header("Prefabs")]
	[SerializeField]
	private BackpackSlot backpackSlotPrefab;

	public void InitializeSlots(Backpack currentBackpack)
	{
		if (currentBackpack.amountOfItems > 0)
		{
			backpackSlotsObject.SetActive(true);
			var backPackSlots = inventorySlots.Count(i => i is BackpackSlot);
			for (var i = backPackSlots; i < currentBackpack.amountOfItems; i++)
			{
				var slot = AddBackpackSlot();
				slot.Initialize();
				slot.slotId = inventorySlots.Count - 1;
			}
		}
		else
		{
			backpackSlotsObject.SetActive(false);
		}
	}

	public void InitializeItems(List<Inventory.ItemInformation> itemInformation)
	{
		foreach (var information in itemInformation)
		{
			information.slotItemInformation.ForEach(si =>
			{
				var inventorySlot = inventorySlots.First(i => i.slotId == si.slotId);
				inventorySlot.Initialize(information.item, si.itemCount);
			});
		}

		foreach (var noItemSlot in inventorySlots.Where(i => !i.itemInside))
		{
			noItemSlot.Initialize();
		}
	}

	public BackpackSlot AddBackpackSlot()
	{
		var backpackSlot = Instantiate(backpackSlotPrefab, backpackSlotHolder.transform);
		inventorySlots.Add(backpackSlot);
		return backpackSlot;
	}

	public void Toggle()
	{
		if (open)
		{
			Close();
		}
		else
		{
			Open();
		}
	}

	public void Open()
	{
		gameObject.SetActive(true);
		open = true;
	}

	public void Close()
	{
		gameObject.SetActive(false);
		open = false;
	}

	public void AddNewItem(Item item, int slotId, int itemCount)
	{
		GetSlotById(slotId).Initialize(item, itemCount);
	}

	public void UpdateItemCount(int slotId, int itemCount)
	{
		GetSlotById(slotId).UpdateItemAmount(itemCount);
	}

	public InventorySlot GetSlotById(int slotId)
	{
		return inventorySlots.FirstOrDefault(i => i.slotId == slotId);
	}

	public int FreeSlotsAmount()
	{
		return inventorySlots.Count(i => !i.itemInside);
	}

	public InventorySlot GetFreeSlot()
	{
		var freeSlots = inventorySlots.Where(i => !i.itemInside).ToList();
		if (freeSlots.Count == 0)
		{
			return null;
		}

		var backpackSlots = freeSlots.Where(i => i is BackpackSlot).OrderBy(i => i.slotId).ToList();
		if (backpackSlots.Count > 0)
		{
			return backpackSlots.First();
		}

		var pocketSlots = freeSlots.Where(i => i is PocketSlot).OrderBy(i => i.slotId).ToList();
		if (pocketSlots.Count > 0)
		{
			return pocketSlots.First();
		}

		var handSlots = freeSlots.Where(i => i is HandSlot).OrderBy(i => i.slotId).ToList();
		return handSlots.First();
	}
}