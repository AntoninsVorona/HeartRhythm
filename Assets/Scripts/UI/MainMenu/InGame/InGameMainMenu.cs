using System;
using UnityEngine;

public class InGameMainMenu : AbstractMainMenu
{
	public void Open()
	{
		globalCanvasGroup.gameObject.SetActive(true);
		screens.currentScreen = null;
		OpenScreen<MainScreen>(false);
		InitHeartBeat();
	}

	public void Close()
	{
		if (screens.currentScreen)
		{
			screens.currentScreen.Close(false);
		}

		globalCanvasGroup.gameObject.SetActive(false);
		uiHeart.Unsubscribe();
	}
	
	private void InitHeartBeat()
	{
		uiHeart.Reset();
		if (AudioManager.Instance.IsCurrentlyPlaying)
		{
			uiHeart.Subscribe();
		}
	}
	
	
	
	protected override CustomStandaloneInputModule GetModule()
	{
		return GameUI.Instance.inputModule;
	}
}