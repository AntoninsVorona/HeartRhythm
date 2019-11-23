using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
	public enum WrongInputType
	{
		InvalidInputTime = 0,
		AlreadyReceivedAnInput = 1
	}

	public struct Acceptor
	{
		public bool PlayerReadyForInput { get; set; }
		public bool BeatIsValid { get; set; }
		public bool ReceivedInputThisTimeFrame { get; set; }
		public bool FirstBattleInputDone { get; set; }
		public bool IgnoreInput { get; set; }
		public bool DontReceiveAnyInput { get; set; }

		public WrongInputType lastWrongInput;
		public List<MovementDirectionUtilities.MovementDirection> danceMoveSet;

		public bool AcceptInput()
		{
			if (IgnoreInput || DontReceiveAnyInput)
			{
				return false;
			}

			switch (GameLogic.Instance.CurrentGameState)
			{
				case GameLogic.GameState.Peace:
					return PlayerReadyForInput;
				case GameLogic.GameState.Fight:
					if (!BeatIsValid)
					{
						lastWrongInput = WrongInputType.InvalidInputTime;
						return false;
					}

					if (ReceivedInputThisTimeFrame)
					{
						lastWrongInput = WrongInputType.AlreadyReceivedAnInput;
						return false;
					}

					return true;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	public int maxDanceMoveSymbols = 2;

	[HideInNormalInspector]
	public Acceptor acceptor;

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

	private void Start()
	{
		GameLogic.Instance.gameStateObservers.Add(new Observer(GameStateChanged));
	}

	private void Update()
	{
		if (!acceptor.DontReceiveAnyInput)
		{
			if (Input.GetKey(KeyCode.R))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}

			int horizontal;
			int vertical;
			switch (GameLogic.Instance.CurrentGameState)
			{
				case GameLogic.GameState.Peace when GameLogic.Instance.playState == GameLogic.PlayState.Basic:
					horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
					vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
					break;
				case GameLogic.GameState.Peace when GameLogic.Instance.playState == GameLogic.PlayState.DanceMove:
				case GameLogic.GameState.Fight:
					horizontal = Input.GetButtonDown("Left")
						? -1
						: Input.GetButtonDown("Right")
							? 1
							: 0;
					vertical = Input.GetButtonDown("Down")
						? -1
						: Input.GetButtonDown("Up")
							? 1
							: 0;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var movementDirection = MovementDirectionUtilities.DirectionFromInput(horizontal, vertical);
			if (movementDirection != MovementDirectionUtilities.MovementDirection.None)
			{
				if (acceptor.AcceptInput())
				{
					if (GameLogic.Instance.inputDebugEnabled)
					{
						Debug.Log(
							$"Music: {AudioManager.Instance.musicAudioSource.time}"
						);
					}

					switch (GameLogic.Instance.CurrentGameState)
					{
						case GameLogic.GameState.Peace:
							break;
						case GameLogic.GameState.Fight:
							acceptor.ReceivedInputThisTimeFrame = true;
							acceptor.FirstBattleInputDone = true;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					Player.Instance.ReceiveInput(movementDirection);

					switch (GameLogic.Instance.CurrentGameState)
					{
						case GameLogic.GameState.Peace:
							break;
						case GameLogic.GameState.Fight:
							AudioManager.Instance.ApplyBeat();
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else
				{
					if (GameLogic.Instance.inputDebugEnabled)
					{
						switch (GameLogic.Instance.CurrentGameState)
						{
							case GameLogic.GameState.Peace:
								break;
							case GameLogic.GameState.Fight:
								switch (acceptor.lastWrongInput)
								{
									case WrongInputType.InvalidInputTime:
										Debug.LogError("Invalid Input Time!");
										break;
									case WrongInputType.AlreadyReceivedAnInput:
										Debug.LogError("Already Received an Input during this Beat!");
										break;
									default:
										throw new ArgumentOutOfRangeException();
								}

								break;
							default:
								throw new ArgumentOutOfRangeException();
						}
					}
				}
			}
			else if (Input.GetKeyDown(KeyCode.Space))
			{
				if (acceptor.AcceptInput())
				{
					if (GameLogic.Instance.inputDebugEnabled)
					{
						Debug.Log(
							$"Music: {AudioManager.Instance.musicAudioSource.time}"
						);
					}

					if (GameLogic.Instance.playState == GameLogic.PlayState.DanceMove)
					{
						Player.Instance.EndDanceMove(true);
					}
				}
			}
		}
	}

	private void GameStateChanged()
	{
		var newGameState = GameLogic.Instance.CurrentGameState;
		switch (newGameState)
		{
			case GameLogic.GameState.Peace:
				acceptor.PlayerReadyForInput = true;
				break;
			case GameLogic.GameState.Fight:
				acceptor.BeatIsValid = false;
				acceptor.ReceivedInputThisTimeFrame = false;
				acceptor.FirstBattleInputDone = false;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
		}
	}

	public void DanceMoveStarted()
	{
		acceptor.FirstBattleInputDone = false;
		acceptor.danceMoveSet = new List<MovementDirectionUtilities.MovementDirection>();
	}

	public void AddDanceMoveSymbol(MovementDirectionUtilities.MovementDirection movementDirection)
	{
		GameUI.Instance.danceMoveUI.AddSymbol(movementDirection, acceptor.danceMoveSet.Count);
		acceptor.danceMoveSet.Add(movementDirection);
		if (acceptor.danceMoveSet.Count == maxDanceMoveSymbols)
		{
			Player.Instance.EndDanceMove(false);
		}
	}

	public void DanceMoveFinished()
	{
		Player.Instance.ApplyDanceMoveSet(acceptor.danceMoveSet);
	}

	public void MissedBeat()
	{
		if (GameLogic.Instance.inputDebugEnabled)
		{
			Debug.LogError("Missed the Beat!");
		}

		if (GameLogic.Instance.playState == GameLogic.PlayState.DanceMove && acceptor.FirstBattleInputDone)
		{
			Player.Instance.ReceiveInput(MovementDirectionUtilities.MovementDirection.None);
		}
	}

	public static PlayerInput Instance { get; private set; }
}