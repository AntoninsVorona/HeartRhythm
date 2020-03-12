using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class InventorySlot : FillingButton
{
	public int slotId;

	[HideInNormalInspector]
	public Item itemInside;
	
	[SerializeField]
	private Image spriteIcon;

	[SerializeField]
	private TextMeshProUGUI itemAmount;

	public void Initialize()
	{
		Initialize(null, 0);
	}

	public void Initialize(Item item, int amount = 1)
	{
		itemInside = item;
		if (item)
		{
			spriteIcon.sprite = item.spriteIcon;
			spriteIcon.gameObject.SetActive(true);
			UpdateItemAmount(amount);
		}
		else
		{
			spriteIcon.gameObject.SetActive(false);
			UpdateItemAmount(0);
		}
	}

	public void UpdateItemAmount(int newAmount)
	{
		if (newAmount > 1)
		{
			itemAmount.gameObject.SetActive(true);
			itemAmount.text = newAmount.ToString();
		}
		else
		{
			itemAmount.gameObject.SetActive(false);
		}
	}

	public void EnableIcon()
	{
		if (itemInside)
		{
			spriteIcon.gameObject.SetActive(true);
		}
	}

	public void DisableIcon()
	{
		spriteIcon.gameObject.SetActive(false);
	}
}