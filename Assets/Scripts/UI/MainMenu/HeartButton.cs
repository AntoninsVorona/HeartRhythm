using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HeartButton : MonoBehaviour
{
	private const float FILL_RATE = 0.05f;
	private const float DECAY_RATE = 0.125f;
	private const float FILL_DELTA = 0.02f;

	[SerializeField]
	private UnityEvent clickEvent;
	[SerializeField]
	private MainMenuUI.HeartSettings heartSettings;
	[SerializeField]
	private Image normalImage;

	private float fillAmount;
	private bool isSelected;
	private float time;

	public float FillAmount
	{
		get => fillAmount;
		private set
		{
			fillAmount = value;
			UpdateFillAmount();
		}
	}

	private void Start()
	{
		normalImage.alphaHitTestMinimumThreshold = 1;
	}

	private void Update()
	{
		if (isSelected)
		{
			FillUp();
		}
		else
		{
			Decay();
		}
	}

	private void FillUp()
	{
		if (FillAmount < 1)
		{
			var currentTime = Time.time;
			if (time + FILL_RATE < currentTime)
			{
				time = currentTime;
				FillAmount += FILL_DELTA;
			}
		}
	}

	private void Decay()
	{
		if (FillAmount > 0)
		{
			var currentTime = Time.time;
			if (time + DECAY_RATE < currentTime)
			{
				time = currentTime;
				FillAmount -= FILL_DELTA;
			}
		}
	}

	public MainMenuUI.HeartSettings Select()
	{
		isSelected = true;
		time = 0;
		return heartSettings;
	}

	public void Deselect()
	{
		isSelected = false;
		time = Time.time;
	}

	private void UpdateFillAmount()
	{
		normalImage.fillAmount = FillAmount;
	}

	public void ResetFill()
	{
		FillAmount = 0;
	}

	public void Click()
	{
		clickEvent.Invoke();
	}
}