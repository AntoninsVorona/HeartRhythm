using UnityEngine;

public class GameUI : MonoBehaviour
{
	public BeatController beatController;

	private void Awake()
	{
		Instance = this;
	}

	public static GameUI Instance { get; private set; }
}