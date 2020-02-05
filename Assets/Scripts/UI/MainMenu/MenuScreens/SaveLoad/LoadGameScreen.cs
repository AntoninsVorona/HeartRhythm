using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoadGameScreen : SaveLoadGameScreen
{
	public void Load()
	{
		if (selectedSaveLoadButton)
		{
			GameLogic.Instance.LoadSave(((GameSaveLoadButton) selectedSaveLoadButton).filePath, true);
		}
	}
	
	protected override int StartingIndex()
	{
		return 0;
	}
}