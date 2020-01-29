using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	private const string LEVEL_STORAGE_PATH = "Levels/";

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

	[HideInInspector]
	public SceneObjects.LevelState currentLevelState;

	private LevelData currentLevelData;
	private GameState gameState;
	private SceneObjects realWorldSceneObjects;
	private Vector2Int previousPlayerPosition;
	private Music fightMusic;
	private CutScene currentCutScene;

	[HideInNormalInspector]
	public SceneObjects currentSceneObjects;

	[HideInInspector]
	public List<Observer> gameStateObservers = new List<Observer>();

	[HideInInspector]
	public PlayState playState = PlayState.Basic;

	[Header("Debug")]
	public LevelData debugLevelToLoad;

	public Music testFightMusic;

	public bool inputDebugEnabled;

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

		SaveSystem.LoadData();
	}

	private void Start()
	{
		const string mainMenuScene = "MainMenu";
		const string gameScene = "Game";
		if (SceneManager.GetActiveScene().name == mainMenuScene)
		{
			MainMenuUI.Instance.Show();
		}
		else
		{
			SaveSystem.MakeDefaultStartingGameSave(debugLevelToLoad.name);
			currentLevelData = debugLevelToLoad;
			currentLevelState = SaveSystem.currentGameSave.GetLevelState(currentLevelData.name);
			Player.Instance.ApplyUnitData(SaveSystem.currentGameSave.playerData);
			StartCoroutine(LoadLevelCoroutine(debugLevelToLoad.GetSpawnPoint(0)));
		}
	}

	public Coroutine LoadLevel(LevelData levelToEnter, int entranceId)
	{
		if (currentLevelData)
		{
			if (currentLevelData.dialogueRegistrator)
			{
				currentLevelData.dialogueRegistrator.UnregisterDialogueFunctions();
			}

			SaveSystem.currentGameSave.UpdateLevelState(false);
		}

		currentLevelData = levelToEnter;
		currentLevelState = SaveSystem.currentGameSave.GetLevelState(currentLevelData.name);
		return StartCoroutine(LoadLevelCoroutine(currentLevelData.GetSpawnPoint(entranceId)));
	}

	private IEnumerator LoadLevelCoroutine(Vector2Int spawnPoint)
	{
		PreLoadSequence(currentLevelData.cameraIsStatic, currentLevelData.focusPosition);
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
		currentSceneObjects.currentObstacleManager.ApplyItemData(currentLevelState.GetItemData());
		if (currentLevelData is BattleArea battleArea)
		{
			BeginFightMode(battleArea.battleMusic);
		}
		else
		{
			CurrentGameState = GameState.Peace;
			GameStateChanged();
		}

		PostLoadSequence();
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

		gameStateObservers.RemoveAll(o => o == null);
		gameStateObservers.ForEach(o => o?.NotifyBegin());
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

	public void FightAnEnemy(BattleConfiguration battleConfiguration)
	{
		StartCoroutine(GoToEnemyRealm(battleConfiguration));
	}

	private IEnumerator GoToEnemyRealm(BattleConfiguration battleConfiguration)
	{
		PreLoadSequence(battleConfiguration.cameraIsStatic, battleConfiguration.focusPosition);
		previousPlayerPosition = Player.Instance.CurrentPosition;
		currentSceneObjects.currentWorld.UnoccupyTargetTile(previousPlayerPosition);
		realWorldSceneObjects = currentSceneObjects;
		currentSceneObjects = Instantiate(battleConfiguration.sceneObjects);
		currentSceneObjects.Initialize(battleConfiguration.name);
		currentSceneObjects.Activate();

		yield return currentSceneObjects.currentWorld.InitializeWorld();
		Player.Instance.Initialize(battleConfiguration.GetSpawnPoint(0));
		Player.Instance.InitializeFightWithEnemyCombatData();
		if (CurrentGameState != GameState.Fight)
		{
			BeginFightMode(battleConfiguration.battleMusic);
		}

		GameUI.Instance.FightAnEnemy();
		PostLoadSequence();
	}

	public void BackToRealWorld(bool enablePieceMode)
	{
		PreLoadSequence(currentLevelData.cameraIsStatic, currentLevelData.focusPosition);
		if (currentSceneObjects)
		{
			Destroy(currentSceneObjects.gameObject);
		}

		currentSceneObjects = realWorldSceneObjects;
		currentSceneObjects.Activate();
		currentSceneObjects.currentMobManager.ResumeAllMobs();
		Player.Instance.Initialize(previousPlayerPosition);
		if (enablePieceMode)
		{
			CurrentGameState = GameState.Peace;
		}

		GameUI.Instance.BackToRealWorld();
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

	public void ToggleMode()
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

	public void StartDanceMove(Unit interactingWith)
	{
		playState = PlayState.DanceMove;
		GameUI.Instance.danceMoveUI.Initialize(interactingWith);
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

		yield return new WaitForSeconds(0.5f);
		PlayerInput.Instance.DanceMoveFinished();
		playState = PlayState.Basic;
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

	public void NewGame()
	{
		SaveSystem.NewGame();
		StartCoroutine(LoadGameSequence(true));
	}

	public void LoadSave(string filePath, bool mainMenu)
	{
		SaveSystem.LoadSave(filePath);
		StartCoroutine(LoadGameSequence(mainMenu));
	}

	private IEnumerator LoadGameSequence(bool mainMenu)
	{
		if (mainMenu)
		{
			yield return MainMenuUI.Instance.FadeIntoPlay();
		}

		currentLevelData = GetLevelByName(SaveSystem.currentGameSave.currentLevelName);
		//TODO Load Scene
		Player.Instance.ApplyUnitData(SaveSystem.currentGameSave.playerData);
		yield return LoadLevelCoroutine(SaveSystem.currentGameSave.playerData.currentPosition);
	}

	private LevelData GetLevelByName(string currentLevelName)
	{
		return Resources.Load<LevelData>($"{LEVEL_STORAGE_PATH}{currentLevelName}");
	}

	public string CurrentLevelName()
	{
		return currentLevelData.name;
	}

	public void Save()
	{
		SaveSystem.currentGameSave.UpdateLevelState(true);
		SaveSystem.Save();
	}

	public static GameLogic Instance { get; private set; }
}