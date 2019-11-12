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

    public static Vector3Int VectorFromDirection(MovementDirection movementDirection)
    {
        switch (movementDirection)
        {
            case MovementDirection.None:
                return Vector3Int.zero;
            case MovementDirection.Left:
                return Vector3Int.left;
            case MovementDirection.Right:
                return Vector3Int.right;
            case MovementDirection.Up:
                return Vector3Int.up;
            case MovementDirection.Down:
                return Vector3Int.down;
            default:
                throw new ArgumentOutOfRangeException(nameof(movementDirection), movementDirection, null);
        }
    }
}