using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public UIInventory uiInventory;
	public BeatController beatController;
	public DanceMoveUI danceMoveUI;
	public Text modeToggler;
	public Animator cutSceneLines;
	public EqualizerController equalizerController;
	public CustomStandaloneInputModule inputModule;
	public RectTransform canvasRect;

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
		danceMoveUI.Initialize();
		uiInventory.Close();
	}

	private void Start()
	{
		GameSessionManager.Instance.gameStateObservers.Add(new Observer(this, GameStateChanged));
	}

	private void GameStateChanged()
	{
		var currentGameState = GameSessionManager.Instance.CurrentGameState;
		modeToggler.text = $"{currentGameState} Mode";
		switch (currentGameState)
		{
			case GameSessionManager.GameState.Peace:
				break;
			case GameSessionManager.GameState.Fight:
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(currentGameState), currentGameState, null);
		}
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
	
	public void CutSceneFinished(bool force = false)
	{
		if (force)
		{
			cutSceneLines.gameObject.SetActive(false);	
		}
		else
		{
			StartCoroutine(CutSceneFinishedSequence());
		}
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