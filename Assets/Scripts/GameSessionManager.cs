using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class GameSessionManager : MonoBehaviour
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

	private LevelData currentLevelData;

	[HideInInspector]
	public SceneObjects.LevelState currentLevelState;

	private GameState gameState;
	private Vector2Int playerPositionBeforeBattle;
	private LevelData levelBeforeBattle;
	private Music fightMusic;
	private CutScene currentCutScene;

	[HideInNormalInspector]
	public SceneObjects currentSceneObjects;

	[HideInInspector]
	public List<Observer> gameStateObservers;

	[HideInInspector]
	public PlayState playState;

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

		InitVariables();
	}

	private void Start()
	{
		CloseMainMenu();
	}

	private void InitVariables()
	{
		gameStateObservers = new List<Observer>();
		playState = PlayState.Basic;
		gameState = GameState.Peace;
	}

#if UNITY_EDITOR
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F9) && currentCutScene == null)
		{
			var latestSave = SaveSystem.GetLatestSave();
			if (!string.IsNullOrEmpty(latestSave))
			{
				GameLogic.Instance.LoadSave(latestSave, false);
			}
		}

		if (Input.GetKeyDown(KeyCode.F5) && currentCutScene == null)
		{
			GameLogic.Instance.Save();
		}
	}
#endif

	public void OpenMainMenu()
	{
		((InGameMainMenu) AbstractMainMenu.Instance).Open();
		PlayerInput.Instance.acceptor.MainMenuOpened = true;
	}

	public void CloseMainMenu()
	{
		((InGameMainMenu) AbstractMainMenu.Instance).Close();
		PlayerInput.Instance.acceptor.MainMenuOpened = false;
	}

	public void ApplyCancelToMainMenu()
	{
		((InGameMainMenu) AbstractMainMenu.Instance).ApplyCancel();
	}

	public Coroutine BackToRealWorld()
	{
		return LoadLevel(levelBeforeBattle, playerPositionBeforeBattle,
			levelBeforeBattle is BattleArea ? GameState.Fight : GameState.Peace);
	}

	public Coroutine LoadLevel(BattleConfiguration levelToEnter)
	{
		return LoadLevel(levelToEnter, 0, CurrentGameState);
	}

	public Coroutine LoadLevel(LevelData levelToEnter, int entranceId)
	{
		return LoadLevel(levelToEnter, entranceId, CurrentGameState);
	}

	public Coroutine LoadLevel(LevelData levelToEnter, int entranceId, GameState newGameState)
	{
		return LoadLevel(levelToEnter, levelToEnter.GetSpawnPoint(entranceId), newGameState);
	}

	public Coroutine LoadLevel(LevelData levelToEnter, Vector2Int spawnPoint)
	{
		return LoadLevel(levelToEnter, spawnPoint, CurrentGameState);
	}

	public Coroutine LoadLevel(LevelData levelToEnter, Vector2Int spawnPoint, GameState newGameState)
	{
		if (currentLevelData)
		{
			if (currentLevelData.dialogueRegistrator)
			{
				currentLevelData.dialogueRegistrator.UnregisterDialogueFunctions();
			}

			if (currentLevelData is BattleConfiguration)
			{
				spawnPoint = playerPositionBeforeBattle;
				GameUI.Instance.BackToRealWorld();
			}
			else
			{
				if (levelToEnter is BattleConfiguration)
				{
					playerPositionBeforeBattle = Player.Instance.CurrentPosition;
					levelBeforeBattle = currentLevelData;
					SaveSystem.currentGameSave.UpdateLevelState(true);
				}
				else
				{
					SaveSystem.currentGameSave.UpdateLevelState(false);
				}
			}
		}

		currentLevelData = levelToEnter;
		return StartCoroutine(LoadLevelCoroutine(spawnPoint, newGameState));
	}

	private IEnumerator LoadLevelCoroutine(Vector2Int spawnPoint, GameState newGameState)
	{
		PreLoadSequence(currentLevelData.cameraIsStatic, currentLevelData.focusPosition);
		currentLevelState = SaveSystem.currentGameSave.GetLevelState(currentLevelData.name);
		if (currentLevelData.dialogueRegistrator)
		{
			currentLevelData.dialogueRegistrator.RegisterDialogueFunctions();
		}

		if (currentSceneObjects)
		{
			Destroy(currentSceneObjects.gameObject);
		}

		currentSceneObjects = Instantiate(currentLevelData.sceneObjects);
		currentSceneObjects.Initialize(currentLevelData.name);
		currentSceneObjects.Activate();
		yield return currentSceneObjects.currentWorld.InitializeWorld();
		Player.Instance.Initialize(spawnPoint);
		var updateGameState = true;
		if (currentLevelData is BattleConfiguration battleConfiguration)
		{
			Player.Instance.InitializeFightWithEnemyCombatData();
			GameUI.Instance.FightAnEnemy();

			if (CurrentGameState != GameState.Fight)
			{
				BeginFightMode(battleConfiguration.battleMusic);
			}

			updateGameState = false;
		}

		currentSceneObjects.currentObstacleManager.ApplyItemData(currentLevelState.GetItemData());
		if (currentLevelData is BattleArea battleArea)
		{
			BeginFightMode(battleArea.battleMusic);
			updateGameState = false;
		}

		if (updateGameState)
		{
			if (CurrentGameState == newGameState)
			{
				GameStateChanged();
			}
			else
			{
				CurrentGameState = newGameState;
			}
		}

		PostLoadSequence();
	}

	private void PreLoadSequence(bool cameraIsStatic, Vector2Int focusPosition)
	{
		PlayerInput.Instance.acceptor.DontReceiveAnyInput = true;
		PlayerInput.Instance.acceptor.FirstBattleInputDone = false;
		LoadingUI.Instance.StartLoading();
		if (currentSceneObjects)
		{
			currentSceneObjects.currentMobManager.StopAllActionsBeforeLoading();
			currentSceneObjects.Deactivate();
		}

		Player.Instance.StopTalk(true);

		GameCamera.Instance.staticView = false;
		if (cameraIsStatic)
		{
			GameCamera.Instance.staticView = false;
			GameCamera.Instance.ChangeTargetPosition((Vector3Int) focusPosition + new Vector3(0.5f, 0.5f), true);
			GameCamera.Instance.staticView = true;
		}
	}

	private void PostLoadSequence()
	{
		if (!currentCutScene)
		{
			PlayerInput.Instance.acceptor.DontReceiveAnyInput = false;
		}

		LoadingUI.Instance.StopLoading();
	}

	private void GameStateChanged()
	{
		switch (CurrentGameState)
		{
			case GameState.Peace:
				AudioManager.Instance.StopBeat();
				break;
			case GameState.Fight:
				AudioManager.Instance.InitializeMusic(fightMusic, true);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		gameStateObservers.RemoveAll(o => o.owner == null);
		gameStateObservers.ForEach(o => o?.NotifyBegin());
	}

	public void StartConversation(string conversationTitle)
	{
		PreConversationStart();
		DialogueManager.instance.StartConversation(conversationTitle, Player.Instance.transform);
	}

	public void StartConversation(string conversationTitle, Unit conversationWith)
	{
		PreConversationStart();
		DialogueManager.instance.StartConversation(conversationTitle, Player.Instance.transform,
			conversationWith.transform);
	}

	private static void PreConversationStart()
	{
		if (GameUI.Instance.uiInventory.open)
		{
			GameUI.Instance.uiInventory.Toggle();
		}

		Player.Instance.StopTalk(true);
		PlayerInput.Instance.ConversationStarted();
	}

	public void EndConversation()
	{
		PlayerInput.Instance.ConversationFinished();
		if (currentCutScene != null)
		{
			currentCutScene.dialogueFinished = true;
		}
	}

	public void PlayCutScene(CutScene cutScene)
	{
		Player.Instance.StopTalk(true);
		currentCutScene = Instantiate(cutScene);
		PlayerInput.Instance.acceptor.DontReceiveAnyInput = true;
		PlayerInput.Instance.acceptor.FirstBattleInputDone = false;
		GameUI.Instance.CutSceneStarted();
		currentCutScene.StartCutScene();
	}

	public void CutSceneFinished()
	{
		currentCutScene = null;
		StartCoroutine(CutSceneFinishedSequence());
	}

	private IEnumerator CutSceneFinishedSequence()
	{
		GameUI.Instance.CutSceneFinished();
		yield return new WaitForSeconds(1);
		PlayerInput.Instance.acceptor.DontReceiveAnyInput = false;
	}

	public SceneObjects.LevelState GetLevelState(bool includeEverything)
	{
		return currentSceneObjects.GetLevelState(includeEverything);
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
		GameUI.Instance.danceMoveUI.Deactivate(false);
		if (backToIdle)
		{
			Player.Instance.BackToIdleAnimation();
		}

		yield return new WaitForSeconds(0.5f);
		PlayerInput.Instance.DanceMoveFinished();
		playState = PlayState.Basic;
	}

	public void BeginFightMode(Music fightMusic)
	{
		if (CurrentGameState == GameState.Fight)
		{
			return;
		}

		if (GameUI.Instance.uiInventory.open)
		{
			GameUI.Instance.uiInventory.Toggle();
		}

		this.fightMusic = fightMusic;
		CurrentGameState = GameState.Fight;
	}

	public void StartDanceMove(Unit interactingWith)
	{
		playState = PlayState.DanceMove;
		GameUI.Instance.danceMoveUI.InitializeInteraction(interactingWith);
		PlayerInput.Instance.DanceMoveStarted();
	}

	public void ToggleMode(Music testFightMusic)
	{
		switch (CurrentGameState)
		{
			case GameState.Peace:
				BeginFightMode(testFightMusic);
				break;
			case GameState.Fight:
				CurrentGameState = GameState.Peace;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public string CurrentLevelName()
	{
		return currentLevelData.name;
	}

	public static GameSessionManager Instance { get; private set; }
}