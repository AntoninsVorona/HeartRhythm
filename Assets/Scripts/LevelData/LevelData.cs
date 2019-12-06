using System;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Level/Level", fileName = "Level")]
public class LevelData : ScriptableObject
{
	public SceneObjects sceneObjects;

	[Serializable]
	public class EntranceSpawnPoint : SerializableDictionary<int, Vector2Int>
	{
	}

	[SerializeField]
	private EntranceSpawnPoint playerSpawnPoints;

	public Vector2Int GetSpawnPoint(int entranceId)
	{
		return playerSpawnPoints.First(p => p.Key == entranceId).Value;
	}
}