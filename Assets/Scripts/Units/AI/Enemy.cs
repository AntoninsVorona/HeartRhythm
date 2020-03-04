using System;
using UnityEngine;

public class Enemy : Mob
{
	[Serializable]
	public class EnemyInteractionWithPlayerData
	{
		public BattleArea.BattleDamage damageOnContact = BattleArea.BattleDamage.DEFAULT_BATTLE_DAMAGE;
		public bool dissipateOnContact = true;
	}

	[SerializeField]
	protected EnemyInteractionWithPlayerData enemyInteractionWithPlayerData;

	protected override void InteractWithObject(Unit unit)
	{
		base.InteractWithObject(unit);
		if (unit is Player player)
		{
			if (enemyInteractionWithPlayerData.damageOnContact.damage > 0)
			{
				player.TakeDamage(enemyInteractionWithPlayerData.damageOnContact.damage);
			}

			if (enemyInteractionWithPlayerData.dissipateOnContact)
			{
				Die();
			}
		}
	}
}