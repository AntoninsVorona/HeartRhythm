using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{
	private const string LEVEL_STORAGE_PATH = "Levels/";
	private const string MAIN_MENU_SCENE = "MainMenu";
	private const string GAME_SCENE = "Game";
	
	public NewGameCutScene newGameCutScene;
	
	[Header("Debug")]
	public LevelData debugLevelToLoad;

	public int debugMaxDanceMoveSymbols = 2;

	public bool wearsHeadset;

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
		if (SceneManager.GetActiveScene().name == MAIN_MENU_SCENE)
		{
			((MainMenu) AbstractMainMenu.Instance).Show();
		}
		else
		{
			SaveSystem.MakeDefaultStartingGameSave(debugLevelToLoad.name, debugMaxDanceMoveSymbols);
			SaveSystem.currentGameSave.globalVariables.wearsHeadset = wearsHeadset;
			Player.Instance.ApplyUnitData(SaveSystem.currentGameSave.playerData);
			GameSessionManager.Instance.LoadLevel(debugLevelToLoad, 0);
		}
	}

	public void ToggleMode()
	{
		GameSessionManager.Instance.ToggleMode(testFightMusic);
	}

	public void NewGame()
	{
		SaveSystem.NewGame();
		StartCoroutine(LoadGameSequence(true, true));
	}

	public void LoadSave(string filePath, bool fade)
	{
		SaveSystem.LoadSave(filePath);
		StartCoroutine(LoadGameSequence(fade, false));
	}

	private IEnumerator LoadGameSequence(bool fade, bool newGame)
	{
		if (fade)
		{
			yield return AbstractMainMenu.Instance.FadeIntoPlay();
		}

		yield return LoadingUI.Instance.StartLoading();
		yield return SceneManager.LoadSceneAsync(GAME_SCENE);
		var levelData = GetLevelByName(SaveSystem.currentGameSave.currentLevelName);
		Player.Instance.ApplyUnitData(SaveSystem.currentGameSave.playerData);
		PersistentDataManager.ApplySaveData(SaveSystem.currentGameSave.databaseData);
		if (newGame)
		{
			SplashArtController.Instance.Activate();
		}
		else
		{
			SplashArtController.Instance.Deactivate();
		}

		yield return GameSessionManager.Instance.LoadLevel(levelData,
			SaveSystem.currentGameSave.playerData.currentPosition);
		if (newGame)
		{
			GameSessionManager.Instance.PlayCutScene(newGameCutScene);
		}
	}

	public SaveSystem.UILoadData Save()
	{
		SaveSystem.currentGameSave.UpdateLevelState(true);
		return SaveSystem.Save();
	}

	private LevelData GetLevelByName(string currentLevelName)
	{
		return Resources.Load<LevelData>($"{LEVEL_STORAGE_PATH}{currentLevelName}");
	}

	public void LoadMainMenuScene()
	{
		StartCoroutine(LoadMainMenuSceneSequence());
	}

	private IEnumerator LoadMainMenuSceneSequence()
	{
		yield return LoadingUI.Instance.StartLoading();
		yield return SceneManager.LoadSceneAsync(MAIN_MENU_SCENE);
		LoadingUI.Instance.StopLoading();
		((MainMenu) AbstractMainMenu.Instance).Show();
	}

	public bool CanSave()
	{
		return GameSessionManager.Instance.CurrentGameState == GameSessionManager.GameState.Peace &&
		       !PlayerInput.Instance.acceptor.ConversationInProgress &&
		       !GameSessionManager.Instance.IsCutSceneInProgress();
	}

	public static GameLogic Instance { get; private set; }
}