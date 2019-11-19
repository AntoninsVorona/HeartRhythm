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
    private List<Obstacle> obstaclesOnTile;
    private Unit occupyingUnit = null;

    public void Initialize(List<Obstacle> obstaclesOnTile)
    {
        this.obstaclesOnTile = obstaclesOnTile;
    }

    public (CantMoveReason, Unit) CanWalk()
    {
        if (!walkable)
        {
            return (CantMoveReason.NonWalkable, null);
        }

        if (obstaclesOnTile.Count > 0)
        {
            return (CantMoveReason.Obstacles, obstaclesOnTile.First());
        }

        if (occupyingUnit)
        {
            return (CantMoveReason.Unit, occupyingUnit);
        }

        return (CantMoveReason.None, null);
    }

    public void AddObstacleOnTop(Obstacle newObstacle)
    {
        obstaclesOnTile.Add(newObstacle);
    }

    public void DestroyFirstObstacle()
    {
        if (obstaclesOnTile.Count > 0)
        {
            var obstacle = obstaclesOnTile.First();
            obstaclesOnTile.Remove(obstacle);
            obstacle.GetDestroyed();
        }
    }

    public void BecomeOccupied(Unit unit)
    {
        occupyingUnit = unit;
    }

    public void Unoccupied()
    {
        occupyingUnit = null;
    }
}