using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Battle Area", fileName = "Battle Area")]
public class BattleArea : LevelData
{
	[Serializable]
	public class BattleDamage
	{
		public int damage;
		public float equalizerShake;

		public BattleDamage(int damage, float equalizerShake)
		{
			this.damage = damage;
			this.equalizerShake = equalizerShake;
		}
	}
	
	[Serializable]
	public class BattleSettings
	{
		public int startingHp = 50;
		public int maxHp = 100;
		public BattleDamage missedBeatDamage = new BattleDamage(1, 2.5f);
		public BattleDamage invalidInputDamage = new BattleDamage(0, 2.5f);
		public int hitsInARowToHeal = 5;
	}

	public bool autoStartBattle = true;
	public Music battleMusic;
	public BattleSettings battleSettings;
}