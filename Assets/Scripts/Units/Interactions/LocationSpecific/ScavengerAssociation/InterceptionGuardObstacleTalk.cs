using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Interception Guard Obstacle Talk",
	fileName = "InterceptionGuardObstacleTalk")]
public class InterceptionGuardObstacleTalk : Interaction
{
	public string textToTalk;

	public override bool ApplyInteraction()
	{
		owner.animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
		((InterceptionGuardLocationRules) LocationRules.Instance).RemoveLeftToInteract((Obstacle) owner);
		Player.Instance.Talk(textToTalk);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}