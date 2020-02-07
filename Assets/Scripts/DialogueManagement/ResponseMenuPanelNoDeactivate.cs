using PixelCrushers.DialogueSystem;
using UnityEngine;

public class ResponseMenuPanelNoDeactivate : StandardUIMenuPanel
{
	protected override void OnHidden()
	{
		panelState = PanelState.Closed;
	}
}