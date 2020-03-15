using System;
using UnityEngine;

public class CoolDudeDetection : Mob
{
	public enum DetectionRadius
	{
		Small = 1,
		Big = 2
	}

	[SerializeField]
	private DetectionRadius detectionRadius = DetectionRadius.Small;

	[SerializeField]
	private SpriteRenderer detectionRadiusRenderer;

	[SerializeField]
	private Sprite smallDetectionRadiusSprite;

	[SerializeField]
	private Sprite bigDetectionRadiusSprite;

	public override void MakeAction()
	{
		if (!((CoolDudeRoomLocationRules) LocationRules.Instance).spotted)
		{
			CheckPlayer();
			base.MakeAction();
			CheckPlayer();
		}
	}

	private void SpotPlayer()
	{
		((CoolDudeRoomLocationRules) LocationRules.Instance).PlayerSpotted(this);
	}

	private void CheckPlayer()
	{
		var playerPosition = Player.Instance.CurrentPosition;
		var xDiff = Math.Abs(Math.Abs(playerPosition.x) - Math.Abs(CurrentPosition.x));
		var yDiff = Math.Abs(Math.Abs(playerPosition.y) - Math.Abs(CurrentPosition.y));
		var diff = Math.Max(xDiff, yDiff);
		if (diff <= (int) detectionRadius)
		{
			SpotPlayer();
		}
	}

	private void OnValidate()
	{
		detectionRadiusRenderer.sprite = detectionRadius == DetectionRadius.Small
			? smallDetectionRadiusSprite
			: bigDetectionRadiusSprite;
	}
}