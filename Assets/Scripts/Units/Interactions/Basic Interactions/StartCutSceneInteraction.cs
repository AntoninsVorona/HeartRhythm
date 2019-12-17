using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Start Cut Scene Interaction",
	fileName = "StartCutSceneInteraction")]
public class StartCutSceneInteraction : Interaction
{
	public CutScene cutSceneToStart;
	
	public override bool ApplyInteraction()
	{
		GameLogic.Instance.PlayCutScene(cutSceneToStart);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}