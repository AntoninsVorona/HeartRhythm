using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainScreen : MenuScreen
{
	private bool secretActivated = false;

	[SerializeField]
	private HeartButton continueButton;

	[SerializeField]
	private HeartButton loadButton;

	public override void Open(bool withAnimation = true)
	{
		if (!SaveSystem.HasAnySaves())
		{
			continueButton.gameObject.SetActive(false);
			loadButton.gameObject.SetActive(false);
		}

		base.Open(withAnimation);
	}

	private void Update()
	{
		if (!secretActivated)
		{
			var secretCanBeActivated = fillingButtons.All(heartButton => !(heartButton.FillAmount < 0.5f));
			if (secretCanBeActivated)
			{
				ActivateSecret();
			}
		}
	}

	private void ActivateSecret()
	{
		secretActivated = true;
		Debug.Log("Secret Activated!");
	}

	public void NewGameClicked()
	{
		GameLogic.Instance.NewGame();
	}

	public void LoadGameClicked()
	{
		MainMenuUI.Instance.OpenScreen<LoadGameScreen>();
	}
	
	public void ContinueClicked()
	{
		var latestSave = SaveSystem.GetLatestSave();
		if (string.IsNullOrEmpty(latestSave))
		{
			return;
		}

		GameLogic.Instance.LoadSave(latestSave, true);
	}

	public void QuitClicked()
	{
		AppHelper.Quit();
	}
}