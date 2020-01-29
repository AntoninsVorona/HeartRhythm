using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Head Set Find Interaction",
	fileName = "HeadSetFindInteraction")]
public class HeadSetFindInteraction : StartCutSceneInteraction
{
	public override bool ApplyInteraction()
	{
		HeadSetTrashPiles.Interacted();
		GameLogic.Instance.PlayCutScene(cutSceneToStart);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}