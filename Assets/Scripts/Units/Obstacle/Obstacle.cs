using System.Collections.Generic;
using UnityEngine;

public class Obstacle : Unit
{
	protected override void Start()
	{
		base.Start();
		if (initializeSelf)
		{
			GameSessionManager.Instance.currentSceneObjects.currentObstacleManager.InitializeUnit(this, spawnPoint);
		}
	}

	protected override void InteractWithObject(Unit unit)
	{
	}

	public override void Die()
	{
		UnoccupyTile();
		GameSessionManager.Instance.currentSceneObjects.currentObstacleManager.RemoveUnit(this);
		if (animator)
		{
			animator.SetTrigger(AnimatorUtilities.DIE_TRIGGER);
		}
		else
		{
			Deactivate();
		}
	}

	public override void Talk(string text = null)
	{
		Player.Instance.Talk(talkUI.GetRandomText());
	}
}