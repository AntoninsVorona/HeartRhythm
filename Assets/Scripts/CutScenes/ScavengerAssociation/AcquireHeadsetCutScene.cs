using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CutScenes/Scavenger Association/Acquire Headset Cut Scene",
	fileName = "AcquireHeadsetCutScene")]
public class AcquireHeadsetCutScene : CutScene
{
	public BattleArea battleArea;

	protected override IEnumerator CutSceneSequence()
	{
		dialogueFinished = false;
		SaveSystem.currentGameSave.globalVariables.wearsHeadset = true;
		Player.Instance.PlayAnimation("TakeHeadset");
		Player.Instance.UpdateHeadset();
		var timeStarted = Time.time;
		GameSessionManager.Instance.StartConversation("HeadSetAcquired");
		yield return new WaitUntil(() => dialogueFinished);
		var timeDiff = Time.time - timeStarted;
		if (timeDiff < 4)
		{
			yield return new WaitForSeconds(4 - timeDiff);
		}

		dialogueFinished = false;
		var playerPosition = Player.Instance.CurrentPosition;
		var spawnPoint = new Vector2Int(0, 1);
		var interceptionGuard =
			GameSessionManager.Instance.currentSceneObjects.currentMobManager.SpawnMob("InterceptionGuard",
				spawnPoint,
				new Mob.MovementSettings {moveDuringPeaceMode = false});
		for (var y = spawnPoint.y - 1; y >= playerPosition.y; y--)
		{
			interceptionGuard.Move(new Vector2Int(spawnPoint.x, y));
			yield return new WaitForSeconds(0.25f);
		}

		for (var x = spawnPoint.x + 1; x < playerPosition.x; x++)
		{
			interceptionGuard.Move(new Vector2Int(x, playerPosition.y));
			yield return new WaitForSeconds(0.25f);
		}

		Player.Instance.TurnAround(true);
		GameSessionManager.Instance.StartConversation("HandOverHeadset");
		yield return new WaitUntil(() => dialogueFinished);
		dialogueFinished = false;
		yield return new WaitForSeconds(0.25f);
		var interceptionGuardPosition = interceptionGuard.CurrentPosition;
		interceptionGuard.Move(playerPosition);
		yield return new WaitForSeconds(0.25f);
		yield return GameSessionManager.Instance.LoadLevel(battleArea);
		SaveSystem.currentGameSave.GetLevelState("PankPocRoom").unitData
			.Add(new Unit.UnitData("InterceptionGuard", interceptionGuardPosition));
		GameSessionManager.Instance.StartConversation("WhereAmIInterceptionGuard");
		yield return new WaitUntil(() => dialogueFinished);
		dialogueFinished = false;
		GameSessionManager.Instance.InitializeFightWithAnEnemy(battleArea);
		GameSessionManager.Instance.CutSceneFinished();
	}
}