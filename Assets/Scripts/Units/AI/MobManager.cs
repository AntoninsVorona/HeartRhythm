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

    public void StopAllActionsBeforeLoading()
    {
        StopAllCoroutines();
        var mobs = new List<Mob>(allUnits);
        foreach (var mob in mobs)
        {
            if (!mob)
            {
                allUnits.Remove(mob);
                continue;
            }

            mob.peaceModeMovementCoroutine = null;
            mob.StopTalk(true);
        }
    }
}