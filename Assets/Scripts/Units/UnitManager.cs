﻿using System;
using System.Collections.Generic;
using System.Linq;
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
	
	public List<Unit.UnitData> GetDataOfUnits()
	{
		return GetDataOfUnits<Unit>();
	}
	
	public List<Unit.UnitData> GetDataOfUnits<U>() where U : Unit
	{
		return allUnits.OfType<U>().Select(i => i.GetUnitData()).Where(d => !string.IsNullOrEmpty(d.identifierName)).ToList();
	}
	
	public void StopAllActions()
	{
		StopAllCoroutines();
		var units = new List<T>(allUnits);
		foreach (var unit in units)
		{
			if (!unit)
			{
				allUnits.Remove(unit);
				continue;
			}

			AdditionalActionsToUnitsWhenStopping(unit);
		}
	}

	public virtual void AdditionalActionsToUnitsWhenStopping(T unit)
	{
		unit.RemoveGameStateObserver();
		unit.StopShake();
		unit.StopTint();
	} 

	public void ApplyActionOnUnits(Action<T> action)
	{
		allUnits.ForEach(action);
	}
}