using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	[Serializable]
	public class GameTiles : SerializableDictionary<Vector3Int, GameTile>
	{
	}

	[SerializeField]
	private Vector3Int spawnPoint;

	private GameTiles gameTiles;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		StartCoroutine(InitializeWorldAfterAFrame());
	}

	private IEnumerator InitializeWorldAfterAFrame()
	{
		yield return null;
		InitializeWorld();
		Player.Instance.Move(spawnPoint, true);
	}

	private void InitializeWorld()
	{
		var obstacles = new Dictionary<Vector3Int, List<Obstacle>>();
		var allObstacles = GetComponentsInChildren<Obstacle>();
		foreach (var obstacle in allObstacles)
		{
			var position = Vector3Int.FloorToInt(obstacle.transform.position);
			if (!obstacles.ContainsKey(position))
			{
				obstacles.Add(position, new List<Obstacle>());
			}

			obstacles[position].Add(obstacle);
		}

		gameTiles = new GameTiles();
		var allTiles = GetComponentsInChildren<GameTile>();

		foreach (var tile in allTiles)
		{
			var position = Vector3Int.FloorToInt(tile.transform.position);
			gameTiles.Add(position, tile);
			tile.Initialize(obstacles.ContainsKey(position) ? obstacles[position] : new List<Obstacle>());
		}
	}

	public bool CanWalk(Vector3Int position)
	{
		return gameTiles.ContainsKey(position) && gameTiles[position].CanWalk();
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0.2f));
		Gizmos.color = Color.green;
		Gizmos.DrawCube(spawnPoint + new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0.2f));
	}

	public static World Instance { get; private set; }
}