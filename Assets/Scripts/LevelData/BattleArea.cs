using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Battle Area", fileName = "Battle Area")]
public class BattleArea : LevelData
{
	[Serializable]
	public class BattleDamage
	{
		public int damage;

		public BattleDamage(int damage)
		{
			this.damage = damage;
		}
	}
	
	[Serializable]
	public class BattleSettings
	{
		public int startingHp = 50;
		public int maxHp = 100;
		public BattleDamage missedBeatDamage = new BattleDamage(1);
		public BattleDamage invalidInputDamage = new BattleDamage(1);
		public int hitsInARowToHeal = 5;
	}

	public bool autoStartBattle = true;
	public Music battleMusic;
	public BattleSettings battleSettings;
}