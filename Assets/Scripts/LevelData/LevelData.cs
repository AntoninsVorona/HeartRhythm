using System;
using System.Linq;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Level/Level", fileName = "Level")]
public class LevelData : ScriptableObject
{
	public SceneObjects sceneObjects;
	public DialogueRegistrator dialogueRegistrator;
	public bool cameraIsStatic;
	public Color32 backgroundColor = new Color32(22, 19, 19, 255);
	public string proclamationName;

	[HideInInspector]
	public SceneObjects.LevelState levelState;

	[DrawIf("cameraIsStatic", true)]
	public Vector2Int focusPosition;

	[Serializable]
	public class EntranceSpawnPoint : SerializableDictionary<int, Vector2Int>
	{
	}

	[SerializeField]
	private EntranceSpawnPoint playerSpawnPoints = new EntranceSpawnPoint {{0, Vector2Int.zero}};

	public Vector2Int GetSpawnPoint(int entranceId)
	{
		return playerSpawnPoints.First(p => p.Key == entranceId).Value;
	}
}