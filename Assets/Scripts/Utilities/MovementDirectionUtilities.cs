using System;
using UnityEngine;

public static class MovementDirectionUtilities
{
	public enum MovementDirection
	{
		None = 0,
		Left = 1,
		Right = 2,
		Up = 3,
		Down = 4
	}

	public static MovementDirection DirectionFromInput(int horizontal, int vertical)
	{
		if (horizontal != 0)
		{
			return horizontal == -1 ? MovementDirection.Left : MovementDirection.Right;
		}

		if (vertical != 0)
		{
			return vertical == -1 ? MovementDirection.Down : MovementDirection.Up;
		}

		return MovementDirection.None;
	}

	public static MovementDirection DirectionFromDifference(Vector2Int currentPosition, Vector2Int target)
	{
		var diff = target - currentPosition;
		if (Mathf.Abs(diff.x) > 1 || Mathf.Abs(diff.y) > 1 || diff.x != 0 && diff.y != 0)
		{
			return MovementDirection.None;
		}

		return DirectionFromInput(diff.x, diff.y);
	}

	public static Vector2Int VectorFromDirection(MovementDirection movementDirection)
	{
		switch (movementDirection)
		{
			case MovementDirection.None:
				return Vector2Int.zero;
			case MovementDirection.Left:
				return Vector2Int.left;
			case MovementDirection.Right:
				return Vector2Int.right;
			case MovementDirection.Up:
				return Vector2Int.up;
			case MovementDirection.Down:
				return Vector2Int.down;
			default:
				throw new ArgumentOutOfRangeException(nameof(movementDirection), movementDirection, null);
		}
	}
}