using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public UIInventory uiInventory;
	public BeatController beatController;
	public GameObject loading;
	public DanceMoveUI danceMoveUI;
	public Text modeToggler;
	public Animator cutSceneLines;
	public EqualizerController equalizerController;

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

		equalizerController.Initialize();
		cutSceneLines.gameObject.SetActive(false);
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

	public void CutSceneStarted()
	{
		cutSceneLines.gameObject.SetActive(true);
		cutSceneLines.SetTrigger(AnimatorUtilities.SHOW_TRIGGER);
	}
	
	public void CutSceneFinished()
	{
		StartCoroutine(CutSceneFinishedSequence());
	}

	private IEnumerator CutSceneFinishedSequence()
	{
		cutSceneLines.SetTrigger(AnimatorUtilities.HIDE_TRIGGER);
		yield return new WaitForSeconds(1);
		cutSceneLines.gameObject.SetActive(false);	
	}

	public void FightAnEnemy()
	{
		equalizerController.InitializeEqualizerInBattle();
	}

	public void BackToRealWorld()
	{
		equalizerController.Deactivate();
	}

	public static GameUI Instance { get; private set; }
}