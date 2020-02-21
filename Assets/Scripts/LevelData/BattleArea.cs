using UnityEngine;

[CreateAssetMenu(menuName = "Level/Battle Area", fileName = "Battle Area")]
public class BattleArea : LevelData
{
	public bool autoStartBattle = true;
	public Music battleMusic;
}