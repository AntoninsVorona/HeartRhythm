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

	[SerializeField]
	private Music mainMenuMusic;
	
	private Coroutine musicLessHeartBeat;
	private float musicLessHeartBeatDelay = 1;
	private Coroutine pressAnyKey;
	public CustomStandaloneInputModule inputModule;
	private bool fastenUp;

	private void Update()
	{
		if (Input.GetButtonDown("Cancel"))
		{
			ApplyCancel();
		}
	}

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
			uiHeart.Beat(true);
			yield return new WaitForSeconds(0.03f);
			introController.ChangeStage(i);
			if (i + 1 != stages)
			{
				shaker.SetTrigger($"Shake{i}");
			}
		}

		fastenUp = false;
		musicLessHeartBeat = StartCoroutine(MusicLessHeartBeat());
		pressAnyKey = StartCoroutine(PressAnyKey());

		yield return new WaitForSeconds(2f);
		yield return new WaitUntil(() => Input.anyKeyDown);
		if (pressAnyKey != null)
		{
			StopCoroutine(pressAnyKey);
		}

		fastenUp = true;
		musicLessHeartBeatDelay = 0.8f;
		pressAnyKeyText.gameObject.SetActive(false);
		yield return letterController.InitiateFlightSequence(uiHeart.transform.position);
		yield return OpenScreen<MainScreen>();
		StopCoroutine(musicLessHeartBeat);
		musicLessHeartBeat = null;
		AudioManager.Instance.InitializeMusic(mainMenuMusic, false, 0);
		uiHeart.Subscribe();
	}

	private IEnumerator MusicLessHeartBeat()
	{
		while (true)
		{
			yield return new WaitForSeconds(musicLessHeartBeatDelay);
			if (fastenUp)
			{
				musicLessHeartBeatDelay = Mathf.Clamp(musicLessHeartBeatDelay - 0.1f, 0.5f, 1);
			}

			uiHeart.Beat(true);
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

	public override IEnumerator FadeIntoPlay()
	{
		StartCoroutine(FadeMusic());
		return base.FadeIntoPlay();
	}

	private IEnumerator FadeMusic()
	{
		var volume = AudioManager.Instance.GetVolume();
		while (volume > 0)
		{
			yield return null;
			volume -= Time.deltaTime * 3;
			AudioManager.Instance.SetVolume(volume);
		}
	}
}