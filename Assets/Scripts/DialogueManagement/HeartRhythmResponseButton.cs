using PixelCrushers.DialogueSystem;
using UnityEngine;

public class HeartRhythmResponseButton : StandardUIResponseButton
{
	public DialogueFillingButton fillingButton;
	
	public override void Awake()
	{
	}

	public override void Start()
	{
		fillingButton.SetOnClick(OnClick);
	}
}