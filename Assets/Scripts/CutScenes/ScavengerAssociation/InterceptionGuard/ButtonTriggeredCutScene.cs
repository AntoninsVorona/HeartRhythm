using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Button Triggered Cut Scene",
	fileName = "ButtonTriggeredCutScene")]
public class ButtonTriggeredCutScene : CutScene
{
	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		yield return new WaitForSeconds(0.5f);
		var battleRules = (InterceptionGuardBattleRules) BattleRules.Instance;
		battleRules.CloseDoors();
		GameSessionManager.Instance.currentBattleSettings.missedBeatDamage = new BattleArea.BattleDamage(0);
		GameSessionManager.Instance.currentBattleSettings.invalidInputDamage = new BattleArea.BattleDamage(0);
		AudioManager.Instance.ChangeVolume(0.3f);
		GameSessionManager.Instance.StartConversation("InterceptionGuardPhewThatWasClose");
		yield return new WaitUntil(() => dialogueFinished);
		AudioManager.Instance.ChangeVolume(1f);
		GameSessionManager.Instance.CutSceneFinished();
	}
}