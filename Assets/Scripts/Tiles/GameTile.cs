using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTile : MonoBehaviour
{
	public enum CantMoveReason
	{
		None = 0,
		NonWalkable = 1,
		Obstacles = 2,
		Unit = 3
	}

	public bool walkable = true;
	private Obstacle obstacleOnTile;
	public Obstacle ObstacleOnTile => obstacleOnTile;
	private Unit occupyingUnit = null;

	public (CantMoveReason, Unit) CanWalk()
	{
		if (obstacleOnTile)
		{
			return (CantMoveReason.Obstacles, obstacleOnTile);
		}
		
		if (occupyingUnit)
		{
			return (CantMoveReason.Unit, occupyingUnit);
		}
		
		if (!walkable)
		{
			return (CantMoveReason.NonWalkable, null);
		}

		return (CantMoveReason.None, null);
	}

	public void BecomeOccupied(Unit unit)
	{
		occupyingUnit = unit;
	}

	public void Unoccupied()
	{
		occupyingUnit = null;
	}

	public void AddObstacle(Obstacle obstacle)
	{
		obstacleOnTile = obstacle;
	}

	public void RemoveObstacle()
	{
		obstacleOnTile = null;
	}
}