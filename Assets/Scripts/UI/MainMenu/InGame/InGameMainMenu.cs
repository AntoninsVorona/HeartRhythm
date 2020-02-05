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
	}
	
	private void InitHeartBeat()
	{
		//TODO
	}
	
	protected override CustomStandaloneInputModule GetModule()
	{
		return GameUI.Instance.inputModule;
	}
}