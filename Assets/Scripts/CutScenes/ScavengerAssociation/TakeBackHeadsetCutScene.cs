using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Take Back Headset Cut Scene",
	fileName = "TakeBackHeadsetCutScene")]
public class TakeBackHeadsetCutScene : CutScene
{
	public BattleArea battleArea;

	protected override IEnumerator CutSceneSequence()
	{
		SaveSystem.currentGameSave.globalVariables.wearsHeadset = true;
		dialogueFinished = false;
		// //TODO Play Take Animation
		GameSessionManager.Instance.StartConversation("HeadSetTakenBack");
		yield return new WaitUntil(() => dialogueFinished);
		dialogueFinished = false;
		// //TODO Play Put On Animation
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

		GameSessionManager.Instance.StartConversation("HandOverHeadset");
		yield return new WaitUntil(() => dialogueFinished);
		dialogueFinished = false;
		yield return new WaitForSeconds(0.25f);
		interceptionGuard.Move(playerPosition);
		yield return new WaitForSeconds(0.25f);
		yield return GameSessionManager.Instance.LoadLevel(battleArea);
		SaveSystem.currentGameSave.GetLevelState("ScavengerField").unitData
			.Add(new Unit.UnitData("InterceptionGuard", new Vector2Int(20, -2)));
		GameSessionManager.Instance.StartConversation("WhereAmIInterceptionGuard");
		yield return new WaitUntil(() => dialogueFinished);
		dialogueFinished = false;
		GameSessionManager.Instance.InitializeFightWithAnEnemy(battleArea.battleMusic);
		GameSessionManager.Instance.CutSceneFinished();
	}
}