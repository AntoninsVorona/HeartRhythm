using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Find Headset Cut Scene", fileName = "FindHeadsetCutScene")]
public class FindHeadsetCutScene : CutScene 
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