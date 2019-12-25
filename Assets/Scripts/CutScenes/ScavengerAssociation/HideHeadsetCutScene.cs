using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Hide Headset Cut Scene", fileName = "HideHeadsetCutScene")]
public class HideHeadsetCutScene : CutScene 
{
	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		//TODO Play Search Animation and Sound
		GameLogic.Instance.StartConversation("FoundHeadsetFirstTime");
		yield return new WaitUntil(() => dialogueFinished);
		Player.Instance.PickUpItem(ItemManager.Instance.GetItemByName("Headset"));
		GameLogic.Instance.CutSceneFinished();
	}
}