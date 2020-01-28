using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Unit
{
	[SerializeField]
	private List<Vector2Int> otherPoints;

	protected override void Start()
	{
		base.Start();
		if (initializeSelf)
		{
			var unitData = GameLogic.Instance.currentLevelState.GetDataByName(identifierName);
			GameLogic.Instance.currentSceneObjects.currentObstacleManager.InitializeUnit(this, spawnPoint);
		}
	}

	protected override void InteractWithObject(Unit unit)
	{
	}

	public override void Die()
	{
		UnoccupyTile();
		GameLogic.Instance.currentSceneObjects.currentObstacleManager.RemoveUnit(this);
		gameObject.SetActive(false);
	}

	protected override void OccupyTile()
	{
		GameLogic.Instance.currentSceneObjects.currentWorld.AddObstacle(currentPosition, this);

		foreach (var point in otherPoints)
		{
			GameLogic.Instance.currentSceneObjects.currentWorld.AddObstacle(point, this);
		}
	}

	protected override void UnoccupyTile()
	{
		GameLogic.Instance.currentSceneObjects.currentWorld.RemoveObstacle(currentPosition);
	}

	public override void Talk(string text = null)
	{
		Player.Instance.Talk(talkUI.GetRandomText());
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.red;
		var size = new Vector3(1, 1, 0.2f);
		foreach (var otherPoint in otherPoints)
		{
			Gizmos.DrawCube(CubeLocation(otherPoint), size);
		}

		Vector3 CubeLocation(Vector2Int point)
		{
			return (Vector3Int) point + new Vector3(0.5f, 0.5f, 0);
		}
	}
}