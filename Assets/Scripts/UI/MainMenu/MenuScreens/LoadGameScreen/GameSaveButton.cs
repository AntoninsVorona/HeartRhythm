using System.Globalization;
using TMPro;
using UnityEngine;

public class GameSaveButton : FillingButton
{
	[HideInInspector]
	public string filePath;

	[SerializeField]
	private TextMeshProUGUI lastChangedText;

	[SerializeField]
	private TextMeshProUGUI lastText;

	[SerializeField]
	private Animator heart;

	[HideInInspector]
	public bool latest;

	public void Initialize(SaveSystem.UILoadData uiLoadData, bool isLatestSave)
	{
		ResetFill();
		filePath = uiLoadData.filePath;
		lastChangedText.text = uiLoadData.lastChanged.ToString(CultureInfo.InvariantCulture);
		heart.gameObject.SetActive(false);
		ApplyLatest(isLatestSave);
	}

	public void ApplyLatest(bool isLatestSave)
	{
		latest = isLatestSave;
		lastText.gameObject.SetActive(isLatestSave);
	}

	public override MainMenuUI.HeartSettings Select()
	{
		heart.gameObject.SetActive(true); //TODO Subscribe to beat
		return base.Select();
	}

	public override void Deselect()
	{
		base.Deselect();
		heart.gameObject.SetActive(false);
	}
}