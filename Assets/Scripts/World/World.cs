using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
	[Serializable]
	public class GameTiles : SerializableDictionary<Vector2Int, GameTile>
	{
	}
	
	public static readonly Vector2Int END_COORDINATE = new Vector2Int(int.MinValue, int.MinValue);
	public static readonly List<Vector2Int> NEIGHBOURS = new List<Vector2Int>
	{
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(0, -1),
		new Vector2Int(0, 1),
	};

	[HideInInspector]
	public List<Observer> tileMapObservers = new List<Observer>();

	[HideInInspector]
	public bool tileMapInitialized;

	[SerializeField]
	private Grid grid;

	private GameTiles gameTiles;

	public IEnumerator InitializeWorld()
	{
		yield return null;
		InitializeTileMap();
	}

	private void InitializeTileMap()
	{
		var obstacles = new Dictionary<Vector2Int, Obstacle>();
		var allObstacles = GetComponentsInChildren<Obstacle>();
		foreach (var obstacle in allObstacles)
		{
			if (obstacle.initializeSelf)
			{
				continue;
			}

			var position = Vector2Int.FloorToInt(obstacle.transform.position);
			if (!obstacles.ContainsKey(position))
			{
				obstacles.Add(position, obstacle);
			}
			else
			{
				Debug.LogError(
					$"Obstacle {obstacle.name} cannot be placed on position {position}! This position already contains an obstacle!");
			}
		}

		gameTiles = new GameTiles();
		var allTiles = GetComponentsInChildren<GameTile>();

		foreach (var tile in allTiles)
		{
			var position = Vector2Int.FloorToInt(tile.transform.position);
			gameTiles.Add(position, tile);
			if (obstacles.ContainsKey(position))
			{
				tile.AddObstacle(obstacles[position]);
			}
		}

		tileMapInitialized = true;
		tileMapObservers.ForEach(o => o?.NotifyBegin());
	}

	public bool TileExists(Vector2Int position)
	{
		return GetTile(position) != null;
	}
	
	public GameTile GetTile(Vector2Int position)
	{
		return gameTiles.ContainsKey(position)
			? gameTiles[position]
			: null;
	}

	public GameTile GetTileInDirection(Vector2Int position, MovementDirectionUtilities.MovementDirection direction)
	{
		var vectorFromDirection = MovementDirectionUtilities.VectorFromDirection(direction);
		return GameSessionManager.Instance.currentSceneObjects.currentWorld.GetTile(position + vectorFromDirection);
	}

	public (GameTile.CantMoveReason, Unit) CanWalk(Vector2Int position)
	{
		return !gameTiles.ContainsKey(position)
			? (GameTile.CantMoveReason.NonWalkable, null)
			: gameTiles[position].CanWalk();
	}

	public (GameTile.CantMoveReason, Unit) CanWalkOnTileInDirection(Vector2Int position,
		MovementDirectionUtilities.MovementDirection direction)
	{
		var endPosition = position + MovementDirectionUtilities.VectorFromDirection(direction);
		return CanWalk(endPosition);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0.2f));
	}

	public Vector3 GetCellCenterWorld(Vector2Int position)
	{
		return grid.GetCellCenterWorld((Vector3Int) position);
	}

	public void AddUnitToTile(Vector2Int currentPosition, Unit unit)
	{
		if (unit is Obstacle obstacle)
		{
			AddObstacle(currentPosition, obstacle);
		}
		else
		{
			OccupyTargetTile(currentPosition, unit);
		}
	}

	public void RemoveUnitFromTile(Vector2Int currentPosition, Unit unit)
	{
		if (unit is Obstacle)
		{
			RemoveObstacle(currentPosition);
		}
		else
		{
			UnoccupyTargetTile(currentPosition);
		}
	}

	private void OccupyTargetTile(Vector2Int currentPosition, Unit unit)
	{
		if (gameTiles.ContainsKey(currentPosition))
		{
			gameTiles[currentPosition].BecomeOccupied(unit);
		}
		else
		{
			Debug.LogWarning($"Can't occupy. No such tile exist: {currentPosition}");
		}
	}

	private void UnoccupyTargetTile(Vector2Int currentPosition)
	{
		if (gameTiles.ContainsKey(currentPosition))
		{
			gameTiles[currentPosition].Unoccupied();
		}
		else
		{
			Debug.LogWarning($"Can't unoccupy. No such tile exist: {currentPosition}");
		}
	}

	private void AddObstacle(Vector2Int currentPosition, Obstacle obstacle)
	{
		if (gameTiles.ContainsKey(currentPosition))
		{
			if (gameTiles[currentPosition].ObstacleOnTile)
			{
				Debug.LogError(
					$"Obstacle {obstacle.name} cannot be placed on position {currentPosition}! This position already contains an obstacle!");
			}
			else
			{
				gameTiles[currentPosition].AddObstacle(obstacle);
			}
		}
		else
		{
			Debug.LogWarning($"Can't place target obstacle. No such tile exist: {currentPosition}");
		}
	}

	private void RemoveObstacle(Vector2Int currentPosition)
	{
		if (gameTiles.ContainsKey(currentPosition))
		{
			gameTiles[currentPosition].RemoveObstacle();
		}
		else
		{
			Debug.LogWarning($"Can't remove target obstacle. No such tile exist: {currentPosition}");
		}
	}
}