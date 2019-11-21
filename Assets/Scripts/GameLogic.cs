using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
	public enum GameState
	{
		Peace = 0,
		Fight = 1
	}

	public enum PlayState
	{
		Basic = 0,
		DanceMove = 1
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

	[HideInInspector]
	public List<Observer> gameStateObservers = new List<Observer>();

	[HideInInspector]
	public PlayState playState = PlayState.Basic;
	
	[Header("Debug")]
	public Music testMusic;

	public bool inputDebugEnabled;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		fightMusic = testMusic;
		CurrentGameState = GameState.Peace;
		GameStateChanged();
	}

	private void GameStateChanged()
	{
		switch (CurrentGameState)
		{
			case GameState.Peace:
				AudioManager.Instance.StopBeat();
				break;
			case GameState.Fight:
				AudioManager.Instance.InitializeBattle(fightMusic);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		gameStateObservers.ForEach(o => o?.NotifyBegin());
	}

	public void BeginFight(Enemy enemy)
	{
		fightMusic = enemy.fightMusic;
		CurrentGameState = GameState.Fight;
	}

	public void ToggleMode()
	{
		switch (CurrentGameState)
		{
			case GameState.Peace:
				CurrentGameState = GameState.Fight;
				break;
			case GameState.Fight:
				CurrentGameState = GameState.Peace;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void StartDanceMove(Unit interactingWith)
	{
		playState = PlayState.DanceMove;
		GameUI.Instance.danceMoveUI.Initialize(interactingWith);
		GameCamera.Instance.DanceMoveZoomIn();
		PlayerInput.Instance.DanceMoveStarted();
	}

	public void FinishDanceMove(bool backToIdle)
	{
		StartCoroutine(FinishDanceMoveSequence(backToIdle));
	}

	private IEnumerator FinishDanceMoveSequence(bool backToIdle)
	{
		PlayerInput.Instance.acceptor.IgnoreInput = true;
		PlayerInput.Instance.acceptor.FirstBattleInputDone = false;
		yield return new WaitForSeconds(0.5f);
		GameUI.Instance.danceMoveUI.Deactivate();
		if (backToIdle)
		{
			Player.Instance.BackToIdleAnimation();
		}
		GameCamera.Instance.ZoomOut();
		yield return new WaitForSeconds(0.5f);
		PlayerInput.Instance.DanceMoveFinished();
		playState = PlayState.Basic;
	}

	public static GameLogic Instance { get; private set; }
}