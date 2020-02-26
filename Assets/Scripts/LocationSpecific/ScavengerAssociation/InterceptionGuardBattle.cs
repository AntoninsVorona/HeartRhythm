using System;
using UnityEngine;

public class InterceptionGuardBattle : MonoBehaviour
{
	public Vector2Int positionWhenStartSpawning;
	public int spawnRate = 3;
	public Vector2Int spawnPoint;
	public Enemy enemyToSpawn;
	private bool startedSpawning;
	private int currentBeatCount;
	private SceneObjects sceneObjects;

	private void Start()
	{
		sceneObjects = GetComponent<SceneObjects>();
		startedSpawning = false;
		currentBeatCount = 3;
		var observer = new Observer(this, OnBeatDone);
		sceneObjects.beatListeners.Add(observer);
	}

	private void OnBeatDone()
	{
		if (startedSpawning)
		{
			if (++currentBeatCount >= spawnRate &&
			    sceneObjects.currentWorld.CanWalk(spawnPoint).Item1 == GameTile.CantMoveReason.None)
			{
				currentBeatCount = 0;
				sceneObjects.currentMobManager.SpawnMob(enemyToSpawn, spawnPoint);
			}
		}
		else if (Player.Instance.CurrentPosition == positionWhenStartSpawning)
		{
			startedSpawning = true;
		}
	}
}