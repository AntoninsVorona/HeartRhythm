using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class EqualizerController : MonoBehaviour
{
	private const int EQUALIZER_LINE_WIDTH = 50;
	private const int EQUALIZER_MAX_DOTS = 8;
	private const int MIDDLE_POINT = 25;
	private const int MAX_DELTA = 25;
	private const float FILL_MODIFIER = 1f / EQUALIZER_MAX_DOTS;

	[HideInNormalInspector]
	public bool active;

	[SerializeField]
	private Image equalizerCurrentHpLeftToRight;
	[SerializeField]
	private Image equalizerCurrentHpRightToLeft;

	[SerializeField]
	private RectTransform area;

	[Header("Equalizer Lines")]
	[SerializeField]
	private Color defaultColor;

	[SerializeField]
	private Color defaultToFadeOutColor;

	[SerializeField]
	private Color completeFadeColor;

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
	private float currentHealthPercentage;
	private float currentMaxValue;
	private int currentDelta;
	private AudioManager.MusicSettings previousMusicSettings;

	public void Initialize()
	{
		for (var i = 0; i <= MIDDLE_POINT + MAX_DELTA; i++)
		{
			var line = Instantiate(equalizerLinePrefab, lineHolder);
			equalizerLines.Add(i, line);
			line.rectTransform.anchoredPosition = GetLinePosition(i);
			line.gameObject.SetActive(false);
		}

		var middle = equalizerLines[MIDDLE_POINT];
		middle.gameObject.SetActive(true);
		middle.fillAmount = 0;

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
		currentDelta = 0;
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
		InitLinesBeyondDelta();
		var randomHpDelta = RandomHpDelta();
		var (color, random) = InitLineColor();
		var startFill = GetStartFill();
		var middle = equalizerLines[MIDDLE_POINT];
		middle.fillAmount = startFill;
		middle.color = color;
		if (randomHpDelta == 0)
		{
			CreateSideBumps(MIDDLE_POINT, MIDDLE_POINT - currentDelta, -1, startFill, color, random);
			CreateSideBumps(MIDDLE_POINT, MIDDLE_POINT + currentDelta, 1, startFill, color, random);
		}
		else
		{
			var lowerStartPoint = MIDDLE_POINT - randomHpDelta;
			var upperStartPoint = MIDDLE_POINT + randomHpDelta;
			var startingLineLower = equalizerLines[lowerStartPoint];
			var startingLineUpper = equalizerLines[upperStartPoint];
			startingLineLower.fillAmount = startFill;
			startingLineLower.color = color;
			startingLineUpper.fillAmount = startFill;
			startingLineUpper.color = color;
			CreateSideBumps(lowerStartPoint, MIDDLE_POINT - currentDelta, -1, startFill, color, random);
			CreateSideBumps(lowerStartPoint, MIDDLE_POINT - 1, 1, startFill, color, random);
			CreateSideBumps(upperStartPoint, MIDDLE_POINT + currentDelta, 1, startFill, color, random);
			CreateSideBumps(upperStartPoint, MIDDLE_POINT + 1, -1, startFill, color, random);
		}

		float timePassed = 0;
		while (true)
		{
			var slowDownRate = slowDownCurve.Evaluate(timePassed) * (float) AudioManager.Instance.beatDelay;
			yield return new WaitForSeconds(slowDownRate);
			timePassed += slowDownRate * 3;
			for (var i = MIDDLE_POINT - currentDelta; i <= MIDDLE_POINT + currentDelta; i++)
			{
				var line = equalizerLines[i];
				var fillAmount = GetFillAmount(line.fillAmount, 0);
				line.fillAmount = fillAmount;
			}
		}
	}

	private (Color color, bool random) InitLineColor()
	{
		var percentage = currentHealthPercentage * 100;
		const int fadeOutColorEdge = 41;
		const int defaultColorUntil = 60;
		const int rainbowStarting = 80;
		var random = false;
		Color color;
		if (percentage < fadeOutColorEdge)
		{
			var t = percentage / fadeOutColorEdge;
			if (t > 0.5f)
			{
				var colorT = Mathf.InverseLerp(0.5f, 1f, t);
				color = Color.Lerp(defaultToFadeOutColor, defaultColor, colorT);
			}
			else
			{
				var colorT = Mathf.InverseLerp(0, 0.5f, t);
				color = Color.Lerp(completeFadeColor, defaultToFadeOutColor, colorT);
			}
		}
		else if (percentage <= defaultColorUntil)
		{
			color = defaultColor;
		}
		else if (percentage < rainbowStarting)
		{
			color = RandomColor();
		}
		else
		{
			random = true;
			color = RandomColor();
		}

		return (color, random);

		Color RandomColor()
		{
			return Random.ColorHSV(0, 1, 0.6f, 1, 0.5f, 0.85f, 0.5f, 0.5f);
		}
	}

	private void CreateSideBumps(int startPoint, int endPoint, int modifier, float startFill, Color color, bool random)
	{
		Color.RGBToHSV(color, out var h, out var s, out var v);
		const float hueModifier = 0.025f;
		var currentHueModifier = hueModifier * modifier;
		var nextPoint = startPoint;
		var minFill = GetMinFill();
		var fillModifier = GetFillAmount(startFill, minFill);
		while (nextPoint != endPoint)
		{
			nextPoint += modifier;
			var line = equalizerLines[nextPoint];
			line.fillAmount = fillModifier;
			fillModifier = GetFillAmount(fillModifier, minFill);
			if (Mathf.Approximately(fillModifier, minFill))
			{
				fillModifier = GetMinFillAmount();
			}

			if (random)
			{
				h += currentHueModifier;
				if (h > 1)
				{
					h -= 1;
				}
				else if (h < 0)
				{
					h += 1;
				}

				color = Color.HSVToRGB(h, s, v);
				color.a = 0.5f;
			}

			line.color = color;
		}
	}

	private float GetStartFill()
	{
		if (currentHealthPercentage > 0.4f)
		{
			return 1;
		}

		if (currentHealthPercentage > 0.2f)
		{
			return FILL_MODIFIER * (EQUALIZER_MAX_DOTS - 1);
		}

		return FILL_MODIFIER * (EQUALIZER_MAX_DOTS - 2);
	}

	private float GetMinFill()
	{
		if (currentHealthPercentage > 0.6f)
		{
			return FILL_MODIFIER * 2;
		}

		if (currentHealthPercentage > 0.8f)
		{
			return FILL_MODIFIER * 3;
		}

		return 0;
	}

	private float GetFillAmount(float start, float minFill)
	{
		float modifier;
		switch (Random.Range(0, 5))
		{
			case 2:
			case 3:
				modifier = FILL_MODIFIER * 2;
				break;
			case 4:
				modifier = FILL_MODIFIER * 3;
				break;
			default:
				modifier = FILL_MODIFIER;
				break;
		}

		var fillAmount = start - modifier;

		if (fillAmount < minFill)
		{
			fillAmount = minFill;
		}

		return fillAmount;
	}

	private float GetMinFillAmount()
	{
		int random;
		if (currentHealthPercentage > 0.8f)
		{
			random = Random.Range(4, 8);
		}
		else if (currentHealthPercentage > 0.6f)
		{
			random = Random.Range(3, 6);
		}
		else
		{
			random = Random.Range(0, 3);
		}

		switch (random)
		{
			case 1:
				return FILL_MODIFIER * 3;
			case 2:
			case 3:
				return FILL_MODIFIER * 4;
			case 4:
				return FILL_MODIFIER * 5;
			case 5:
			case 6:
				return FILL_MODIFIER * 6;
			case 7:
				return FILL_MODIFIER * 7;
			default:
				return FILL_MODIFIER * 2;
		}
	}

	private int RandomHpDelta()
	{
		return Random.Range(0, currentDelta + 1);
	}

	public void UpdateCurrentHp(int currentHp, int maxHp)
	{
		currentHealthPercentage = (float) currentHp / maxHp;
		equalizerCurrentHpLeftToRight.fillAmount = currentHealthPercentage;
		equalizerCurrentHpRightToLeft.fillAmount = currentHealthPercentage;
		currentMaxValue = (float) currentHp / maxHp;
		var prevDelta = currentDelta;
		currentDelta = Mathf.RoundToInt(currentMaxValue * (MAX_DELTA + 1)) - 1;
		if (currentDelta < 0)
		{
			currentDelta = 0;
		}

		if (prevDelta > currentDelta)
		{
			for (var i = MIDDLE_POINT - prevDelta; i < MIDDLE_POINT - currentDelta; i++)
			{
				equalizerLines[i].gameObject.SetActive(false);
			}

			for (var i = MIDDLE_POINT + prevDelta; i > MIDDLE_POINT + currentDelta; i--)
			{
				equalizerLines[i].gameObject.SetActive(false);
			}
		}
		else
		{
			for (var i = MIDDLE_POINT - currentDelta; i < MIDDLE_POINT - prevDelta; i++)
			{
				var line = equalizerLines[i];
				line.gameObject.SetActive(true);
				line.fillAmount = 0;
			}

			for (var i = MIDDLE_POINT + currentDelta; i > MIDDLE_POINT + prevDelta; i--)
			{
				var line = equalizerLines[i];
				line.gameObject.SetActive(true);
				line.fillAmount = 0;
			}
		}

		ApplyHealthEffects(Mathf.RoundToInt(currentHealthPercentage * 100));
	}

	private void ApplyHealthEffects(int percentage)
	{
		const int basicModeMin = 41;
		var corruptionLevel = 0;
		AudioManager.MusicSettings musicSettings;
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

	private void InitLinesBeyondDelta()
	{
		for (var i = 0; i < MIDDLE_POINT - currentDelta; i++)
		{
			equalizerLines[i].gameObject.SetActive(false);
		}

		for (var i = MIDDLE_POINT + MAX_DELTA; i > MIDDLE_POINT + currentDelta; i--)
		{
			equalizerLines[i].gameObject.SetActive(false);
		}
	}
}