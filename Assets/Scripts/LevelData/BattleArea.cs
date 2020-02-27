using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Battle Area", fileName = "Battle Area")]
public class BattleArea : LevelData
{
	[Serializable]
	public class BattleSettings
	{
		public int damagePerMissedBeat = 1;
		public float equalizerShake = 2.5f;
		public int hitsInARowToHeal = 5;
	}

	public bool autoStartBattle = true;
	public Music battleMusic;
	public BattleSettings battleSettings;
}