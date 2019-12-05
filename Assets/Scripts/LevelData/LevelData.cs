using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Level/Level", fileName = "Level")]
public class LevelData : ScriptableObject
{
	public Vector2Int playerSpawnPoint;
	public SceneObjects sceneObjects;
}