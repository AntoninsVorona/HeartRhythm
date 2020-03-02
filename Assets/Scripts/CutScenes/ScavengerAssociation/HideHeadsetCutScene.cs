using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Hide Headset Cut Scene", fileName = "HideHeadsetCutScene")]
public class HideHeadsetCutScene : CutScene
{
	public LevelData levelToLoad;
	
	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		//TODO Play Hide Animation
		Player.Instance.LoseItem(ItemManager.Instance.GetItemByName("Headset"));
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.playerHasToSleepFirst = false;
		GameSessionManager.Instance.StartConversation("HeadSetHidden");
		yield return new WaitUntil(() => dialogueFinished);
		dialogueFinished = false;
		yield return GameSessionManager.Instance.LoadLevel(levelToLoad, 0);
		yield return new WaitForSeconds(0.5f);
		GameSessionManager.Instance.StartConversation("ItsTimeToPackStuff");
		yield return new WaitUntil(() => dialogueFinished);
		GameSessionManager.Instance.CutSceneFinished();
	}
}