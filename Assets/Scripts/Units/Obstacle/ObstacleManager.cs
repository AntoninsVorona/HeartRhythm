using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
	private const string OBSTACLE_PATH = "Obstacles";
	private readonly List<Obstacle> allObstacles = new List<Obstacle>();

	public void SpawnItemOnGround(Item item, int amount, Vector2Int spawnPoint)
	{
		var itemOnGroundPrefab = Resources.Load<ItemOnGround>($"{OBSTACLE_PATH}/{item.itemName}");
		var itemOnGround = Instantiate(itemOnGroundPrefab, transform);
		itemOnGround.initializeSelf = false;
		itemOnGround.amount = amount;
		InitializeObstacle(itemOnGround, spawnPoint);
	}
	
	public void SpawnObstacle(Obstacle obstaclePrefab, Vector2Int spawnPoint)
	{
		var obstacle = Instantiate(obstaclePrefab, transform);
		obstacle.initializeSelf = false;
		InitializeObstacle(obstacle, spawnPoint);
	}
	
	public void InitializeObstacle(Obstacle obstacle, Vector2Int spawnPoint)
	{
		obstacle.Initialize(spawnPoint);
		allObstacles.Add(obstacle);
	}

	public (bool canSpawn, MovementDirectionUtilities.MovementDirection movementDirection) CanSpawnAroundLocation(Vector2Int location)
	{
		var world = GameLogic.Instance.currentSceneObjects.currentWorld;
		var movementDirections = EnumUtilities.GetValues<MovementDirectionUtilities.MovementDirection>();
		movementDirections.Remove(MovementDirectionUtilities.MovementDirection.None);
		foreach (var direction in movementDirections)
		{
			var (cantMoveReason, _) = world.CanWalkOnTileInDirection(location, direction);
			if (cantMoveReason == GameTile.CantMoveReason.None)
			{
				return (true, direction);
			}
		}

		return (false, MovementDirectionUtilities.MovementDirection.None);
	}
	
	public void RemoveObstacle(Obstacle obstacle)
	{
		allObstacles.Remove(obstacle);
	}
}