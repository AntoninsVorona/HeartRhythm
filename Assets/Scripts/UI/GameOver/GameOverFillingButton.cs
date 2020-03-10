using UnityEngine;
using UnityEngine.Events;

public class GameOverFillingButton : HeartButton
{
	[SerializeField]
	protected UIHeart uiHeart;

	public override AbstractMainMenu.HeartSettings Select()
	{
		uiHeart.gameObject.SetActive(true);
		return base.Select();
	}

	public override void Deselect()
	{
		uiHeart.gameObject.SetActive(false);
		base.Deselect();
	}
}