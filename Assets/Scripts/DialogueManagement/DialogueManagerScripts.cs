using PixelCrushers.DialogueSystem;
using UnityEngine;

public class DialogueManagerScripts : MonoBehaviour
{
	private int lastSpeakerId = -1;
	private bool conversationStarted;

	private void OnConversationStart(Transform actor)
	{
		conversationStarted = true;
	}

	private void OnConversationLine(Subtitle subtitle)
	{
		if (conversationStarted && (lastSpeakerId == -1 || subtitle.speakerInfo.id != lastSpeakerId))
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
		conversationStarted = false;
	}
}