using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InterceptionGuardLocationRules : LocationRules
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
	private bool hisRoomConversationTriggered;
	public Vector2Int hisRoomConversationPoint1;
	public Vector2Int hisRoomConversationPoint2;
	public Obstacle guitar;
	public InterceptionGuardStartCutScene attack;
	public InterceptionGuardStartCutScene sync;
	private bool allObstaclesTriggered;

	[SerializeField]
	private List<Obstacle> leftToInteract;

	private bool startedSpawning;
	private int currentBeatCount;

	protected override void Start()
	{
		base.Start();
		startedSpawning = false;
		guardTriggered = false;
		buttonTriggered = false;
		hisRoomConversationTriggered = false;
		allObstaclesTriggered = false;
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

		if (!hisRoomConversationTriggered && (Player.Instance.CurrentPosition == hisRoomConversationPoint1 ||
		                                      Player.Instance.CurrentPosition == hisRoomConversationPoint2))
		{
			hisRoomConversationTriggered = true;
			GameSessionManager.Instance.StartConversation("InterceptionGuardRoom");
		}
	}

	protected override void OnMoveDone()
	{
		
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
		startedSpawning = false;
		foreach (var doorPosition in placeDoor.doorPosition)
		{
			sceneObjects.currentObstacleManager.SpawnObstacle("BlockingObstacle", doorPosition);
		}

		sceneObjects.currentWorld
			.GetTile(removeDoor1.doorPosition.First()).ObstacleOnTile.Die();
		sceneObjects.currentWorld
			.GetTile(removeDoor2.doorPosition.First()).ObstacleOnTile.Die();
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

	public void DisableStartConversation()
	{
		sceneObjects.currentObstacleManager.ApplyActionOnUnits(u => u.startsConversationUponInteraction = false);
	}

	public void StartStars()
	{
		leftToInteract.ForEach(l => l.animator.SetTrigger(AnimatorUtilities.START_TRIGGER));
	}

	public void RemoveLeftToInteract(Obstacle obstacle)
	{
		leftToInteract.Remove(obstacle);
		if (!allObstaclesTriggered && leftToInteract.Count == 0)
		{
			allObstaclesTriggered = true;
			GameSessionManager.Instance.StartConversation("InterceptionGuardAllObstacles");
			guitar.animator.ResetTrigger(AnimatorUtilities.STOP_TRIGGER);
			guitar.animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
			guitar.interactions.Clear();
			guitar.interactions.Add(attack);
			guitar.interactions.Add(sync);
			guitar.InitializeInteractions();
		}
	}

	public void DisableGuitarStar()
	{
		guitar.animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
	}
}