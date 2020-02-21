using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue Registrators/Scavenger Association Inside Registrator ", fileName = "ScavengerAssociationInsideRegistrator")]
public class ScavengerAssociationInsideRegistrator : DialogueRegistrator
{
	public BattleArea duelGuard;
	
	public bool CanPickUp(string itemName, double amount)
	{
		return Player.Instance.CanPickUp(ItemManager.Instance.GetItemByName(itemName), (int) amount).canPickUpAll;
	}

	public void StartDuel()
	{
		GameSessionManager.Instance.LoadLevel(duelGuard);
	}

	public override void RegisterDialogueFunctions()
	{
		Lua.RegisterFunction("CanPickUp", this, typeof(ScavengerAssociationInsideRegistrator).GetMethod("CanPickUp"));
		Lua.RegisterFunction("StartDuel", this, typeof(ScavengerAssociationInsideRegistrator).GetMethod("StartDuel"));
	}

	public override void UnregisterDialogueFunctions()
	{
		Lua.UnregisterFunction("CanPickUp");
		Lua.UnregisterFunction("StartDuel");
	}
}