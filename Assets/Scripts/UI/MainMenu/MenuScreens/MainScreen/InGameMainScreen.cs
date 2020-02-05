using UnityEngine;

public class InGameMainScreen : MainScreen
{
	[SerializeField]
	private HeartButton saveButton;

	public override void Open(bool withAnimation = true)
	{
		if (!GameLogic.Instance.CanSave())
		{
			saveButton.gameObject.SetActive(false);
		}

		base.Open(withAnimation);
	}

	public void ResumeClicked()
	{
		GameSessionManager.Instance.CloseMainMenu();
	}

	public void SaveClicked()
	{
		AbstractMainMenu.Instance.OpenScreen<SaveGameScreen>();
	}

	public override void QuitClicked()
	{
		GameLogic.Instance.LoadMainMenuScene();
	}

	public override void ApplyCancel()
	{
		GameSessionManager.Instance.CloseMainMenu();
	}
}