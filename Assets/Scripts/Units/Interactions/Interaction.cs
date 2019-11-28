using System;
using System.Collections.Generic;
using System.Linq;
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

	[Serializable]
	public class Visibility
	{
		public bool visibleOnUI = true;

		[DrawIf("visibleOnUI", false)]
		public bool canBeUsedIfInvisible = true;
	}

	[HideInNormalInspector]
	public Unit owner;

	[SerializeField]
	private string interactionDescription;

	public Sprite interactionSymbol;
	public Visibility visibility;
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

		if (!visibility.visibleOnUI && !visibility.canBeUsedIfInvisible)
		{
			return false;
		}

		if (danceMovesSetToApply.Any(d => d.lockedState.locked &&
		                                  d.lockedState.lockedType == LockedType.OnlyWhenKnown))
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
		}

		Debug.Log(interactionDescription);
		return true;
	}

	public string GetDescription()
	{
		return string.Format(interactionDescription, GetDescriptionParams());
	}

	protected abstract object[] GetDescriptionParams();
	public abstract bool ApplyInteraction();
}