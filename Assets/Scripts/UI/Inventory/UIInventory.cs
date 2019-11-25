﻿using System.Collections.Generic;
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
		return Instantiate(backpackSlotPrefab, backpackSlotHolder.transform);
	}

	public void RemoveSlot(BackpackSlot slot)
	{
		inventorySlots.Remove(slot);
		Destroy(slot.gameObject);
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
}