using System;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

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
		public bool ConversationInProgress { get; set; }
		public bool CutSceneInProgress { get; set; }
		public bool IgnoreInput { get; set; }
		public bool DontReceiveAnyInput { get; set; }
		public bool MainMenuOpened { get; set; }

		public WrongInputType lastWrongInput;
		public List<MovementDirectionUtilities.MovementDirection> danceMoveSet;

		public bool AcceptInput()
		{
			if (IgnoreInput || DontReceiveAnyInput || ConversationInProgress || GameUI.Instance.uiInventory.open ||
			    MainMenuOpened || CutSceneInProgress)
			{
				return false;
			}

			switch (GameSessionManager.Instance.CurrentGameState)
			{
				case GameSessionManager.GameState.Peace:
					return PlayerReadyForInput;
				case GameSessionManager.GameState.Fight:
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

		public bool CanToggleMainMenu()
		{
			return !ConversationInProgress && !CutSceneInProgress;
		}

		public bool CanToggleInventory()
		{
			return !ConversationInProgress && !CutSceneInProgress &&
			       GameSessionManager.Instance.CurrentGameState != GameSessionManager.GameState.Fight &&
			       !MainMenuOpened;
		}
	}

	[HideInNormalInspector]
	public Acceptor acceptor;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
	}

	private void Start()
	{
		GameSessionManager.Instance.gameStateObservers.Add(new Observer(this, GameStateChanged));
	}

	private void Update()
	{
		if (!acceptor.DontReceiveAnyInput)
		{
			if (acceptor.CanToggleInventory())
			{
				if (Input.GetButtonDown("Inventory"))
				{
					GameUI.Instance.ToggleInventory();
				}
			}

			if (Input.GetButtonDown("Cancel"))
			{
				if (GameUI.Instance.uiInventory.open)
				{
					GameUI.Instance.uiInventory.ApplyCancel();
				}
				else if (acceptor.MainMenuOpened)
				{
					GameSessionManager.Instance.ApplyCancelToMainMenu();
				}
				else if (acceptor.CanToggleMainMenu())
				{
					GameSessionManager.Instance.OpenMainMenu();
				}
				else if (acceptor.ConversationInProgress)
				{
					((HeartRhythmDialogueUI) DialogueManager.DialogueUI).FastForward();
				}
			}

			if (Input.GetButtonDown("Submit"))
			{
				if (GameUI.Instance.uiInventory.open)
				{
					GameUI.Instance.uiInventory.ApplySubmit();
				}
				else if (acceptor.ConversationInProgress)
				{
					((HeartRhythmDialogueUI) DialogueManager.DialogueUI).FastForward();
				}
			}

			int horizontal;
			int vertical;
			switch (GameSessionManager.Instance.CurrentGameState)
			{
				case GameSessionManager.GameState.Peace
					when GameSessionManager.Instance.playState == GameSessionManager.PlayState.Basic:
					horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
					vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
					break;
				case GameSessionManager.GameState.Peace
					when GameSessionManager.Instance.playState == GameSessionManager.PlayState.DanceMove:
				case GameSessionManager.GameState.Fight:
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

					switch (GameSessionManager.Instance.CurrentGameState)
					{
						case GameSessionManager.GameState.Peace:
							break;
						case GameSessionManager.GameState.Fight:
							acceptor.ReceivedInputThisTimeFrame = true;
							acceptor.FirstBattleInputDone = true;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}

					Player.Instance.ReceiveInput(movementDirection);

					switch (GameSessionManager.Instance.CurrentGameState)
					{
						case GameSessionManager.GameState.Peace:
							break;
						case GameSessionManager.GameState.Fight:
							((InGameAudioManager) AudioManager.Instance).ApplyBeat();
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				else
				{
					if (acceptor.FirstBattleInputDone)
					{
						switch (GameSessionManager.Instance.CurrentGameState)
						{
							case GameSessionManager.GameState.Peace:
								break;
							case GameSessionManager.GameState.Fight:
								if (GameLogic.Instance.inputDebugEnabled)
								{
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
								}

								Player.Instance.InvalidInputTime();
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

					if (GameSessionManager.Instance.playState == GameSessionManager.PlayState.DanceMove &&
					    SaveSystem.currentGameSave.globalVariables.maxDanceMoveSymbols > 2)
					{
						Player.Instance.EndDanceMove(true);
					}
				}
			}
		}
	}

	private void GameStateChanged()
	{
		var newGameState = GameSessionManager.Instance.CurrentGameState;
		switch (newGameState)
		{
			case GameSessionManager.GameState.Peace:
				acceptor.PlayerReadyForInput = true;
				break;
			case GameSessionManager.GameState.Fight:
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
		if (acceptor.danceMoveSet.Count == SaveSystem.currentGameSave.globalVariables.maxDanceMoveSymbols)
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
		if (acceptor.FirstBattleInputDone)
		{
			if (GameLogic.Instance.inputDebugEnabled)
			{
				Debug.LogError("Missed the Beat!");
			}

			Player.Instance.MissedBeat();
		}
	}

	public void ConversationStarted()
	{
		acceptor.ConversationInProgress = true;
		acceptor.FirstBattleInputDone = false;
	}

	public void ConversationFinished()
	{
		acceptor.ConversationInProgress = false;
	}

	public static PlayerInput Instance { get; private set; }
}