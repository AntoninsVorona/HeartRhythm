using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	[SerializeField]
	private RectTransform area;

	[Header("Equalizer Lines")]
	[SerializeField]
	private AnimationCurve slowDownCurve;

	[SerializeField]
	private Image equalizerLinePrefab;

	[SerializeField]
	private RectTransform lineHolder;

	private readonly Dictionary<int, Image> equalizerLines = new Dictionary<int, Image>();
	private Coroutine bumpCoroutine;
	private Coroutine shakeCoroutine;
	private Vector3 shakeRot;
	private float currentMaxValue;
	private int currentMaxPoint;
	private AudioManager.MusicSettings previousMusicSettings;

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
		previousMusicSettings = null;
	}

	public void Deactivate()
	{
		if (bumpCoroutine != null)
		{
			StopCoroutine(bumpCoroutine);
		}

		StopShake();
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
		currentMaxValue = (float) currentHp / maxHp;
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
		const int basicModeMin = 41;
		var corruptionLevel = 0;
		AudioManager.MusicSettings musicSettings;
		//TODO Add color change when very high
		if (percentage >= basicModeMin)
		{
			musicSettings = AudioManager.MusicSettings.DEFAULT_SETTINGS;
		}
		else
		{
			const int corruptionLevel2 = 35;
			const int corruptionLevel3 = 30;
			const int corruptionLevel4 = 25;
			const int corruptionLevel5 = 20;
			const int corruptionLevel6 = 10;
			if (percentage <= corruptionLevel6)
			{
				corruptionLevel = 6;
			}
			else if (percentage <= corruptionLevel5)
			{
				corruptionLevel = 5;
			}
			else if (percentage <= corruptionLevel4)
			{
				corruptionLevel = 4;
			}
			else if (percentage <= corruptionLevel3)
			{
				corruptionLevel = 3;
			}
			else if (percentage <= corruptionLevel2)
			{
				corruptionLevel = 2;
			}
			else
			{
				corruptionLevel = 1;
			}

			const float middleLowPass = 4000;
			var point = (float) percentage / basicModeMin;
			float lowPass;
			if (point > 0.5f)
			{
				var t = Mathf.InverseLerp(0.5f, 1f, point);
				lowPass = Mathf.Lerp(middleLowPass, AudioManager.MusicSettings.NORMAL_LOWPASS, t);
			}
			else
			{
				var t = Mathf.InverseLerp(0, 0.5f, point);
				lowPass = Mathf.Lerp(AudioManager.MusicSettings.MIN_LOWPASS, middleLowPass, t);
			}

			musicSettings = new AudioManager.MusicSettings(lowPass);
		}

		if (previousMusicSettings == null || !musicSettings.Equals(previousMusicSettings))
		{
			previousMusicSettings = musicSettings;
			AudioManager.Instance.ApplyMusicSettings(previousMusicSettings);
		}

		GameUI.Instance.corruption.UpdateCorruption(corruptionLevel);
	}

	public void Shake(float rotPower)
	{
		if (shakeCoroutine != null)
		{
			StopCoroutine(shakeCoroutine);
		}

		shakeCoroutine = StartCoroutine(ShakeSequence(rotPower));
	}

	private IEnumerator ShakeSequence(float rotPower)
	{
		const float duration = 0.15f;
		const int segments = 3;
		const float segmentTime = duration / segments;
		var minus = Random.Range(0, 2) == 0;
		for (var i = 0; i < segments; i++)
		{
			minus = !minus;
			float t = 0;
			var rotationTarget = new Vector3(0, 0, (minus ? -rotPower : rotPower) / (i + 1));
			while (t < 1)
			{
				t += Time.deltaTime / (segmentTime / 2);
				Vector3 shakeRotStart = shakeRot;
				Vector3 shakeRotEnd = rotationTarget;

				shakeRot = Vector3.Lerp(shakeRotStart, shakeRotEnd, t);
				area.rotation = Quaternion.Euler(shakeRot);
				yield return null;
			}

			t = 0;
			while (t < 1)
			{
				t += Time.deltaTime / (segmentTime / 2);
				Vector3 shakeRotStart = rotationTarget;
				Vector3 shakeRotEnd = Vector3.zero;

				shakeRot = Vector3.Lerp(shakeRotStart, shakeRotEnd, t);
				area.rotation = Quaternion.Euler(shakeRot);
				yield return null;
			}
		}

		StopShake();
	}

	private void StopShake()
	{
		shakeRot = Vector3.zero;
		area.rotation = Quaternion.identity;
		if (shakeCoroutine != null)
		{
			StopCoroutine(shakeCoroutine);
			shakeCoroutine = null;
		}
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