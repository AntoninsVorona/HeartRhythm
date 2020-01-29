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
	private Animator heart;
	
	public void Initialize(SaveSystem.UILoadData uiLoadData)
	{
		filePath = uiLoadData.filePath;
		lastChangedText.text = uiLoadData.lastChanged.ToString(CultureInfo.InvariantCulture);
		heart.gameObject.SetActive(false);
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