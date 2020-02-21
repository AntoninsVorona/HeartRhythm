using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Interception Guard/Heart Attack Interaction",
	fileName = "HeartAttackInteraction")]
public class HeartAttackInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}