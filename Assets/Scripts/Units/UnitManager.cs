using System.Collections.Generic;
using UnityEngine;

public class UnitManager<T> : MonoBehaviour where T : Unit 
{
	protected readonly List<T> allUnits = new List<T>();
	
	public void InitializeUnit(T unit, Vector2Int spawnPoint)
	{
		unit.Initialize(spawnPoint);
		allUnits.Add(unit);
	}

	public void RemoveUnit(T unit)
	{
		allUnits.Remove(unit);
	}
}