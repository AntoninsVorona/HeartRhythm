using System.Linq;
using UnityEngine;

public class SaveGameScreen : SaveLoadGameScreen
{
	[SerializeField]
	private EmptySaveGameButton emptySaveGameButton;

	protected override void InitializeSaves()
	{
		emptySaveGameButton.Initialize();
		base.InitializeSaves();
	}

	public override void Open(bool withAnimation = true)
	{
		base.Open(withAnimation);
		selectedSaveLoadButton = emptySaveGameButton;
		emptySaveGameButton.Select();
	}

	public void Save()
	{
		if (selectedSaveLoadButton)
		{
			var last = saveLoadButtons.FirstOrDefault();
			if (last)
			{
				last.ApplyLatest(false);
			}

			if (selectedSaveLoadButton is GameSaveLoadButton)
			{
				Erase(); //TODO Add YesNoWindow (Empty UI Rect)
			}

			var newSave = AddSave(GameLogic.Instance.Save(), true, 0);
			newSave.SetSiblingIndex(1);
		}
	}

	protected override int StartingIndex()
	{
		return 1;
	}
}