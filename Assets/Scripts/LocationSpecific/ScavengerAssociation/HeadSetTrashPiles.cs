using System;
using System.Collections.Generic;
using UnityEngine;

public class HeadSetTrashPiles : Obstacle
{
	[Header("Additional Data")]
	[SerializeField]
	private List<Vector2Int> otherPoints;

	[SerializeField]
	private Animator animator;

	protected override void Start()
	{
		base.Start();
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER); //TODO Load
	}

	public void Interacted()
	{
		animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
		talksWhenInteractedWith = true;
	}

	protected override void OccupyTile()
	{
		foreach (var point in otherPoints)
		{
			GameLogic.Instance.currentSceneObjects.currentWorld.AddObstacle(point, this);
		}
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