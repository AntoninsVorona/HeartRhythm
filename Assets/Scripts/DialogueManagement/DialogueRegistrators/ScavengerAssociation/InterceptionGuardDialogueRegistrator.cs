using PixelCrushers.DialogueSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogues/Dialogue Registrators/Interception Guard Dialogue Registrator",
	fileName = "InterceptionGuardDialogueRegistrator")]
public class InterceptionGuardDialogueRegistrator : DialogueRegistrator
{
	public Vector2Int handlePosition; 
	
	public void MoveCameraToPlayer()
	{
		GameCamera.Instance.ChangeTargetPosition(Player.Instance.transform.position);
	}

	public void MoveCameraToHandle()
	{
		GameCamera.Instance.ChangeTargetPosition((Vector3Int) handlePosition + new Vector3(0.5f, 0.5f));
	}
	
	public void SpawnDude()
	{
		((InterceptionGuardBattleRules) BattleRules.Instance).StartSpawning();
	}

	public void TurnAroundDamagingGuard()
	{
		((InterceptionGuardBattleRules) BattleRules.Instance).TurnAroundGuard();
	}

	public override void RegisterDialogueFunctions()
	{
		Lua.RegisterFunction("MoveCameraToPlayer", this,
			typeof(InterceptionGuardDialogueRegistrator).GetMethod("MoveCameraToPlayer"));
		Lua.RegisterFunction("MoveCameraToHandle", this,
			typeof(InterceptionGuardDialogueRegistrator).GetMethod("MoveCameraToHandle"));
		Lua.RegisterFunction("SpawnDude", this,
			typeof(InterceptionGuardDialogueRegistrator).GetMethod("SpawnDude"));
		Lua.RegisterFunction("TurnAroundDamagingGuard", this,
			typeof(InterceptionGuardDialogueRegistrator).GetMethod("TurnAroundDamagingGuard"));
	}

	public override void UnregisterDialogueFunctions()
	{
		Lua.UnregisterFunction("MoveCameraToPlayer");
		Lua.UnregisterFunction("MoveCameraToHandle");
		Lua.UnregisterFunction("SpawnDude");
		Lua.UnregisterFunction("TurnAroundDamagingGuard");
	}
}