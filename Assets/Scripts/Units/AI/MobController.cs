using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    private readonly List<Mob> allMobs = new List<Mob>();

    private void Awake()
    {
        Instance = this;
    }

    public void MakeMobsActions()
    {
        allMobs.ForEach(m => m.MakeAction());
    }

    public void InitializeMob(Mob mob, Vector3Int location, Mob.MovementSettings movementSettings = null)
    {
        if (movementSettings != null)
        {
            mob.movementSettings = movementSettings;
        }

        mob.Initialize(location);
        allMobs.Add(mob);
    }

    public void RemoveMob(Mob mob)
    {
        allMobs.Remove(mob);
    }

    public static MobController Instance { get; private set; }
}