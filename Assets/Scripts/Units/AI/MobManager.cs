using System.Collections.Generic;
using UnityEngine;

public class MobManager : UnitManager<Mob>
{
	private const string PATH = "Mobs/";

	public void MakeMobsActions()
	{
		var units = new List<Mob>(allUnits);
		units.ForEach(m => m.MakeAction());
	}

	public Mob SpawnMob(string mobName, Vector2Int location, Mob.MovementSettings movementSettings = null)
	{
		var mob = Resources.Load<Mob>($"{PATH}{mobName}");
		return SpawnMob(mob, location, movementSettings);
	}

	public Mob SpawnMob(Mob mob, Vector2Int location, Mob.MovementSettings movementSettings = null)
	{
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

	public void RemoveAllUnits()
	{
		var units = new List<Mob>(allUnits);
		foreach (var unit in units)
		{
			unit.Die();
		}
	}
}