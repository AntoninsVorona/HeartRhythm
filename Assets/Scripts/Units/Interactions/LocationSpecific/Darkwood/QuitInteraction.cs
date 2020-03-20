using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Darkwood/QuitInteraction",
	fileName = "QuitInteraction")]
public class QuitInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		GameLogic.Instance.LoadMainMenuScene();
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}