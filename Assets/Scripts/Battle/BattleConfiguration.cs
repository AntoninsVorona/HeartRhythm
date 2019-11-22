using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Battle/Battle Configuration", fileName = "BattleConfiguration")]
public class BattleConfiguration : ScriptableObject
{
	public Grid customGrid;
	public MobController mobController;
	public Vector2Int playerSpawnPoint;
}