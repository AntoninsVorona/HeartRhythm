using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadGameScreen : MenuScreen
{
	[SerializeField]
	private GameSaveButton gameSaveButtonPrefab;

	[SerializeField]
	private RectTransform gameSaveButtonHolder;

	private GameSaveButton selectedGameSaveButton;
	private bool savesInitialized;

	public override void Open(bool withAnimation = true)
	{
		if (!savesInitialized)
		{
			InitializeSaves();
		}
		base.Open(withAnimation);
	}

	public override Coroutine Close(bool withAnimation = true)
	{
		if (selectedGameSaveButton)
		{
			selectedGameSaveButton.Deselect();
			selectedGameSaveButton = null;
		}

		return base.Close(withAnimation);
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var hit = MainMenuUI.Instance.CurrentUIHit();
			if (hit)
			{
				var gameSaveButton = hit.GetComponentInParent<GameSaveButton>();
				if (gameSaveButton)
				{
					if (gameSaveButton != selectedGameSaveButton)
					{
						if (selectedGameSaveButton)
						{
							selectedGameSaveButton.Deselect();
						}

						selectedGameSaveButton = gameSaveButton;
						selectedGameSaveButton.Select();
					}
				}
			}
		}
	}

	private void InitializeSaves()
	{
		savesInitialized = true;
		foreach (var uiLoadData in SaveSystem.uiGameSaves.OrderByDescending(u => u.lastChanged))
		{
			AddSave(uiLoadData);
		}
	}

	private void AddSave(SaveSystem.UILoadData uiLoadData)
	{
		var gameSaveButton = Instantiate(gameSaveButtonPrefab, gameSaveButtonHolder);
		gameSaveButton.Initialize(uiLoadData);
		fillingButtons.Add(gameSaveButton);
	}

	public void BackToMainScreen()
	{
		MainMenuUI.Instance.OpenScreen<MainScreen>();
	}

	public void Load()
	{
		if (selectedGameSaveButton)
		{
			GameLogic.Instance.LoadSave(selectedGameSaveButton.filePath, true);
		}
	}

	public void Erase()
	{
		if (selectedGameSaveButton)
		{
			SaveSystem.EraseSave(selectedGameSaveButton.filePath);
			fillingButtons.Remove(selectedGameSaveButton);
			Destroy(selectedGameSaveButton.gameObject);
			selectedGameSaveButton = null;
		}
	}
}