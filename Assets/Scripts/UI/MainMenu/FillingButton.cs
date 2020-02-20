using UnityEngine;
using UnityEngine.UI;

public class FillingButton : MonoBehaviour
{
	private const float FILL_RATE = 0.005f;
	private const float DECAY_RATE = 0.025f;
	private const float FILL_DELTA = 0.02f;

	private float fillAmount;
	private bool isSelected;
	private float time;

	[SerializeField]
	protected Image normalImage;

	public float FillAmount
	{
		get => fillAmount;
		private set
		{
			fillAmount = value;
			UpdateFillAmount();
		}
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

	public virtual AbstractMainMenu.HeartSettings Select()
	{
		isSelected = true;
		time = 0;
		return default;
	}

	public virtual void Deselect()
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
}