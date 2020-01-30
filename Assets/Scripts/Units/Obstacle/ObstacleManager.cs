using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : UnitManager<Obstacle>
{
	private const string OBSTACLE_PATH = "Obstacles";

	public void SpawnItemOnGround(Item item, int amount, Vector2Int spawnPoint)
	{
		var itemOnGroundPrefab = Resources.Load<ItemOnGround>($"{OBSTACLE_PATH}/{item.itemName}");
		var itemOnGround = Instantiate(itemOnGroundPrefab, transform);
		itemOnGround.initializeSelf = false;
		itemOnGround.amount = amount;
		InitializeUnit(itemOnGround, spawnPoint);
	}

	public void SpawnObstacle(Obstacle obstaclePrefab, Vector2Int spawnPoint)
	{
		var obstacle = Instantiate(obstaclePrefab, transform);
		obstacle.initializeSelf = false;
		InitializeUnit(obstacle, spawnPoint);
	}

	public bool CanSpawnOnLocation(Vector2Int location)
	{
		var world = GameSessionManager.Instance.currentSceneObjects.currentWorld;
		var (cantMoveReason, _) = world.CanWalk(location);
		return cantMoveReason == GameTile.CantMoveReason.None;
	}

	public (bool canSpawn, MovementDirectionUtilities.MovementDirection movementDirection) CanSpawnAroundLocation(
		Vector2Int location)
	{
		var world = GameSessionManager.Instance.currentSceneObjects.currentWorld;
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

	public void ApplyItemData(List<ItemOnGround.ItemOnGroundData> itemDatas)
	{
		foreach (var itemData in itemDatas)
		{
			if (CanSpawnOnLocation(itemData.currentPosition))
			{
				SpawnItemOnGround(ItemManager.Instance.GetItemByName(itemData.identifierName), itemData.amount,
					itemData.currentPosition);
			}
			else
			{
				Debug.Log(
					$"Can't spawn item {itemData.identifierName} due to location being occupied: {itemData.currentPosition}");
			}
		}
	}
}