using PixelCrushers.DialogueSystem;

public class SubtitlePanelNoDeactivate : StandardUISubtitlePanel
{
	protected override void OnHidden()
	{
		panelState = PanelState.Closed;
	}
}