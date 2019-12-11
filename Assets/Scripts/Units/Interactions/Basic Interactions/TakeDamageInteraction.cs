using UnityEngine;
using System;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Take Damage Interaction", fileName = "TakeDamageInteraction")]
public class TakeDamageInteraction : Interaction
{
	public int damage = 5;

	public override bool ApplyInteraction()
	{
		Player.Instance.TakeDamage(damage);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[] { damage };
	}
}