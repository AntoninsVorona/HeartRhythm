using System.Collections.Generic;
using UnityEngine;

public class MobManager : UnitManager<Mob>
{
	private const string PATH = "Mobs/"; 
	
	public void MakeMobsActions()
	{
		allUnits.ForEach(m => m.MakeAction());
	}

	public Mob SpawnMob(string mobName, Vector2Int location, Mob.MovementSettings movementSettings = null)
	{
		var mob = Resources.Load<Mob>($"{PATH}{mobName}");
		var mobInstance = Instantiate(mob, transform);
		mobInstance.initializeSelf = false;
		InitializeMob(mobInstance, location, movementSettings);
		return mobInstance;
	}

	public void InitializeMob(Mob mob, Vector2Int location, Mob.MovementSettings movementSettings = null)
	{
		if (movementSettings != null)
		{
			mob.movementSettings = movementSettings;
		}

		InitializeUnit(mob, location);
	}

	public override void AdditionalActionsToUnitsWhenStopping(Mob unit)
	{
		base.AdditionalActionsToUnitsWhenStopping(unit);
		unit.peaceModeMovementCoroutine = null;
		unit.StopTalk(true);
	}
}