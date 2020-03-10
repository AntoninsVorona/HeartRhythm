using System;
using System.Collections;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private HeartButton continueButton;

	[SerializeField]
	private HeartButton quitButton;

	[SerializeField]
	private AudioSource heartBeat;
	
	private bool fadeDone;
	private bool scanButtons;
	private GameOverFillingButton lastSelectedButton;

	private void Update()
	{
		if (scanButtons)
		{
			var hit = AbstractMainMenu.Instance.CurrentUIHit();
			if (hit)
			{
				var gameOverFillingButton = hit.GetComponentInParent<GameOverFillingButton>();
				if (gameOverFillingButton)
				{
					if (gameOverFillingButton != lastSelectedButton)
					{
						if (lastSelectedButton)
						{
							lastSelectedButton.Deselect();
						}

						lastSelectedButton = gameOverFillingButton;
						lastSelectedButton.Select();
					}

					if (Input.GetMouseButtonDown(0))
					{
						gameOverFillingButton.Click();
					}
				}
			}
		}
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public Coroutine Show()
	{
		gameObject.SetActive(true);
		continueButton.ResetFill();
		quitButton.ResetFill();
		continueButton.Deselect();
		quitButton.Deselect();
		fadeDone = false;
		scanButtons = false;
		return StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		yield return new WaitUntil(() => fadeDone);
		scanButtons = true;
	}

	public void FadeDone()
	{
		fadeDone = true;
	}

	public void ContinueButton()
	{
		scanButtons = false;
		var latestSave = SaveSystem.GetLatestSave();
		if (string.IsNullOrEmpty(latestSave))
		{
			return;
		}

		GameLogic.Instance.LoadSave(latestSave, false);
	}
	
	public void QuitButton()
	{
		scanButtons = false;
		GameLogic.Instance.LoadMainMenuScene();
	}

	public void HeartBeat()
	{
		heartBeat.Play();
	}

	public void GameOverSquare()
	{
		Player.Instance.ActivateGameOverSquare();
	}
}