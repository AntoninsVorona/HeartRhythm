using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Take Back Headset Cut Scene", fileName = "TakeBackHeadsetCutScene")]
public class TakeBackHeadsetCutScene : CutScene
{
	protected override IEnumerator CutSceneSequence()
	{
		//TODO Make Cutscene
		// dialogueFinished = false;
		// //TODO Play Hide Animation
		// Player.Instance.LoseItem(ItemManager.Instance.GetItemByName("Headset"));
		// GameLogic.Instance.StartConversation("HeadSetHidden");
		// yield return new WaitUntil(() => dialogueFinished);
		// dialogueFinished = false;
		// yield return new WaitForSeconds(0.5f);
		// GameLogic.Instance.StartConversation("ItsTimeToPackStuff");;
		// yield return new WaitUntil(() => dialogueFinished);
		yield return null;
		GameSessionManager.Instance.CutSceneFinished();
	}
}