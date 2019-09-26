using System;
using UnityEngine;

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
		fightMusic = testMusic;
		CurrentGameState = GameState.Fight;
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
		PlayerInput.Instance.GameStateChanged(CurrentGameState);
		Player.Instance.GameStateChanged(CurrentGameState);
	}

	public void BeginFight(Enemy enemy)
	{
		fightMusic = enemy.fightMusic;
		CurrentGameState = GameState.Fight;
	}
	
	public static GameLogic Instance { get; private set; }
}