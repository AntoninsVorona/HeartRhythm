using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue Registrators/Cool Dude Dialogue Registrator",
	fileName = "CoolDudeDialogueRegistrator")]
public class CoolDudeDialogueRegistrator : DialogueRegistrator
{
	public LevelData levelDataToLoad;
	
	public void CoolDudeSpotted()
	{
		GameSessionManager.Instance.LoadLevel(levelDataToLoad, 2);
	}

	public override void RegisterDialogueFunctions()
	{
		Lua.RegisterFunction("CoolDudeSpotted", this,
			typeof(CoolDudeDialogueRegistrator).GetMethod("CoolDudeSpotted"));
	}

	public override void UnregisterDialogueFunctions()
	{
		Lua.UnregisterFunction("CoolDudeSpotted");
	}
}