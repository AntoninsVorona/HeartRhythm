using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Head Set Take Back Interaction",
	fileName = "HeadSetTakeBackInteraction")]
public class HeadSetTakeBackInteraction : StartCutSceneInteraction
{
	public override bool ApplyInteraction()
	{
		HeadSetHidePlace.HeadSetTakenBack();
		GameSessionManager.Instance.PlayCutScene(cutSceneToStart);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}