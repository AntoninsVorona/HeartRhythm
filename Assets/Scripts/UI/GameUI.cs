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
	public BlackAnnouncer blackAnnouncer;
	public CanvasGroup fadeAlphaCanvasGroup;
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

		ResetFadeAlpha();
		equalizerController.Initialize();
		cutSceneLines.gameObject.SetActive(false);
		beatController.Deactivate();
		danceMoveUI.Initialize();
		uiInventory.Close();
		blackAnnouncer.Close(true);
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

	public void ResetFadeAlpha()
	{
		fadeAlphaCanvasGroup.alpha = 0;
	}
	
	public void FadeAlpha(float start, float end)
	{
		StartCoroutine(FadeAlphaSequence(start, end));
	}

	private IEnumerator FadeAlphaSequence(float start, float end)
	{
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime;
			float alpha = Mathf.Lerp(start, end, t);
			fadeAlphaCanvasGroup.alpha = alpha;
			yield return null;
		}
	}

	public Coroutine ShowBlackAnnouncer(BlackAnnouncer.AnnouncementData announcementData)
	{
		return blackAnnouncer.Show(announcementData);
	}

	public Coroutine CloseBlackAnnouncer()
	{
		return blackAnnouncer.Close();
	}

	public static GameUI Instance { get; private set; }
}