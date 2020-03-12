using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class SaveLoadGameScreen : MenuScreen
{
	[SerializeField]
	private GameSaveLoadButton gameSaveLoadButtonPrefab;

	[SerializeField]
	private ScrollRect gameSaveScrollRect;

	[SerializeField]
	private RectTransform gameSaveButtonHolder;

	protected SaveLoadScreenButton selectedSaveLoadButton;
	public static bool savesInitialized;
	protected static List<GameSaveLoadButton> saveLoadButtons;

	public override void Open(bool withAnimation = true)
	{
		base.Open(withAnimation);
		if (!savesInitialized)
		{
			InitializeSaves();
		}
		else
		{
			RearrangeSaves();
		}
		
		foreach (var heartButton in saveLoadButtons)
		{
			heartButton.ResetFill();
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
		if (selectedSaveLoadButton)
		{
			selectedSaveLoadButton.Deselect();
			selectedSaveLoadButton = null;
		}

		return base.Close(withAnimation);
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetMouseButtonDown(0))
		{
			var hit = AbstractMainMenu.Instance.CurrentUIHit();
			if (hit)
			{
				var gameSaveButton = hit.GetComponentInParent<SaveLoadScreenButton>();
				if (gameSaveButton)
				{
					if (gameSaveButton != selectedSaveLoadButton)
					{
						if (selectedSaveLoadButton)
						{
							selectedSaveLoadButton.Deselect();
						}

						selectedSaveLoadButton = gameSaveButton;
						selectedSaveLoadButton.Select();
					}
				}
			}
		}
	}

	protected virtual void InitializeSaves()
	{
		savesInitialized = true;
		saveLoadButtons = new List<GameSaveLoadButton>();
		var isLatest = true;
		foreach (var uiLoadData in SaveSystem.uiGameSaves.OrderByDescending(u => u.lastChanged))
		{
			AddSave(uiLoadData, isLatest);
			isLatest = false;
		}
	}

	protected RectTransform AddSave(SaveSystem.UILoadData uiLoadData, bool isLatest)
	{
		return AddSave(uiLoadData, isLatest, saveLoadButtons.Count);
	}

	protected RectTransform AddSave(SaveSystem.UILoadData uiLoadData, bool isLatest, int index)
	{
		var gameSaveButton = Instantiate(gameSaveLoadButtonPrefab, gameSaveButtonHolder);
		gameSaveButton.Initialize(uiLoadData, isLatest);
		saveLoadButtons.Insert(index, gameSaveButton);
		return (RectTransform) gameSaveButton.transform;
	}

	public void BackToMainScreen()
	{
		AbstractMainMenu.Instance.OpenScreen<MainScreen>();
	}

	public void EraseClicked()
	{
		if (selectedSaveLoadButton && selectedSaveLoadButton is GameSaveLoadButton)
		{
			YesNoDialogue.Instance.Show(Erase);
		}
	}
	
	protected void Erase()
	{
		if (selectedSaveLoadButton && selectedSaveLoadButton is GameSaveLoadButton gameSaveLoadButton)
		{
			SaveSystem.EraseSave(gameSaveLoadButton.filePath);
			saveLoadButtons.Remove(gameSaveLoadButton);
			if (gameSaveLoadButton.latest)
			{
				var latestSave = saveLoadButtons.FirstOrDefault();
				if (latestSave)
				{
					latestSave.ApplyLatest(true);
				}
			}

			Destroy(gameSaveLoadButton.gameObject);
			selectedSaveLoadButton = null;
		}
	}

	private void RearrangeSaves()
	{
		var currentIndex = StartingIndex();
		foreach (var rect in saveLoadButtons.Select(saveLoadButton => (RectTransform) saveLoadButton.transform))
		{
			rect.parent = gameSaveButtonHolder;
			rect.localScale = Vector3.one;
			rect.localRotation = Quaternion.identity;
			rect.SetSiblingIndex(currentIndex++);
		}
	}

	public override void ApplyCancel()
	{
		BackToMainScreen();
	}

	protected abstract int StartingIndex();
}