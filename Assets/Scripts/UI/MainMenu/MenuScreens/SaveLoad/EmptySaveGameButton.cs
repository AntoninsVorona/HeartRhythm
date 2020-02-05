using TMPro;
using UnityEngine;

public class EmptySaveGameButton : SaveLoadScreenButton
{
	public void Initialize()
	{
		ResetFill();
		uiHeart.gameObject.SetActive(false);
	}
}