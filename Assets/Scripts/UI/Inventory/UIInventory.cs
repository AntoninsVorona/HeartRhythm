using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UIInventory : MonoBehaviour
{
	private const float TIME_HOLD_TO_DRAG = 0.1f;

	[HideInNormalInspector]
	public bool open;

	[SerializeField]
	private DragController dragController;

	[SerializeField]
	private ItemActionsUI itemActionsUI;

	[SerializeField]
	private List<InventorySlot> inventorySlots;

	[SerializeField]
	private Transform backpackSlotHolder;

	[SerializeField]
	private GameObject backpackSlotsObject;

	private InventorySlot selectedInventorySlot;

	private float dragTime;

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

		foreach (var slot in inventorySlots)
		{
			slot.Deselect();
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

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var slotHit = GetSlotHit();
			if (slotHit && slotHit.itemInside)
			{
				itemActionsUI.Close();
				SelectInventorySlot(slotHit);
				dragTime = Time.time + TIME_HOLD_TO_DRAG;
			}
			else
			{
				dragTime = float.MaxValue;
				if (!ItemActionsHit())
				{
					itemActionsUI.Close();
					DeselectInventorySlot();
				}
			}
		}
		else
		{
			if (Input.GetMouseButton(0))
			{
				if (Time.time >= dragTime)
				{
					if (dragController.draggedInventorySlot)
					{
						dragController.OnDrag();
					}
					else
					{
						itemActionsUI.Close();
						dragController.BeginDrag(selectedInventorySlot);
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (dragController.draggedInventorySlot)
				{
					var swappedPlaces = dragController.OnEndDrag();
					if (swappedPlaces)
					{
						DeselectInventorySlot();
					}
				}
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			if (dragController.draggedInventorySlot)
			{
				dragController.StopDrag();
				dragTime = float.MaxValue;
			}
			else
			{
				var slotHit = GetSlotHit();
				if (slotHit && slotHit.itemInside)
				{
					SelectInventorySlot(slotHit);
					itemActionsUI.OpenActionsFor(slotHit);
				}
			}
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
		itemActionsUI.Close();
		gameObject.SetActive(true);
		DeselectInventorySlot();
		open = true;
	}

	public void Close()
	{
		itemActionsUI.Close();
		dragController.StopDrag();
		gameObject.SetActive(false);
		DeselectInventorySlot();
		open = false;
	}

	private void SelectInventorySlot(InventorySlot inventorySlot)
	{
		if (inventorySlot)
		{
			DeselectInventorySlot();
			selectedInventorySlot = inventorySlot;
			selectedInventorySlot.Select();
		}
	}

	private void DeselectInventorySlot()
	{
		if (selectedInventorySlot)
		{
			selectedInventorySlot.Deselect();
			selectedInventorySlot = null;
		}
	}

	public void AddNewItem(Item item, int slotId, int itemCount)
	{
		GetSlotById(slotId).Initialize(item, itemCount);
	}

	public void RemoveItem(int slotId)
	{
		GetSlotById(slotId).Initialize();
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

	public static InventorySlot GetSlotHit()
	{
		var inputModule = (CustomStandaloneInputModule) EventSystem.current.currentInputModule;
		if (inputModule.IsPointerOverUI())
		{
			return inputModule.CurrentHit().GetComponentInParent<InventorySlot>();
		}

		return null;
	}

	public static bool ItemActionsHit()
	{
		var inputModule = (CustomStandaloneInputModule) EventSystem.current.currentInputModule;
		if (inputModule.IsPointerOverUI())
		{
			return inputModule.CurrentHit().GetComponentInParent<ItemActionsUI>();
		}

		return false;
	}

	public void DropActionPressed(int input)
	{
		if (selectedInventorySlot)
		{
			if (input > 0)
			{
				var droppedAll = Player.Instance.DropItem(selectedInventorySlot, input);
				if (droppedAll)
				{
					DeselectInventorySlot();
				}
			}
		}
		else
		{
			Debug.LogError("No Selected Slot!");
		}
	}
}