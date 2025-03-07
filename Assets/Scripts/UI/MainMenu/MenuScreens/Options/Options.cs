﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options : MenuScreen
{
	[SerializeField]
	private TMP_Dropdown screenResolutionDropdown;

	[SerializeField]
	private TMP_Dropdown fullScreenModeDropdown;

	[SerializeField]
	private TMP_Dropdown targetFrameRateDropdown;

	[SerializeField]
	private TextMeshProUGUI version;

	private static readonly List<int> FRAME_RATES = new List<int>
	{
		30,
		60,
		90,
		120,
		144,
		-1
	};

	private Resolution[] screenResolutions;
	private bool initialized;
	private bool somethingChanged;

	public override void Open(bool withAnimation = true)
	{
		somethingChanged = false;
		if (!initialized)
		{
			Initialize();
		}
		else
		{
			InitializeSettingsAsPerSave();
		}

		base.Open(withAnimation);
	}

	private void Initialize()
	{
		initialized = true;
		version.text = $"Version: {SaveSystem.SAVE_VERSION}";
		InitFullScreenMode();
		InitFrameRate();
		InitResolution();
		InitializeSettingsAsPerSave();
	}

	private void InitializeSettingsAsPerSave()
	{
		SetCurrentFullScreenMode();
		SetCurrentFrameRate();
		SetCurrentResolution();
	}

	private void InitFrameRate()
	{
		targetFrameRateDropdown.ClearOptions();
		var options = FRAME_RATES.Select(frameRate =>
				new TMP_Dropdown.OptionData(
					$"Frame Rate Cap: {(frameRate == -1 ? "Unlimited" : frameRate.ToString())}"))
			.ToList();
		targetFrameRateDropdown.AddOptions(options);
	}

	private void SetCurrentFrameRate()
	{
		targetFrameRateDropdown.SetValueWithoutNotify(GetCurrentFrameRate());
	}

	private int GetCurrentFrameRate()
	{
		var currentFrameRate = Application.targetFrameRate;
		for (var i = 0; i < FRAME_RATES.Count; i++)
		{
			var frameRate = FRAME_RATES[i];
			if (currentFrameRate == frameRate)
			{
				return i;
			}
		}

		Debug.LogError($"No such Frame Rate: {currentFrameRate}");
		Application.targetFrameRate = 60;
		return 1;
	}

	private void InitFullScreenMode()
	{
		fullScreenModeDropdown.ClearOptions();
		fullScreenModeDropdown.AddOptions(new List<TMP_Dropdown.OptionData>
		{
			new TMP_Dropdown.OptionData("Exclusive FullScreen"),
			new TMP_Dropdown.OptionData("FullScreen Windowed"),
			new TMP_Dropdown.OptionData("Maximized Window"),
			new TMP_Dropdown.OptionData("Windowed")
		});
	}

	private void SetCurrentFullScreenMode()
	{
		fullScreenModeDropdown.SetValueWithoutNotify((int) Screen.fullScreenMode);
		InitResolution();
		SetCurrentResolution();
	}

	private void InitResolution()
	{
		screenResolutions = Screen.resolutions.Where(resolution => resolution.refreshRate == 60).Reverse().ToArray();
		screenResolutionDropdown.ClearOptions();
		var options = screenResolutions.Select(resolution => new TMP_Dropdown.OptionData(resolution.ToString()))
			.ToList();
		screenResolutionDropdown.AddOptions(options);
	}

	private void SetCurrentResolution()
	{
		screenResolutionDropdown.SetValueWithoutNotify(GetCurrentResolution());
	}

	private int GetCurrentResolution()
	{
		var currentResolution = Screen.currentResolution;
		for (var i = 0; i < screenResolutions.Length; i++)
		{
			var resolution = screenResolutions[i];
			if (resolution.height == currentResolution.height && resolution.width == currentResolution.width)
			{
				return i;
			}
		}

		Debug.LogError($"No such resolution: {currentResolution}");
		var screenResolution = screenResolutions[0];
		Screen.SetResolution(screenResolution.width, screenResolution.height, Screen.fullScreenMode,
			FRAME_RATES[GetCurrentFrameRate()]);
		somethingChanged = true;
		return 0;
	}

	public void SaveClicked()
	{
		if (somethingChanged)
		{
			SaveSystem.SetSettings(screenResolutions[screenResolutionDropdown.value],
				(FullScreenMode) fullScreenModeDropdown.value, FRAME_RATES[targetFrameRateDropdown.value]);
		}

		BackToMainScreen();
	}

	public void OnValueChanged()
	{
		somethingChanged = true;
	}

	public void BackToMainScreen()
	{
		AbstractMainMenu.Instance.OpenScreen<MainScreen>();
	}

	public override void ApplyCancel()
	{
		BackToMainScreen();
	}
}