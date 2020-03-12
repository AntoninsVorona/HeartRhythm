using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class MainScreen : MenuScreen
{
	[SerializeField]
	protected HeartButton loadButton;

	public override void Open(bool withAnimation = true)
	{
		if (!SaveSystem.HasAnySaves())
		{
			DisableLoadingButtons();
		}
		else
		{
			EnableLoadingButtons();
		}

		base.Open(withAnimation);
	}

	protected virtual void EnableLoadingButtons()
	{
		loadButton.gameObject.SetActive(true);
	}

	protected virtual void DisableLoadingButtons()
	{
		loadButton.gameObject.SetActive(false);
	}

	public void LoadGameClicked()
	{
		AbstractMainMenu.Instance.OpenScreen<LoadGameScreen>();
	}

	public void OptionsClicked()
	{
		AbstractMainMenu.Instance.OpenScreen<Options>();
	}

	public abstract void QuitClicked();
}