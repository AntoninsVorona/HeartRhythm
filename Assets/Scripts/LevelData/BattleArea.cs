using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Battle Area", fileName = "Battle Area")]
public class BattleArea : LevelData
{
	[Serializable]
	public struct BattleDamage
	{
		public int damage;

		public BattleDamage(int damage)
		{
			this.damage = damage;
		}
		
		public static readonly BattleDamage DEFAULT_BATTLE_DAMAGE = new BattleDamage(0);
	}

	[Serializable]
	public struct BattleSettings
	{
		public int startingHp;
		public int maxHp;
		public BattleDamage missedBeatDamage;
		public BattleDamage invalidInputDamage;
		public int hitsInARowToHeal;

		public BattleSettings(int startingHp, int maxHp, BattleDamage missedBeatDamage, BattleDamage invalidInputDamage,
			int hitsInARowToHeal)
		{
			this.startingHp = startingHp;
			this.maxHp = maxHp;
			this.missedBeatDamage = missedBeatDamage;
			this.invalidInputDamage = invalidInputDamage;
			this.hitsInARowToHeal = hitsInARowToHeal;
		}

		public static readonly BattleSettings DEFAULT_BATTLE_SETTINGS =
			new BattleSettings(50, 100, new BattleDamage(1), new BattleDamage(1), 5);
	}

	public bool autoStartBattle = true;
	public Music battleMusic;
	public BattleSettings battleSettings = BattleSettings.DEFAULT_BATTLE_SETTINGS;
}