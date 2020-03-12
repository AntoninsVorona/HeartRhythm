﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SplashArtController : MonoBehaviour
{
	[Serializable]
	public class SplashArtFrame
	{
		[Multiline]
		public string text;

		public Sprite image;
	}

	[SerializeField]
	private CanvasGroup globalCanvasGroup;

	[SerializeField]
	private List<SplashArtFrame> splashArts;

	[SerializeField]
	private CanvasGroup splashArtHolder;

	[SerializeField]
	private TextMeshProUGUI splashArtText;

	[SerializeField]
	private Image splashArtImage;

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
	}

	public Coroutine PlaySequence()
	{
		globalCanvasGroup.gameObject.SetActive(true);
		return StartCoroutine(Sequence());
	}

	private IEnumerator Sequence()
	{
		SplashArtFrame currentSplashArt = null;
		float t;
		foreach (var s in splashArts)
		{
			if (currentSplashArt != null)
			{
				t = 1;
				while (t > 0)
				{
					t -= Time.deltaTime;
					splashArtHolder.alpha = t;
					yield return null;
				}
			}

			currentSplashArt = s;
			splashArtImage.sprite = currentSplashArt.image;
			splashArtText.text = currentSplashArt.text;
			t = 0;
			while (t < 1)
			{
				t += Time.deltaTime;
				splashArtHolder.alpha = t;
				yield return null;
			}

			yield return new WaitUntil(() => Input.anyKeyDown);
		}

		t = 1;
		while (t > 0)
		{
			t -= Time.deltaTime;
			splashArtHolder.alpha = t;
			yield return null;
		}

		t = 1;
		while (t > 0)
		{
			t -= Time.deltaTime;
			globalCanvasGroup.alpha = t;
			yield return null;
		}

		Deactivate();
	}

	public void Activate()
	{
		globalCanvasGroup.gameObject.SetActive(true);
	}
	
	public void Deactivate()
	{
		globalCanvasGroup.gameObject.SetActive(false);
	}

	public static SplashArtController Instance { get; private set; }
}