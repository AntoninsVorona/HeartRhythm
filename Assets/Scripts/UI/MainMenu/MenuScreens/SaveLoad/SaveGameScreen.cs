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
			if (selectedSaveLoadButton is GameSaveLoadButton)
			{
				YesNoDialogue.Instance.Show(EraseAndApplySave);
			}
			else
			{
				ApplySave();
			}
		}
	}

	private void EraseAndApplySave()
	{
		Erase();
		ApplySave();
	}

	private void ApplySave()
	{
		var last = saveLoadButtons.FirstOrDefault();
		if (last)
		{
			last.ApplyLatest(false);
		}

		var newSave = AddSave(GameLogic.Instance.Save(), true, 0);
		newSave.SetSiblingIndex(1);
	}

	protected override int StartingIndex()
	{
		return 1;
	}
}