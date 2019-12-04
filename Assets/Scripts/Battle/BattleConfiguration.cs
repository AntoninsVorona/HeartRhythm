using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Level/Battle Configuration", fileName = "BattleConfiguration")]
public class BattleConfiguration : LevelData
{
	public Vector2Int playerSpawnPoint;
}