using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public UIInventory uiInventory;
	public BeatController beatController;
	public GameObject loading;
	public DanceMoveUI danceMoveUI;
	public Text modeToggler;

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

		beatController.Deactivate();
		danceMoveUI.Deactivate();
		uiInventory.Close();
	}

	private void Start()
	{
		GameLogic.Instance.gameStateObservers.Add(new Observer(GameStateChanged));
	}

	private void GameStateChanged()
	{
		var currentGameState = GameLogic.Instance.CurrentGameState;
		modeToggler.text = $"{currentGameState} Mode";
		switch (currentGameState)
		{
			case GameLogic.GameState.Peace:
				break;
			case GameLogic.GameState.Fight:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(currentGameState), currentGameState, null);
		}
	}

	public void StartLoading()
	{
		loading.SetActive(true);
	}

	public void StopLoading()
	{
		loading.SetActive(false);
	}

	public void ToggleInventory()
	{
		uiInventory.Toggle();
	}

	public static GameUI Instance { get; private set; }
}