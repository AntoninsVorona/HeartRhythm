using System.Collections.Generic;
using System.Text;
using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue Registrators/Inside Dialogue Registrator",
	fileName = "InsideDialogueRegistrator")]
public class InsideDialogueRegistrator : DialogueRegistrator
{
	public string GetItemsLeft()
	{
		if (SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.shirtGiven &&
		    SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.pantsGiven &&
		    SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.funkyGiven &&
		    SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.bananaGiven)
		{
			return "All done.";
		}

		var stringBuilder = new List<string>();
		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.shirtGiven)
		{
			stringBuilder.Add("Shirt");
		}

		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.pantsGiven)
		{
			stringBuilder.Add("Pants");
		}

		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.funkyGiven)
		{
			stringBuilder.Add("Toy");
		}

		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.bananaGiven)
		{
			stringBuilder.Add("Piece of Art");
		}

		return $"You still need to bring {string.Join(", ", stringBuilder)}.";
	}

	public override void RegisterDialogueFunctions()
	{
		Lua.RegisterFunction("GetItemsLeft", this,
			typeof(InsideDialogueRegistrator).GetMethod("GetItemsLeft"));
	}

	public override void UnregisterDialogueFunctions()
	{
		Lua.UnregisterFunction("GetItemsLeft");
	}
}