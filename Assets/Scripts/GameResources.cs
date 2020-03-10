using System;
using UnityEngine;

public class GameResources : MonoBehaviour
{
	[Serializable]
	public class Effects
	{
		public CorruptionFog corruptionFog;
	}

	public Effects effects;

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

	public static GameResources Instance { get; private set; }
}