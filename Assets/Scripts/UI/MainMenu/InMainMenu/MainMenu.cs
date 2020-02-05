using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenu : AbstractMainMenu
{
	[SerializeField]
	private MainMenuIntroController introController;

	[SerializeField]
	private LetterController letterController;

	[SerializeField]
	private CanvasGroup pressAnyKeyText;

	[SerializeField]
	private Animator shaker;

	private Coroutine musicLessHeartBeat;
	private Coroutine pressAnyKey;
	public CustomStandaloneInputModule inputModule;

	public void Show()
	{
		introController.gameObject.SetActive(true);
		pressAnyKeyText.gameObject.SetActive(false);
		StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		introController.Initialize();
		var stages = introController.GetStagesAmount();
		yield return new WaitForSeconds(0.03f);
		for (var i = 1; i < stages; i++)
		{
			yield return new WaitForSeconds(0.97f);
			uiHeart.Beat();
			yield return new WaitForSeconds(0.03f);
			introController.ChangeStage(i);
			if (i + 1 != stages)
			{
				shaker.SetTrigger($"Shake{i}");
			}
		}

		musicLessHeartBeat = StartCoroutine(MusicLessHeartBeat());
		pressAnyKey = StartCoroutine(PressAnyKey());

		yield return new WaitForSeconds(2f);
		yield return new WaitUntil(() => Input.anyKeyDown);
		if (pressAnyKey != null)
		{
			StopCoroutine(pressAnyKey);
		}

		pressAnyKeyText.gameObject.SetActive(false);
		yield return letterController.InitiateFlightSequence(uiHeart.transform.position);
		yield return OpenScreen<MainScreen>();
	}

	private IEnumerator MusicLessHeartBeat()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			uiHeart.Beat();
		}
	}

	private IEnumerator PressAnyKey()
	{
		yield return new WaitForSeconds(2.5f);
		float t = 0;
		pressAnyKeyText.gameObject.SetActive(true);
		pressAnyKeyText.alpha = 0;
		while (t < 1)
		{
			yield return new WaitForFixedUpdate();
			t += Time.fixedDeltaTime;
			pressAnyKeyText.alpha = t;
		}
	}

	protected override CustomStandaloneInputModule GetModule()
	{
		return inputModule;
	}
}