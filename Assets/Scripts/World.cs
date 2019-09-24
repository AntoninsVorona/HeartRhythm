using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

	public static World Instance { get; private set; }
}