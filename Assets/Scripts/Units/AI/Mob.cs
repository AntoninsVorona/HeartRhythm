﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Mob : Unit
{
	public enum TypeOfMovement
	{
		None = 0,
		Constant = 1,
		Random = 2,
		DefinedArea = 3,
		FollowPlayer = 4,
		SpecificArea = 5
	}

	[Serializable]
	public class SpecificArea
	{
		public List<Vector2Int> movement;
		public bool circle;
		private bool goBackward;

		public MovementDirectionUtilities.MovementDirection GetNextMovementDirection(Vector2Int currentPosition)
		{
			var nextMoveLocation = GetNextMoveLocation(currentPosition);
			return MovementDirectionUtilities.DirectionFromDifference(currentPosition, nextMoveLocation);
		}

		private Vector2Int GetNextMoveLocation(Vector2Int currentPosition)
		{
			var indexOf = movement.IndexOf(currentPosition);
			if (indexOf == -1)
			{
				return currentPosition;
			}

			if (circle)
			{
				if (indexOf == movement.Count - 1)
				{
					return movement[0];
				}

				return movement[indexOf + 1];
			}

			if (goBackward)
			{
				if (indexOf == 0)
				{
					goBackward = false;
					return movement[1];
				}

				return movement[indexOf - 1];
			}

			if (indexOf == movement.Count - 1)
			{
				goBackward = true;
				return movement[movement.Count - 2];
			}

			return movement[indexOf + 1];
		}
	}

	[Serializable]
	public class DefinedArea
	{
		public List<Vector2Int> locations;

		public MovementDirectionUtilities.MovementDirection GetMovementDirectionWithinArea(Vector2Int currentPosition)
		{
			var suitableDirection = FindSuitableDirections(currentPosition);
			return suitableDirection.Count > 0
				? suitableDirection.OrderBy(s => Guid.NewGuid()).First()
				: MovementDirectionUtilities.MovementDirection.None;
		}

		private List<MovementDirectionUtilities.MovementDirection> FindSuitableDirections(
			Vector2Int position)
		{
			var movementDirections = EnumUtilities.GetValues<MovementDirectionUtilities.MovementDirection>();
			movementDirections.Remove(MovementDirectionUtilities.MovementDirection.None);
			var suitableDirections = new List<MovementDirectionUtilities.MovementDirection>();
			foreach (var diff in locations.Select(l => l - position))
			{
				foreach (var movementDirection in from movementDirection in movementDirections
					where !suitableDirections.Contains(movementDirection)
					let possible = CheckLocation(diff, movementDirection)
					where possible
					select movementDirection)
				{
					suitableDirections.Add(movementDirection);
					if (suitableDirections.Count == 4)
					{
						return suitableDirections;
					}
				}
			}

			return suitableDirections;

			bool CheckLocation(Vector2Int difference, MovementDirectionUtilities.MovementDirection movementDirection)
			{
				return difference == MovementDirectionUtilities.VectorFromDirection(movementDirection);
			}
		}
	}

	[Serializable]
	public class MovementSettings
	{
		public TypeOfMovement typeOfMovement;

		[DrawIf("typeOfMovement", TypeOfMovement.Constant)]
		public MovementDirectionUtilities.MovementDirection movementDirection;

		[DrawIf("typeOfMovement", TypeOfMovement.DefinedArea)]
		public DefinedArea definedArea;

		[DrawIf("typeOfMovement", TypeOfMovement.SpecificArea)]
		public SpecificArea specificArea;

		[Header("Peace Mode")]
		public bool moveDuringPeaceMode = true;

		[DrawIf("moveDuringPeaceMode", true)]
		public float peaceModeMovementDelay = 1;
	}

	public MovementSettings movementSettings;
	private float lastMovementDuringPeaceMode;
	private readonly Pathfinder pathfinder = new Pathfinder();

	[HideInInspector]
	public Coroutine peaceModeMovementCoroutine;

	protected override void Start()
	{
		base.Start();
		if (initializeSelf)
		{
			GameSessionManager.Instance.currentSceneObjects.currentMobManager.InitializeMob(this, spawnPoint);
		}
	}

	public override void Initialize(Vector2Int location)
	{
		base.Initialize(location);
		if (GameSessionManager.Instance.CurrentGameState == GameSessionManager.GameState.Peace
		    && movementSettings.moveDuringPeaceMode
		    && peaceModeMovementCoroutine == null)
		{
			peaceModeMovementCoroutine = CoroutineStarter().StartCoroutine(PeaceModeMovement());
		}
	}

	public virtual void MakeAction()
	{
		switch (movementSettings.typeOfMovement)
		{
			case TypeOfMovement.None:
				break;
			case TypeOfMovement.Constant:
				Move(movementSettings.movementDirection);
				break;
			case TypeOfMovement.Random:
				AssignRandomDirection();
				Move(movementSettings.movementDirection);
				break;
			case TypeOfMovement.DefinedArea:
				movementSettings.movementDirection =
					movementSettings.definedArea.GetMovementDirectionWithinArea(currentPosition);
				Move(movementSettings.movementDirection);
				break;
			case TypeOfMovement.FollowPlayer:
				var (canMove, nextStep) = pathfinder.FindPath(Player.Instance.CurrentPosition, currentPosition);
				if (canMove)
				{
					movementSettings.movementDirection =
						MovementDirectionUtilities.DirectionFromDifference(currentPosition, nextStep);
					Move(movementSettings.movementDirection);
				}
				else
				{
					(canMove, nextStep) = pathfinder.FindPath(Player.Instance.CurrentPosition, currentPosition, true);
					if (canMove &&
					    GameSessionManager.Instance.currentSceneObjects.currentWorld.CanWalk(nextStep).Item1 ==
					    GameTile.CantMoveReason.None)
					{
						movementSettings.movementDirection =
							MovementDirectionUtilities.DirectionFromDifference(currentPosition, nextStep);
						Move(movementSettings.movementDirection);
					}
				}

				break;
			case TypeOfMovement.SpecificArea:
				movementSettings.movementDirection =
					movementSettings.specificArea.GetNextMovementDirection(CurrentPosition);
				Move(movementSettings.movementDirection);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void AssignRandomDirection()
	{
		movementSettings.movementDirection =
			EnumUtilities.RandomEnumValue<MovementDirectionUtilities.MovementDirection>();
	}

	public override void Die()
	{
		UnoccupyTile();
		GameSessionManager.Instance.currentSceneObjects.currentMobManager.RemoveUnit(this);
		if (peaceModeMovementCoroutine != null)
		{
			CoroutineStarter().StopCoroutine(peaceModeMovementCoroutine);
			peaceModeMovementCoroutine = null;
		}

		if (animator)
		{
			animator.SetTrigger(AnimatorUtilities.DIE_TRIGGER);
		}
		else
		{
			Deactivate();
		}
	}

	protected override void InteractWithObject(Unit unit)
	{
	}

	protected override void PeaceState()
	{
		base.PeaceState();
		if (movementSettings.moveDuringPeaceMode && peaceModeMovementCoroutine == null)
		{
			peaceModeMovementCoroutine = CoroutineStarter().StartCoroutine(PeaceModeMovement());
		}
	}

	protected override void FightState()
	{
		base.FightState();
		if (peaceModeMovementCoroutine != null)
		{
			CoroutineStarter().StopCoroutine(peaceModeMovementCoroutine);
			peaceModeMovementCoroutine = null;
		}
	}

	private IEnumerator PeaceModeMovement()
	{
		while (true)
		{
			yield return new WaitForSeconds(movementSettings.peaceModeMovementDelay);
			if (GameSessionManager.Instance.playState == GameSessionManager.PlayState.Basic)
			{
				MakeAction();
			}
		}
	}

	public override void Talk(string text = null)
	{
		if (drawingBoard)
		{
			drawingBoard.Talk(CoroutineStarter(), string.IsNullOrEmpty(text) ? talkUI.GetRandomText() : text);
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		var size = new Vector3(1, 1, 0.2f);
		if (movementSettings.typeOfMovement == TypeOfMovement.DefinedArea)
		{
			Gizmos.color = Color.magenta;
			foreach (var location in movementSettings.definedArea.locations)
			{
				Gizmos.DrawCube(CubeLocation(location), size);
			}
		}
		else if (movementSettings.typeOfMovement == TypeOfMovement.SpecificArea)
		{
			Gizmos.color = Color.cyan;
			foreach (var location in movementSettings.specificArea.movement)
			{
				Gizmos.DrawCube(CubeLocation(location), size);
			}
		}

		Vector3 CubeLocation(Vector2Int point)
		{
			return (Vector3Int) point + new Vector3(0.5f, 0.5f, 0);
		}
	}
}