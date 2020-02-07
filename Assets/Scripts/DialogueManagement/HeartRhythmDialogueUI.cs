using System.Linq;
using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartRhythmDialogueUI : StandardDialogueUI
{
	private const string DELAY_COMMAND = "Delay(5)";

	[SerializeField]
	private GameObject playerPortrait;

	[SerializeField]
	private Image playerImage;

	[SerializeField]
	private TextMeshProUGUI playerName;

	private int lastSpeakerId;

	public override void Start()
	{
		var actor = DialogueManager.masterDatabase.GetActor("Player");
		playerImage.sprite = actor.spritePortrait;
		playerName.text = actor.fields.First(f => f.title == "Display Name").value;
		playerPortrait.SetActive(true);
		base.Start();
	}
	
	public override void Open()
	{
		base.Open();
		lastSpeakerId = -1;
	}
	
	public override void ShowSubtitle(Subtitle subtitle)
	{
		// Debug.Log("Show");
		// if (lastSpeakerId == -1 || subtitle.speakerInfo.id != lastSpeakerId)
		// {
		// 	subtitle.sequence = string.IsNullOrEmpty(subtitle.sequence)
		// 		? $"{DELAY_COMMAND}; {{default}}"
		// 		: $"{DELAY_COMMAND}; {subtitle.sequence}";
		// 	lastSpeakerId = subtitle.speakerInfo.id;
		// }

		base.ShowSubtitle(subtitle);
	}
	

	public override void ShowResponses(Subtitle subtitle, Response[] responses, float timeout)
	{
		base.ShowResponses(subtitle, responses, timeout);
		conversationUIElements.defaultNPCSubtitlePanel.Close();
	}
}