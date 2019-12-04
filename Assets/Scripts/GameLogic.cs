using System;
using System.Collections;
using System.Collections.Generic;
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

	[HideInNormalInspector]
	public SceneObjects currentSceneObjects;

	[HideInNormalInspector]
	public Enemy fightingWith;

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

		currentLevelData = debugLevelToLoad ? debugLevelToLoad : LevelManager.Instance.GetLevelData("TestLevel");

		currentSceneObjects = Instantiate(currentLevelData.sceneObjects);
		currentSceneObjects.Activate();
	}

	private IEnumerator Start()
	{
		yield return currentSceneObjects.currentWorld.InitializeWorld();
		Player.Instance.Initialize(new Vector2Int(0, 0)); //TODO Load
		CurrentGameState = GameState.Peace;
		GameStateChanged();
		GameUI.Instance.StopLoading();
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

	public void BeginFightMode(Music fightMusic)
	{
		this.fightMusic = fightMusic;
		CurrentGameState = GameState.Fight;
	}

	public IEnumerator GoToEnemyRealm(Enemy enemy)
	{
		fightingWith = enemy;
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
		currentSceneObjects = Instantiate(enemy.battleConfiguration.sceneObjects);
		currentSceneObjects.Activate();

		yield return currentSceneObjects.currentWorld.InitializeWorld();
		Player.Instance.Initialize(enemy.battleConfiguration.playerSpawnPoint);
		PostLoadSequence();
	}

	public void BackToRealWorld()
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
		PostLoadSequence();
	}

	private void PreLoadSequence()
	{
		PlayerInput.Instance.acceptor.DontReceiveAnyInput = true;
		PlayerInput.Instance.acceptor.FirstBattleInputDone = false;
		GameUI.Instance.StartLoading();
		currentSceneObjects.currentMobManager.StopAllActionsBeforeLoading();
		currentSceneObjects.Deactivate();
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

	public static GameLogic Instance { get; private set; }
}