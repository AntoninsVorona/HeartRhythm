using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InterceptionGuardBattleRules : BattleRules
{
	[Serializable]
	public class DoorObstacle
	{
		public List<Vector2Int> doorPosition;
		public Animator doorAnimator;
	}

	public Vector2Int positionToTriggerDamagingGuardCutScene;
	private bool guardTriggered;
	public DamagingGuardCutScene damagingGuardCutScene;
	public Enemy damagingGuard;
	public Vector2Int buttonTriggerPosition;
	private bool buttonTriggered;
	public ButtonTriggeredCutScene buttonTriggeredCutScene;
	public DoorObstacle placeDoor;
	public DoorObstacle removeDoor1;
	public DoorObstacle removeDoor2;
	public int spawnRate = 3;
	public Vector2Int spawnPoint1;
	public Vector2Int spawnPoint2;
	public Enemy enemyToSpawn;

	[HideInInspector]
	public bool startedSpawning;

	private int currentBeatCount;

	protected override void Start()
	{
		base.Start();
		startedSpawning = false;
		guardTriggered = false;
		buttonTriggered = false;
	}

	protected override void OnBeatDone()
	{
		if (startedSpawning)
		{
			if (++currentBeatCount >= spawnRate)
			{
				currentBeatCount = 0;
				SpawnDude();
			}
		}

		if (!guardTriggered && Player.Instance.CurrentPosition == positionToTriggerDamagingGuardCutScene)
		{
			guardTriggered = true;
			GameSessionManager.Instance.PlayCutScene(damagingGuardCutScene);
		}

		if (!buttonTriggered && Player.Instance.CurrentPosition == buttonTriggerPosition)
		{
			buttonTriggered = true;
			GameSessionManager.Instance.PlayCutScene(buttonTriggeredCutScene);
		}
	}

	public void SpawnDude()
	{
		if (sceneObjects.currentWorld.CanWalk(spawnPoint1).Item1 == GameTile.CantMoveReason.None)
		{
			sceneObjects.currentMobManager.SpawnMob(enemyToSpawn, spawnPoint1);
		}

		if (sceneObjects.currentWorld.CanWalk(spawnPoint2).Item1 == GameTile.CantMoveReason.None)
		{
			sceneObjects.currentMobManager.SpawnMob(enemyToSpawn, spawnPoint2);
		}
	}

	public void StartSpawning()
	{
		currentBeatCount = 0;
		startedSpawning = true;
		SpawnDude();
	}

	public void CloseDoors()
	{
		DespawnAllEnemies();
		foreach (var doorPosition in placeDoor.doorPosition)
		{
			sceneObjects.currentObstacleManager.SpawnObstacle("BlockingObstacle", doorPosition);
		}

		sceneObjects.currentObstacleManager.RemoveUnit(sceneObjects.currentWorld
			.GetTile(removeDoor1.doorPosition.First())
			.ObstacleOnTile);
		sceneObjects.currentObstacleManager.RemoveUnit(sceneObjects.currentWorld
			.GetTile(removeDoor2.doorPosition.First())
			.ObstacleOnTile);
		placeDoor.doorAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		removeDoor1.doorAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		removeDoor2.doorAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
	}

	private void DespawnAllEnemies()
	{
		sceneObjects.currentMobManager.RemoveAllUnits();
	}

	public void TurnAroundGuard()
	{
		damagingGuard.TurnAround();
	}
}