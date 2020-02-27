using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContinueButton : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		((HeartRhythmDialogueUI) DialogueManager.DialogueUI).FastForward();
	}
}