using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue Registrators/Scavenger Field Dialogue Registrator",
	fileName = "ScavengerFieldDialogueRegistrator")]
public class ScavengerFieldDialogueRegistrator : DialogueRegistrator
{
	public void ChangeBumClothes()
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.bumIsNaked = true;
		NeedYourClothesBum.Instance.ClothesChanged();
		Player.Instance.PickUpItem(ItemManager.Instance.GetItemByName("NeedYourClothesBumShirt"));
		Player.Instance.PickUpItem(ItemManager.Instance.GetItemByName("NeedYourClothesBumPants"));
	}

	public override void RegisterDialogueFunctions()
	{
		Lua.RegisterFunction("ChangeBumClothes", this,
			typeof(ScavengerFieldDialogueRegistrator).GetMethod("ChangeBumClothes"));
	}

	public override void UnregisterDialogueFunctions()
	{
		Lua.UnregisterFunction("ChangeBumClothes");
	}
}