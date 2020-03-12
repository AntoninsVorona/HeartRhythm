using System.Collections;
using UnityEngine;


[CreateAssetMenu(menuName = "CutScenes/New Game Cut Scene", fileName = "NewGameCutScene")]
public class NewGameCutScene : CutScene
{
	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		yield return SplashArtController.Instance.PlaySequence();
		yield return new WaitForSeconds(0.5f);
		GameSessionManager.Instance.StartConversation("NewGame");
		yield return new WaitUntil(() => dialogueFinished);
		GameSessionManager.Instance.CutSceneFinished();
		yield return new WaitForSeconds(0.5f);
		GameUI.Instance.messageBox.Show("YOUR ROOM");
	}
}