using PixelCrushers.DialogueSystem;

public class SubtitlePanelNoDeactivate : StandardUISubtitlePanel
{
	protected override void OnHidden()
	{
		if (panelState != PanelState.Uninitialized)
		{
			panelState = PanelState.Closed;
		}
	}
}