using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    private readonly List<Mob> allMobs = new List<Mob>();

    public void MakeMobsActions()
    {
        allMobs.ForEach(m => m.MakeAction());
    }

    public void InitializeMob(Mob mob, Vector2Int location, Mob.MovementSettings movementSettings = null)
    {
        if (movementSettings != null)
        {
            mob.movementSettings = movementSettings;
        }

        mob.Initialize(location);
        allMobs.Add(mob);
    }

    public void ResumeAllMobs()
    {
        allMobs.ForEach(m => m.Initialize(m.CurrentPosition));
    }

    public void StopAllActionsBeforeLoading()
    {
        StopAllCoroutines();
        var mobs = new List<Mob>(allMobs);
        foreach (var mob in mobs)
        {
            if (!mob)
            {
                allMobs.Remove(mob);
                continue;
            }

            mob.peaceModeMovementCoroutine = null;
        }
    }

    public void RemoveMob(Mob mob)
    {
        allMobs.Remove(mob);
    }
}