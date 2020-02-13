using UnityEngine;

public abstract class SaveLoadScreenButton : FillingButton
{
	[SerializeField]
	protected UIHeart uiHeart;

	public override AbstractMainMenu.HeartSettings Select()
	{
		uiHeart.gameObject.SetActive(true);
		uiHeart.Subscribe();
		return base.Select();
	}

	public override void Deselect()
	{
		base.Deselect();
		uiHeart.Unsubscribe();
		uiHeart.gameObject.SetActive(false);
	}
}