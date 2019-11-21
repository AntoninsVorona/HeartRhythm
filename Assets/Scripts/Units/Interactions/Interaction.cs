using System.Collections.Generic;
using UnityEngine;

public abstract class Interaction : ScriptableObject
{
	[HideInNormalInspector]
	public Unit owner;
	
	[SerializeField]
	private List<MovementDirectionUtilities.MovementDirection> danceMovesSetToApply;

	public void Initialize(Unit owner)
	{
		this.owner = owner;
	}
	
	public bool DanceMoveEquals(List<MovementDirectionUtilities.MovementDirection> danceMoveSet)
	{
		for (var i = 0; i < danceMoveSet.Count; i++)
		{
			var danceMove1 = danceMoveSet[i];
			var danceMove2 = danceMovesSetToApply[i];
			if (danceMove1 != danceMove2)
			{
				return false;
			}
		}

		return true;
	}

	public abstract bool ApplyInteraction();
}