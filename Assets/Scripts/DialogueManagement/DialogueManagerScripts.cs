using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueManagerScripts : MonoBehaviour
{
	private int lastSpeakerId = -1;

	private void OnConversationLine(Subtitle subtitle)
	{
		if (lastSpeakerId == -1)
		{
			lastSpeakerId = subtitle.speakerInfo.id;
		}
		else if (subtitle.speakerInfo.id != lastSpeakerId)
		{
			const string delayCommand = "Delay(0.5)";
			subtitle.sequence = string.IsNullOrEmpty(subtitle.sequence)
				? $"{delayCommand}; {{default}}"
				: $"{delayCommand}; {subtitle.sequence}";

			lastSpeakerId = subtitle.speakerInfo.id;
		}
	}

	private void OnConversationEnd(Transform actor)
	{
		lastSpeakerId = -1;
	}
}