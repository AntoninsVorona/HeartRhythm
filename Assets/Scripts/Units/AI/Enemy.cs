using System;
using UnityEngine;

public class Enemy : Mob
{
	[Serializable]
	public class EnemyInteractionWithPlayerData
	{
		public int damageOnContact;
		public bool dissipateOnContact = true;
		public float equalizerShake = 5f;
	}

	[SerializeField]
	protected EnemyInteractionWithPlayerData enemyInteractionWithPlayerData;

	protected override void InteractWithObject(Unit unit)
	{
		base.InteractWithObject(unit);
		if (unit is Player player)
		{
			if (enemyInteractionWithPlayerData.damageOnContact > 0)
			{
				player.TakeDamage(enemyInteractionWithPlayerData.damageOnContact, enemyInteractionWithPlayerData.equalizerShake);
			}

			if (enemyInteractionWithPlayerData.dissipateOnContact)
			{
				Die();
			}
		}
	}
}