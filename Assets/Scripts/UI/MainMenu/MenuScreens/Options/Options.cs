using System;
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
		InitFullScreenMode();
		InitResolution();
		InitializeSettingsAsPerSave();
	}

	private void InitializeSettingsAsPerSave()
	{
		SetCurrentResolution();
		SetCurrentFullScreenMode();
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
	}

	private void InitResolution()
	{
		screenResolutions = Screen.resolutions;
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
			if (resolution.height == currentResolution.height && resolution.width == currentResolution.width &&
			    resolution.refreshRate == currentResolution.refreshRate)
			{
				return i;
			}
		}

		throw new ArgumentException("No such resolution");
	}

	public void SaveClicked()
	{
		if (somethingChanged)
		{
			SaveSystem.SetSettings(screenResolutions[screenResolutionDropdown.value],
				(FullScreenMode) fullScreenModeDropdown.value);
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