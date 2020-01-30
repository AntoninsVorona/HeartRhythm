using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoadGameScreen : MenuScreen
{
	[SerializeField]
	private GameSaveButton gameSaveButtonPrefab;

	[SerializeField]
	private ScrollRect gameSaveScrollRect;

	[SerializeField]
	private RectTransform gameSaveButtonHolder;

	private GameSaveButton selectedGameSaveButton;
	private bool savesInitialized;
	private List<GameSaveButton> gameSaveButtons;

	public override void Open(bool withAnimation = true)
	{
		base.Open(withAnimation);
		if (!savesInitialized)
		{
			InitializeSaves();
		}

		StartCoroutine(ScrollToTop());
	}

	private IEnumerator ScrollToTop()
	{
		yield return new WaitForSeconds(0.4f);
		gameSaveScrollRect.verticalNormalizedPosition = 1;
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
		gameSaveButtons = new List<GameSaveButton>();
		savesInitialized = true;
		var isLatest = true;
		foreach (var uiLoadData in SaveSystem.uiGameSaves.OrderByDescending(u => u.lastChanged))
		{
			AddSave(uiLoadData, isLatest);
			isLatest = false;
		}
	}

	private void AddSave(SaveSystem.UILoadData uiLoadData, bool isLatest)
	{
		var gameSaveButton = Instantiate(gameSaveButtonPrefab, gameSaveButtonHolder);
		gameSaveButton.Initialize(uiLoadData, isLatest);
		fillingButtons.Add(gameSaveButton);
		gameSaveButtons.Add(gameSaveButton);
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
			gameSaveButtons.Remove(selectedGameSaveButton);
			if (selectedGameSaveButton.latest)
			{
				var latestSave = gameSaveButtons.FirstOrDefault();
				if (latestSave)
				{
					latestSave.ApplyLatest(true);
				}
			}

			Destroy(selectedGameSaveButton.gameObject);
			selectedGameSaveButton = null;
		}
	}
}