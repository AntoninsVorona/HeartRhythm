using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : ScriptableObject
{
	public enum LockedType
	{
		CanGuess = 0,
		OnlyWhenKnown = 1
	}

	[Serializable]
	public class LockedState
	{
		public bool locked = false;

		[DrawIf("locked", true)]
		public LockedType lockedType = LockedType.CanGuess;
	}

	[Serializable]
	public class DanceMoveXVisibility
	{
		public LockedState lockedState;
		public MovementDirectionUtilities.MovementDirection danceMove;
	}

	[HideInNormalInspector]
	public Unit owner;

	public string interactionName;
	public bool visibleOnUI = true;
	public List<DanceMoveXVisibility> danceMovesSetToApply;

	public void Initialize(Unit owner)
	{
		this.owner = owner;
	}

	public bool DanceMoveCanBeApplied(List<MovementDirectionUtilities.MovementDirection> danceMoveSet)
	{
		if (danceMoveSet.Count != danceMovesSetToApply.Count)
		{
			return false;
		}

		for (var i = 0; i < danceMoveSet.Count; i++)
		{
			var danceMoveValue = danceMoveSet[i];
			var danceMoveCheck = danceMovesSetToApply[i];
			if (danceMoveValue != danceMoveCheck.danceMove)
			{
				return false;
			}

			if (danceMoveCheck.lockedState.locked &&
			    danceMoveCheck.lockedState.lockedType == LockedType.OnlyWhenKnown)
			{
				return false;
			}
		}

		Debug.Log(interactionName);
		return true;
	}

	public abstract bool ApplyInteraction();
}