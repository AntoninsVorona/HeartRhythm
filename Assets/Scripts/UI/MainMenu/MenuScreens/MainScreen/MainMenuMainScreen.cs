using System.Linq;
using UnityEngine;

public class MainMenuMainScreen : MainScreen
{
	private bool secretActivated = false;

	[SerializeField]
	private HeartButton continueButton;
	
	protected override void EnableLoadingButtons()
	{
		base.EnableLoadingButtons();
		continueButton.gameObject.SetActive(true);
	}
	
	protected override void DisableLoadingButtons()
	{
		base.DisableLoadingButtons();
		continueButton.gameObject.SetActive(false);
	}

	protected override void Update()
	{
		base.Update();
		// if (!secretActivated)
		// {
		// 	var secretCanBeActivated = fillingButtons.All(heartButton => !(heartButton.FillAmount < 0.5f));
		// 	if (secretCanBeActivated)
		// 	{
		// 		ActivateSecret();
		// 	}
		// }
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

	public void ContinueClicked()
	{
		var latestSave = SaveSystem.GetLatestSave();
		if (string.IsNullOrEmpty(latestSave))
		{
			return;
		}

		GameLogic.Instance.LoadSave(latestSave, true);
	}

	public override void QuitClicked()
	{
		AppHelper.Quit();
	}

	public override void ApplyCancel()
	{
		
	}
}