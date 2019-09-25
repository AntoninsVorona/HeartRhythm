using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	public enum GameState
	{
		Peace = 0,
		Fight = 1
	}

	public GameState CurrentGameState
	{
		get => gameState;
		private set
		{
			if (value != gameState)
			{
				gameState = value;
				GameStateChanged();
			}
		}
	}

	private GameState gameState;
	private Music fightMusic;

	public Music testMusic;
	
	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		CurrentGameState = GameState.Peace;
		GameStateChanged();
		AudioManager.Instance.InitializeBattle(testMusic);
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.R))
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	private void GameStateChanged()
	{
		switch (CurrentGameState)
		{
			case GameState.Peace:
				break;
			case GameState.Fight:
				AudioManager.Instance.InitializeBattle(fightMusic);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void BeginFight(Enemy enemy)
	{
		fightMusic = enemy.fightMusic;
		CurrentGameState = GameState.Fight;
	}
	
	public static GameLogic Instance { get; private set; }
}