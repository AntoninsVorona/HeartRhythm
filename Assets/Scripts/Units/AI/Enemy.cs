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
		if (unit is Player)
		{
			if (enemyInteractionWithPlayerData.dissipateOnContact)
			{
				Die();
			}

			CreateCorruptionFog();
		}
	}

	public void CreateCorruptionFog()
	{
		var corruptionFog = Instantiate(GameResources.Instance.effects.corruptionFog, transform.position + new Vector3(0, 0.5f),
			Quaternion.identity,
			GameSessionManager.Instance.currentSceneObjects.transform);
		corruptionFog.Fly(InteractWithPlayer);
	}

	private void InteractWithPlayer()
	{
		if (enemyInteractionWithPlayerData.damageOnContact.damage > 0)
		{
			Player.Instance.TakeDamage(enemyInteractionWithPlayerData.damageOnContact.damage);
		}
	}
}