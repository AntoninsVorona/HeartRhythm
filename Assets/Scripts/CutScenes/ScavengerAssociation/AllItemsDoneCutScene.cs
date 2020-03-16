using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/All Items Done Cut Scene",
	fileName = "AllItemsDoneCutScene")]
public class AllItemsDoneCutScene : CutScene
{
	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		yield return new WaitUntil(() => dialogueFinished);
		dialogueFinished = false;
		GameSessionManager.Instance.StartConversation("AllItemsDone");
		yield return new WaitUntil(() => dialogueFinished);
		Player.Instance.PickUpItem(ItemManager.Instance.GetItemByName("Microscheme"));
		yield return new WaitForSeconds(0.5f);
		GameLogic.Instance.Save();
		GameSessionManager.Instance.CutSceneFinished();
	}
}