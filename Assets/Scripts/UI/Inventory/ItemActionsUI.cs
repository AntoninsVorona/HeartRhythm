using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemActionsUI : MonoBehaviour
{
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
	private TMP_InputField dropCountInput;

	private int maxDropCount;

	private void Update()
	{
		if (dropCountInput.gameObject.activeSelf)
		{
			if (Input.GetButtonDown("Cancel"))
			{
				DropCancel();
			}
			else if (Input.GetButtonDown("Submit"))
			{
				DropInputDone();
			}
		}
	}

	public void Close()
	{
		actionHolder.gameObject.SetActive(false);
		dropCountInput.gameObject.SetActive(false);
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
		dropCountInput.gameObject.SetActive(true);
		dropCountInput.text = maxDropCount.ToString();
		EventSystem.current.SetSelectedGameObject(dropCountInput.gameObject);
	}

	public void DropInputDone()
	{
		DropInputDone(dropCountInput.text);
	}

	public void DropInputDone(string input)
	{
		var success = int.TryParse(input, out int result);
		if (success)
		{
			GameUI.Instance.uiInventory.DropActionPressed(result);
			dropCountInput.gameObject.SetActive(false);
		}
	}

	public void OnValueChange(string input)
	{
		var success = int.TryParse(input, out int result);
		if (success)
		{
			if (result < 0)
			{
				dropCountInput.text = 0.ToString();
			}
			else if (result > maxDropCount)
			{
				dropCountInput.text = maxDropCount.ToString();
			}
		}
	}

	public void DropCancel()
	{
		Close();
	}
}