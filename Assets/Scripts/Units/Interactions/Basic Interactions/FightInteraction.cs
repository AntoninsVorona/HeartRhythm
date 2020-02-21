using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Fight Interaction", fileName = "FightInteraction")]
public class FightInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		GameSessionManager.Instance.LoadLevel(((Enemy) owner).battleArea);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}