using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EqualizerController : MonoBehaviour
{
	private const int EQUALIZER_MAX_AREA_WIDTH = 1000;
	private const int EQUALIZER_LINE_WIDTH = 50;
	private const int EQUALIZER_MAX_DOTS = 10;
	private const int MIN_POINT = 0;
	private const int MAX_POINT = 19;
	private const float MIN_FILL = 0; // 1f / EQUALIZER_MAX_DOTS;
	private const float FILL_MODIFIER = 1f / EQUALIZER_MAX_DOTS;

	[HideInNormalInspector]
	public bool active;

	[SerializeField]
	private Image equalizerCurrentHp;

	[SerializeField]
	private Image damagedEqualizerBackground;

	[Header("Equalizer Lines")]
	[SerializeField]
	private AnimationCurve slowDownCurve;

	[SerializeField]
	private Image equalizerLinePrefab;

	[SerializeField]
	private RectTransform lineHolder;

	private readonly Dictionary<int, Image> equalizerLines = new Dictionary<int, Image>();
	private Coroutine bumpCoroutine;
	private float currentMaxValue;
	private int currentMaxPoint;

	public void Initialize()
	{
		for (var i = 0; i <= MAX_POINT; i++)
		{
			var line = Instantiate(equalizerLinePrefab, lineHolder);
			equalizerLines.Add(i, line);
			line.rectTransform.anchoredPosition = GetLinePosition(i);
			line.gameObject.SetActive(false);
		}

		Deactivate();
	}

	public void InitializeEqualizerInBattle()
	{
		gameObject.SetActive(true);
		active = true;
	}

	public void Deactivate()
	{
		if (bumpCoroutine != null)
		{
			StopCoroutine(bumpCoroutine);
		}

		currentMaxPoint = 0;
		active = false;
		gameObject.SetActive(false);
	}

	public void CreateBump()
	{
		if (bumpCoroutine != null)
		{
			StopCoroutine(bumpCoroutine);
		}

		bumpCoroutine = StartCoroutine(BumpSequence());
	}

	private IEnumerator BumpSequence()
	{
		var randomHpPoint = RandomHpPoint();
		var firstLine = equalizerLines[randomHpPoint];
		firstLine.fillAmount = 1;

		CreateSideBumps(randomHpPoint, -1);
		CreateSideBumps(randomHpPoint, 1);
		InitLinesBeyondMaxPoint();
		float timePassed = 0;
		while (true)
		{
			var slowDownRate = slowDownCurve.Evaluate(timePassed) * (float) AudioManager.Instance.beatDelay;
			yield return new WaitForSeconds(slowDownRate);
			timePassed += slowDownRate * 3;
			for (var i = 0; i <= currentMaxPoint; i++)
			{
				var line = equalizerLines[i];
				var fillAmount = GetFillAmount(line.fillAmount);
				line.fillAmount = fillAmount;
			}
		}
	}

	private void CreateSideBumps(int startPoint, int modifier)
	{
		var nextPoint = startPoint + modifier;
		var fillModifier = GetFillAmount(1);
		while (nextPoint >= MIN_POINT && nextPoint <= currentMaxPoint)
		{
			var line = equalizerLines[nextPoint];
			line.fillAmount = fillModifier;
			fillModifier = GetFillAmount(fillModifier);
			if (Mathf.Approximately(fillModifier, MIN_FILL))
			{
				fillModifier = GetMinFillAmount();
			}

			nextPoint += modifier;
		}
	}

	private int RandomHpPoint()
	{
		return Mathf.RoundToInt(Random.Range(0f, currentMaxValue) * currentMaxPoint);
	}

	public void UpdateCurrentHp(int currentHp, int maxHp)
	{
		var percentage = (float) currentHp / maxHp;
		equalizerCurrentHp.fillAmount = percentage;
		currentMaxValue = Player.Instance.GetCurrentHp() / Player.Instance.GetMaxHp();
		var prevMaxPoint = currentMaxPoint;
		currentMaxPoint = Mathf.RoundToInt(currentMaxValue * (MAX_POINT + 1)) - 1;
		if (currentMaxPoint < 0)
		{
			currentMaxPoint = 0;
		}

		if (prevMaxPoint > currentMaxPoint)
		{
			for (var i = prevMaxPoint; i > currentMaxPoint; i--)
			{
				equalizerLines[i].gameObject.SetActive(false);
			}
		}
		else
		{
			for (var i = prevMaxPoint; i <= currentMaxPoint; i++)
			{
				var line = equalizerLines[i];
				line.gameObject.SetActive(true);
				line.fillAmount = MIN_FILL;
			}
		}

		damagedEqualizerBackground.fillAmount = (float) (MAX_POINT - currentMaxPoint) / (MAX_POINT + 1);
		ApplyHealthEffects(Mathf.RoundToInt(percentage * 100));
	}

	private void ApplyHealthEffects(int percentage)
	{
		const float minPitch = 0.5f;
		const int basicModeMin = 36;
		const int basicModeMax = 69;
		AudioManager.MusicSettings musicSettings;
		if (percentage >= basicModeMin && percentage <= basicModeMax)
		{
			musicSettings = AudioManager.MusicSettings.DEFAULT_SETTINGS;
		}
		else
		{
			var pitchT = (float) percentage / (basicModeMin - 1);
			var pitch = Mathf.Lerp(minPitch, 1, pitchT);
			musicSettings = new AudioManager.MusicSettings(pitch);
		}
		
		// AudioManager.Instance.ApplyMusicSettings(musicSettings);
	}

	private static Vector2 GetLinePosition(int equalizerPoint)
	{
		return new Vector2(equalizerPoint * EQUALIZER_LINE_WIDTH, 0);
	}

	private float GetFillAmount(float start)
	{
		float modifier = 0;
		switch (Random.Range(0, 5))
		{
			case 0:
			case 1:
				modifier = FILL_MODIFIER;
				break;
			case 2:
			case 3:
				modifier = FILL_MODIFIER * 2;
				break;
			case 4:
				modifier = FILL_MODIFIER * 3;
				break;
		}

		var fillAmount = start - modifier;
		if (fillAmount < MIN_FILL)
		{
			fillAmount = MIN_FILL;
		}

		return fillAmount;
	}

	private float GetMinFillAmount()
	{
		switch (Random.Range(0, 3))
		{
			case 1:
				return FILL_MODIFIER * 3;
			case 2:
				return FILL_MODIFIER * 4;
			default:
				return FILL_MODIFIER * 2;
		}
	}

	private void InitLinesBeyondMaxPoint()
	{
		for (var i = currentMaxPoint + 1; i <= MAX_POINT; i++)
		{
			equalizerLines[i].gameObject.SetActive(false);
		}
	}
}