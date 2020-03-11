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
			SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.interceptionGuardHeartAttack =
				true;
		}

		return base.ApplyInteraction();
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}