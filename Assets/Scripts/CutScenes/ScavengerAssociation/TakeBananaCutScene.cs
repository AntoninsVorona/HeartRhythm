using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Take Banana Cut Scene",
	fileName = "TakeBananaCutScene")]
public class TakeBananaCutScene : CutScene
{
	public LevelData levelData;
	
	protected override IEnumerator CutSceneSequence()
	{
		((CoolDudeRoomLocationRules) LocationRules.Instance).BananaTaken();
		dialogueFinished = false;
		GameSessionManager.Instance.StartConversation("BananaTaken");
		yield return new WaitUntil(() => dialogueFinished);
		yield return GameSessionManager.Instance.LoadLevel(levelData, 2);
		GameLogic.Instance.Save();
		GameSessionManager.Instance.CutSceneFinished();
	}
}