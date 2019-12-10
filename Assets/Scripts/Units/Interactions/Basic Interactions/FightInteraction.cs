using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Fight Interaction", fileName = "FightInteraction")]
public class FightInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		GameLogic.Instance.FightAnEnemy(((Enemy) owner).battleConfiguration);
		return true;
	}
	
	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}