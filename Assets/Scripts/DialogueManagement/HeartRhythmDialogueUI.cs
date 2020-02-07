using System.Linq;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartRhythmDialogueUI : StandardDialogueUI
{
	[SerializeField]
	private GameObject playerPortrait;

	[SerializeField]
	private Image playerImage;

	[SerializeField]
	private TextMeshProUGUI playerName;

	public override void Start()
	{
		var actor = DialogueManager.masterDatabase.GetActor("Player");
		playerImage.sprite = actor.spritePortrait;
		playerName.text = actor.fields.First(f => f.title == "Display Name").value;
		playerPortrait.SetActive(true);
		base.Start();
	}
	
	public override void ShowResponses(Subtitle subtitle, Response[] responses, float timeout)
	{
		base.ShowResponses(subtitle, responses, timeout);
		conversationUIElements.defaultNPCSubtitlePanel.Close();
	}
}