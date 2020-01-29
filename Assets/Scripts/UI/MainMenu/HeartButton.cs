using UnityEngine;
using UnityEngine.Events;

public class HeartButton : FillingButton
{
	[SerializeField]
	private UnityEvent clickEvent;
	[SerializeField]
	private MainMenuUI.HeartSettings heartSettings = MainMenuUI.HeartSettings.DEFAULT_SETTINGS;

	private void Start()
	{
		normalImage.alphaHitTestMinimumThreshold = 1;
	}

	public void Click()
	{
		clickEvent.Invoke();
	}

	public override MainMenuUI.HeartSettings Select()
	{
		base.Select();
		return heartSettings;
	}
}