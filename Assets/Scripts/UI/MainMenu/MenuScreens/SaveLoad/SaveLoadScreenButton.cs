using UnityEngine;

public abstract class SaveLoadScreenButton : FillingButton
{
	[SerializeField]
	protected UIHeart uiHeart;

	public override AbstractMainMenu.HeartSettings Select()
	{
		uiHeart.gameObject.SetActive(true); //TODO Subscribe to beat
		return base.Select();
	}

	public override void Deselect()
	{
		base.Deselect();
		uiHeart.gameObject.SetActive(false); //TODO Unsubscribe to beat
	}
}