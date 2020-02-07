using PixelCrushers.DialogueSystem;
using UnityEngine;

public class ResponseMenuPanelNoDeactivate : StandardUIMenuPanel
{
	private CanvasGroup canvasGroup;

	private CanvasGroup CanvasGroup
	{
		get
		{
			if (!canvasGroup)
			{
				canvasGroup = buttonTemplateHolder.GetComponent<CanvasGroup>();
			}

			return canvasGroup;
		}
	}

	protected override void OnHidden()
	{
		panelState = PanelState.Closed;
	}

	public override void Open()
	{
		CanvasGroup.alpha = 0;
		CanvasGroup.interactable = false;
		CanvasGroup.blocksRaycasts = false;
		base.Open();
	}
}