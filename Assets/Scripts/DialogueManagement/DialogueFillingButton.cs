using UnityEngine;
using UnityEngine.Events;

public class DialogueFillingButton : HeartButton
{
	[SerializeField]
	protected UIHeart uiHeart;
	
	public void SetOnClick(UnityAction action)
	{
		clickEvent.RemoveAllListeners();
		clickEvent.AddListener(action);
	}

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