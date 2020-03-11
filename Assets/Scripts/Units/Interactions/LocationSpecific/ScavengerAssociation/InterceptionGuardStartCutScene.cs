using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(
	menuName =
		"Interactions/Location Specific/Scavenger Association/Interception Guard/Interception Guard Start CutScene",
	fileName = "InterceptionGuardStartCutScene")]
public class InterceptionGuardStartCutScene : StartCutSceneInteraction
{
	public bool attack;

	public override bool ApplyInteraction()
	{
		((InterceptionGuardBattleRules)BattleRules.Instance).DisableGuitarStar();
		if (attack)
		{
			//TODO Play Guitar Destroy Animation
			SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.interceptionGuardHeartAttack =
				true;
		}
		else
		{
			Player.Instance.PlayAnimation("Headbang");
		}

		return base.ApplyInteraction();
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}