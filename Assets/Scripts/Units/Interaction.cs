using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
	[SerializeField]
	private List<MovementDirectionUtilities.MovementDirection> danceMovesSetToApply;

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
}