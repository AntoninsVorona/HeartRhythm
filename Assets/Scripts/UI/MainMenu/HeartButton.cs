using UnityEngine;
using UnityEngine.Events;

public class HeartButton : FillingButton
{
	[SerializeField]
	protected UnityEvent clickEvent;
	[SerializeField]
	private AbstractMainMenu.HeartSettings heartSettings = AbstractMainMenu.HeartSettings.DEFAULT_SETTINGS;

	private void Start()
	{
		normalImage.alphaHitTestMinimumThreshold = 1;
	}

	public void Click()
	{
		clickEvent.Invoke();
	}

	public override AbstractMainMenu.HeartSettings Select()
	{
		base.Select();
		return heartSettings;
	}
}