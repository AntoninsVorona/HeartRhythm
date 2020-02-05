using System.Globalization;
using TMPro;
using UnityEngine;

public class GameSaveLoadButton : SaveLoadScreenButton
{
	[HideInInspector]
	public string filePath;

	[SerializeField]
	private TextMeshProUGUI lastChangedText;

	[SerializeField]
	private TextMeshProUGUI lastText;

	[HideInInspector]
	public bool latest;

	public void Initialize(SaveSystem.UILoadData uiLoadData, bool isLatestSave)
	{
		ResetFill();
		filePath = uiLoadData.filePath;
		lastChangedText.text = uiLoadData.lastChanged.ToString(CultureInfo.InvariantCulture);
		uiHeart.gameObject.SetActive(false);
		ApplyLatest(isLatestSave);
	}

	public void ApplyLatest(bool isLatestSave)
	{
		latest = isLatestSave;
		lastText.gameObject.SetActive(isLatestSave);
	}
}