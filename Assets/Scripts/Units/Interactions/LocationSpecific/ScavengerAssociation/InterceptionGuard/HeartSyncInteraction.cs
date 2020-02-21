using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Interception Guard/Heart Sync Interaction",
	fileName = "HeartSyncInteraction")]
public class HeartSyncInteraction : Interaction
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