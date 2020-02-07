using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueManagerScripts : MonoBehaviour
{
	private int lastSpeakerId;

	private void OnConversationStart(Transform actor)
	{
		lastSpeakerId = -1;
	}

	private void OnConversationLine(Subtitle subtitle)
	{
		if (subtitle.dialogueEntry.id != 0 && subtitle.speakerInfo.id != lastSpeakerId)
		{
			const string delayCommand = "Delay(0.45)";
			subtitle.sequence = string.IsNullOrEmpty(subtitle.sequence)
				? $"{delayCommand}; {{default}}"
				: $"{delayCommand}; {subtitle.sequence}";
		
			lastSpeakerId = subtitle.speakerInfo.id;
		}
	}
}