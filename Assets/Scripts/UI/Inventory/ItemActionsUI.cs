using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemActionsUI : MonoBehaviour
{
	private const string DROP_AMOUNT_TEXT = "Drop: {0}";
	
	public enum ItemActionType
	{
		Drop = 0,
		Use = 1,
		Move = 2
	}

	[Serializable]
	public class ActionXButton : SerializableDictionary<ItemActionType, Button>
	{
	}

	[SerializeField]
	private ActionXButton actionXButtons;

	[SerializeField]
	private Transform actionHolder;

	[SerializeField]
	private GameObject inputSliderHolder;

	[SerializeField]
	private Slider inputSlider;

	[SerializeField]
	private TextMeshProUGUI dropAmount;

	[HideInNormalInspector]
	public bool dropInProgress;
	private int maxDropCount;

	public void Close()
	{
		actionHolder.gameObject.SetActive(false);
		inputSliderHolder.SetActive(false);
		dropInProgress = false;
	}

	public void OpenActionsFor(InventorySlot slot)
	{
		var item = slot.itemInside;
		maxDropCount = Player.Instance.ItemsInSlot(slot);
		foreach (var button in actionXButtons)
		{
			button.Value.gameObject.SetActive(item.accessibleActions.Contains(button.Key));
		}

		actionHolder.position = slot.transform.position;
		actionHolder.gameObject.SetActive(true);
	}

	public void MovePressed()
	{
		Close();
	}

	public void UsePressed()
	{
		Close();
	}

	public void DropPressed()
	{
		actionHolder.gameObject.SetActive(false);
		inputSliderHolder.SetActive(true);
		inputSlider.maxValue = maxDropCount;
		inputSlider.value = maxDropCount;
		dropAmount.text = string.Format(DROP_AMOUNT_TEXT, maxDropCount);
		dropInProgress = true;
		EventSystem.current.SetSelectedGameObject(inputSlider.gameObject);
	}

	public void DropInputDone()
	{
		DropInputDone(Mathf.RoundToInt(inputSlider.value));
	}

	public void DropInputDone(int input)
	{
		GameUI.Instance.uiInventory.DropActionPressed(input);
		inputSliderHolder.SetActive(false);
		dropInProgress = false;
	}

	public void OnValueChanged(float value)
	{
		dropAmount.text = string.Format(DROP_AMOUNT_TEXT, value);
	}

	public void DropCancel()
	{
		Close();
	}
}