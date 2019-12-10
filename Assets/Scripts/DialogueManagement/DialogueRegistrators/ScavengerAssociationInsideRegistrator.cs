using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue Registrators/Scavenger Association Inside Registrator ", fileName = "ScavengerAssociationInsideRegistrator")]
public class ScavengerAssociationInsideRegistrator : DialogueRegistrator
{
	public bool CanPickUp(string itemName, double amount)
	{
		return Player.Instance.CanPickUp(ItemManager.Instance.GetItemByName(itemName), (int) amount).canPickUpAll;
	}

	public override void RegisterDialogueFunctions()
	{
		Lua.RegisterFunction("CanPickUp", this, typeof(ScavengerAssociationInsideRegistrator).GetMethod("CanPickUp"));
	}

	public override void UnregisterDialogueFunctions()
	{
		Lua.UnregisterFunction("CanPickUp");
	}
}