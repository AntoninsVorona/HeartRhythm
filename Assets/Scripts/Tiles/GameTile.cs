using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTile : MonoBehaviour
{
	public bool walkable = true;
	private List<Obstacle> obstaclesOnTile;

	public void Initialize(List<Obstacle> obstaclesOnTile)
	{
		this.obstaclesOnTile = obstaclesOnTile;
	}

	public bool CanWalk()
	{
		return walkable && obstaclesOnTile.Count == 0;
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
}