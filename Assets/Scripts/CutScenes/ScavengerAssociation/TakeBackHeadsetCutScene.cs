using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Take Back Headset Cut Scene",
	fileName = "TakeBackHeadsetCutScene")]
public class TakeBackHeadsetCutScene : CutScene
{
	public BattleConfiguration battleConfiguration;

	protected override IEnumerator CutSceneSequence()
	{
		SaveSystem.currentGameSave.globalVariables.wearsHeadset = true;
		dialogueFinished = false;
		// //TODO Play Take Animation
		GameSessionManager.Instance.StartConversation("HeadSetTakenBack");
		yield return new WaitUntil(() => dialogueFinished);
		dialogueFinished = false;
		var playerPosition = Player.Instance.CurrentPosition;
		var spawnPoint = new Vector2Int(0, playerPosition.y + 1);
		var interceptionGuard =
			GameSessionManager.Instance.currentSceneObjects.currentMobManager.SpawnMob("InterceptionGuard",
				spawnPoint,
				new Mob.MovementSettings {moveDuringPeaceMode = false});
		for (var x = spawnPoint.x + 1; x <= playerPosition.x; x++)
		{
			interceptionGuard.Move(new Vector2Int(x, spawnPoint.y));
			yield return new WaitForSeconds(0.25f);
		}

		//TODO Player Exclamation Mark
		interceptionGuard.Move(playerPosition);
		yield return new WaitForSeconds(0.25f);
		GameSessionManager.Instance.CutSceneFinished();
		GameSessionManager.Instance.LoadLevel(battleConfiguration);
	}
}