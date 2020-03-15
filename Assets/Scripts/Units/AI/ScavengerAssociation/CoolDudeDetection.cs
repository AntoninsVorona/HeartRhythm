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
		if (!((CoolDudeRoomLocationRules) LocationRules.Instance).caughtPlayer)
		{
			CheckPlayer();
			base.MakeAction();
			CheckPlayer();
		}
	}

	private void SpotPlayer()
	{
		((CoolDudeRoomLocationRules) LocationRules.Instance).caughtPlayer = this;
	}
	
	private void CheckPlayer()
	{
		
	}

	private void OnValidate()
	{
		detectionRadiusRenderer.sprite = detectionRadius == DetectionRadius.Small ? smallDetectionRadiusSprite : bigDetectionRadiusSprite;
	}
}