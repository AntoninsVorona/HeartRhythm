using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Coming", fileName = "ComingToScavengerAssociationCutScene")]
public class ComingToScavengerAssociationCutScene : CutScene 
{
	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		Debug.Log("Start");
		Player.Instance.Move(new Vector2Int(-5, 12), true);
		yield return new WaitForSeconds(1);
		yield return Player.Instance.Move(new Vector2Int(-5, 13));
		yield return new WaitForSeconds(0.5f);
		yield return Player.Instance.Move(new Vector2Int(-5, 14));
		yield return new WaitForSeconds(0.5f);
		yield return Player.Instance.Move(new Vector2Int(-5, 15));
		yield return new WaitForSeconds(0.5f);
		GameLogic.Instance.StartConversation("ComingToScavengerAssociation");
		Debug.Log(dialogueFinished);
		yield return new WaitUntil(() => dialogueFinished);
		GameLogic.Instance.CutSceneFinished();
		Debug.Log("End");
	}
}