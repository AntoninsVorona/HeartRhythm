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
		if (SceneManager.GetActiveScene().name == MAIN_MENU_SCENE)
		{
			((MainMenu) AbstractMainMenu.Instance).Show();
		}
		else
		{
			SaveSystem.MakeDefaultStartingGameSave(debugLevelToLoad.name);
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
		StartCoroutine(LoadGameSequence(true));
	}

	public void LoadSave(string filePath, bool fade)
	{
		SaveSystem.LoadSave(filePath);
		StartCoroutine(LoadGameSequence(fade));
		//TODO Dialogue Manager doesn't display text if loaded
	}

	private IEnumerator LoadGameSequence(bool fade)
	{
		if (fade)
		{
			yield return AbstractMainMenu.Instance.FadeIntoPlay();
		}

		LoadingUI.Instance.StartLoading();
		yield return SceneManager.LoadSceneAsync(GAME_SCENE);
		var levelData = GetLevelByName(SaveSystem.currentGameSave.currentLevelName);
		Player.Instance.ApplyUnitData(SaveSystem.currentGameSave.playerData);
		yield return GameSessionManager.Instance.LoadLevel(levelData,
			SaveSystem.currentGameSave.playerData.currentPosition);
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
		LoadingUI.Instance.StartLoading();
		yield return SceneManager.LoadSceneAsync(MAIN_MENU_SCENE);
		LoadingUI.Instance.StopLoading();
		((MainMenu) AbstractMainMenu.Instance).Show();
	}

	public bool CanSave()
	{
		return true; //TODO
	}

	public static GameLogic Instance { get; private set; }
}