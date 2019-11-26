using UnityEngine;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
	[Header("Dragging")]
	[HideInNormalInspector]
	public InventorySlot draggedInventorySlot;

	[SerializeField]
	private Image draggedItemImage;

	[SerializeField]
	private RectTransform draggedItemTransform;

	public void BeginDrag(InventorySlot slotHit)
	{
		if (slotHit && slotHit.itemInside)
		{
			draggedInventorySlot = slotHit;
			draggedItemImage.sprite = draggedInventorySlot.itemInside.spriteIcon;
			draggedItemTransform.position = Input.mousePosition;
			draggedItemTransform.gameObject.SetActive(true);
			draggedInventorySlot.DisableIcon();
		}
	}

	public void OnDrag()
	{
		draggedItemTransform.position = Input.mousePosition;
	}

	public InventorySlot OnEndDrag()
	{
		var slotHit = UIInventory.GetSlotHit();
		if (slotHit)
		{
			DraggedIntoSlot(slotHit);
		}

		draggedInventorySlot.EnableIcon();
		draggedInventorySlot = null;
		draggedItemTransform.gameObject.SetActive(false);
		return slotHit;
	}

	public void StopDrag()
	{
		if (draggedInventorySlot)
		{
			draggedInventorySlot.EnableIcon();
		}

		draggedInventorySlot = null;
		draggedItemTransform.gameObject.SetActive(false);
	}

	private void DraggedIntoSlot(InventorySlot slotHit)
	{
		Player.Instance.ChangeSlots(draggedInventorySlot, slotHit);
	}
}