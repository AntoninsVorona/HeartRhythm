using System.Collections.Generic;
using UnityEngine;

public class MobManager : UnitManager<Mob>
{
	public void MakeMobsActions()
	{
		allUnits.ForEach(m => m.MakeAction());
	}

	public void InitializeMob(Mob mob, Vector2Int location, Mob.MovementSettings movementSettings = null)
	{
		if (movementSettings != null)
		{
			mob.movementSettings = movementSettings;
		}

		InitializeUnit(mob, location);
	}

	public void ResumeAllMobs()
	{
		allUnits.ForEach(m => m.Initialize(m.CurrentPosition));
	}

	public override void AdditionalActionsToUnitsWhenStopping(Mob unit)
	{
		base.AdditionalActionsToUnitsWhenStopping(unit);
		unit.peaceModeMovementCoroutine = null;
		unit.StopTalk(true);
	}
}