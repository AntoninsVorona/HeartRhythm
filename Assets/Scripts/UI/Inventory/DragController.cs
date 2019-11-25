using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
	[Header("Dragging")]
	private InventorySlot draggedInventorySlot;

	[SerializeField]
	private Image draggedItemImage;

	[SerializeField]
	private RectTransform draggedItemTransform;
	
	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			OnBeginDrag();
		}

		if (draggedInventorySlot)
		{
			if (Input.GetMouseButton(0))
			{
				OnDrag();
			}
			else if (Input.GetMouseButtonUp(0))
			{
				OnEndDrag();
			}
		}
	}

	private void OnBeginDrag()
	{
		var slotHit = GetSlotHit();
		if (slotHit && slotHit.itemInside)
		{
			draggedInventorySlot = slotHit;
			draggedItemTransform.gameObject.SetActive(true);
			draggedItemImage.sprite = draggedInventorySlot.itemInside.spriteIcon;
			draggedItemTransform.anchoredPosition = Input.mousePosition;
			draggedInventorySlot.DisableIcon();
		}
	}

	private InventorySlot GetSlotHit()
	{
		var inputModule = (CustomStandaloneInputModule) EventSystem.current.currentInputModule;
		if (inputModule.IsPointerOverUI())
		{
			return inputModule.CurrentHit().GetComponent<InventorySlot>();
		}

		return null;
	}

	private void OnDrag()
	{
		draggedItemTransform.position = Input.mousePosition;
	}

	private void OnEndDrag()
	{
		var slotHit = GetSlotHit();
		if (slotHit)
		{
			DraggedIntoSlot(slotHit);
		}

		draggedInventorySlot.EnableIcon();
		draggedInventorySlot = null;
		draggedItemTransform.gameObject.SetActive(false);
	}

	private void DraggedIntoSlot(InventorySlot slotHit)
	{
		Player.Instance.inventory.ChangeSlots(draggedInventorySlot, slotHit);
	}
}