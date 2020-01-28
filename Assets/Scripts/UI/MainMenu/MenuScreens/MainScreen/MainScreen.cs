using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainScreen : MenuScreen
{
	private bool secretActivated = false;
	public List<HeartButton> heartButtons;

	public override void Open(bool withAnimation = true)
	{
		foreach (var heartButton in heartButtons)
		{
			heartButton.ResetFill();
		}

		base.Open(withAnimation);
	}

	private void Update()
	{
		if (!secretActivated)
		{
			var secretCanBeActivated = heartButtons.All(heartButton => !(heartButton.FillAmount < 0.5f));
			if (secretCanBeActivated)
			{
				ActivateSecret();
			}
		}
	}

	private void ActivateSecret()
	{
		secretActivated = true;
		Debug.Log("Secret Activated!");
	}

	public void NewGameClicked()
	{
	}

	public void LoadGameClicked()
	{
	}

	public void ContinueClicked()
	{
	}

	public void QuitClicked()
	{
	}
}