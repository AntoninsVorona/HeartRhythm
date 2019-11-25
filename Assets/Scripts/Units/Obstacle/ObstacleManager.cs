using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
	private readonly List<Obstacle> allObstacles = new List<Obstacle>();
	
	public void InitializeObstacle(Obstacle obstacle, Vector2Int spawnPoint)
	{
		obstacle.Initialize(spawnPoint);
		allObstacles.Add(obstacle);
	} 
	
	public void RemoveObstacle(Obstacle obstacle)
	{
		allObstacles.Remove(obstacle);
	}
}