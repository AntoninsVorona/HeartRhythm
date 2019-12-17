using UnityEngine;

public class Obstacle : Unit
{
	protected override void Start()
	{
		base.Start();
		if (initializeSelf)
		{
			GameLogic.Instance.currentSceneObjects.currentObstacleManager.InitializeObstacle(this, spawnPoint);
		}
	}
	
	protected override void InteractWithObject(Unit unit)
	{
	}

	public override void Die()
	{
		UnoccupyTile();
		GameLogic.Instance.currentSceneObjects.currentObstacleManager.RemoveObstacle(this);
		gameObject.SetActive(false);
	}

	protected override void OccupyTile()
	{
		GameLogic.Instance.currentSceneObjects.currentWorld.AddObstacle(currentPosition, this);
	}

	protected override void UnoccupyTile()
	{
		GameLogic.Instance.currentSceneObjects.currentWorld.RemoveObstacle(currentPosition);
	}

	public override void Talk(string text = null)
	{
		Player.Instance.Talk(talkUI.GetRandomText());
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		var size = new Vector3(1, 1, 0.2f);
		Gizmos.DrawCube(CubeLocation(spawnPoint), size);

		Vector3 CubeLocation(Vector2Int point)
		{
			return (Vector3Int) point + new Vector3(0.5f, 0.5f, 0);
		}
	}
}