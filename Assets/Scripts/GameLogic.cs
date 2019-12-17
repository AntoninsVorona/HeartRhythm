using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	private LevelData currentLevelData;
	private GameState gameState;
	private Scene realWorldScene;
	private SceneObjects realWorldSceneObjects;
	private Vector2Int previousPlayerPosition;
	private Scene battleScene;
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

	public CutScene debugCutScene;

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
	}

	private IEnumerator Start()
	{
		currentLevelData =
			debugLevelToLoad ? debugLevelToLoad : LevelManager.Instance.GetLevelData("TestLevel"); //TODO Load
		yield return LoadLevelCoroutine(debugLevelToLoad.GetSpawnPoint(0)); //TODO Load
	}

	public void LoadLevel(LevelData levelToEnter, int entranceId)
	{
		if (currentLevelData)
		{
			if (currentLevelData.dialogueRegistrator)
			{
				currentLevelData.dialogueRegistrator.UnregisterDialogueFunctions();
			}
		}

		currentLevelData = levelToEnter;
		StartCoroutine(LoadLevelCoroutine(currentLevelData.GetSpawnPoint(entranceId)));
	}

	private IEnumerator LoadLevelCoroutine(Vector2Int spawnPoint)
	{
		PreLoadSequence();
		if (currentLevelData.dialogueRegistrator)
		{
			currentLevelData.dialogueRegistrator.RegisterDialogueFunctions();
		}

		if (currentSceneObjects)
		{
			Destroy(currentSceneObjects.gameObject);
		}

		currentSceneObjects = Instantiate(currentLevelData.sceneObjects);
		currentSceneObjects.Activate();
		yield return currentSceneObjects.currentWorld.InitializeWorld();
		Player.Instance.Initialize(spawnPoint);
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
		if (debugCutScene)
		{
			PlayCutScene(debugCutScene);
		}
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
		PreLoadSequence();
		realWorldScene = SceneManager.GetActiveScene();
		previousPlayerPosition = Player.Instance.CurrentPosition;
		currentSceneObjects.currentWorld.UnoccupyTargetTile(previousPlayerPosition);
		realWorldSceneObjects = currentSceneObjects;
		if (!battleScene.isLoaded)
		{
			yield return LoadBattleScene();
		}

		SceneManager.SetActiveScene(battleScene);
		currentSceneObjects = Instantiate(battleConfiguration.sceneObjects);
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
		PreLoadSequence();
		if (currentSceneObjects)
		{
			Destroy(currentSceneObjects.gameObject);
		}

		currentSceneObjects = realWorldSceneObjects;
		SceneManager.SetActiveScene(realWorldScene);
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

	private void PreLoadSequence()
	{
		PlayerInput.Instance.acceptor.DontReceiveAnyInput = true;
		PlayerInput.Instance.acceptor.FirstBattleInputDone = false;
		GameUI.Instance.StartLoading();
		if (currentSceneObjects)
		{
			currentSceneObjects.currentMobManager.StopAllActionsBeforeLoading();
			currentSceneObjects.Deactivate();
		}

		Player.Instance.StopTalk(true);
	}

	private static void PostLoadSequence()
	{
		PlayerInput.Instance.acceptor.DontReceiveAnyInput = false;
		GameUI.Instance.StopLoading();
	}

	private IEnumerator LoadBattleScene()
	{
		const string sceneName = "BattleScene";
		var sceneAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		yield return new WaitUntil(() => sceneAsync.isDone);
		battleScene = SceneManager.GetSceneByName(sceneName);
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

	public static GameLogic Instance { get; private set; }
}