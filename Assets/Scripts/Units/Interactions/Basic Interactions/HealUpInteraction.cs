using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Heal Interaction", fileName = "HealInteraction")]
public class HealUpInteraction : Interaction
{
	public int health = 5;

	public override bool ApplyInteraction()
	{
		Player.Instance.Heal(health);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[] { health };
	}
}