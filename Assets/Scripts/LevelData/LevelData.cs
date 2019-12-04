using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Level/Level", fileName = "Level")]
public class LevelData : ScriptableObject
{
	public SceneObjects sceneObjects;
}