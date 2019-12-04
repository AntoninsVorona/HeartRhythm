using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
	[SerializeField]
	private List<LevelData> levels;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
	}

	public LevelData GetLevelData(string name)
	{
		return levels.FirstOrDefault(l => l.name == name);
	}
	
	public static LevelManager Instance { get; private set; }
}