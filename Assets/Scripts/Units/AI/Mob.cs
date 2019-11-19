using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mob : Unit
{
	public enum TypeOfMovement
	{
		None = 0,
		Constant = 1,
		Random = 2,
		DefinedArea = 3
	}

	[Serializable]
	public class DefinedArea
	{
		public List<Vector2Int> locations;

		public MovementDirectionUtilities.MovementDirection GetMovementDirectionWithinArea(Vector2Int currentPosition)
		{
			var suitableDirection = FindSuitableDirections(currentPosition);
			foreach (var suitableLocation in suitableDirection)
			{
				Debug.Log(suitableLocation);
			}

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

		[Header("Peace Mode")]
		public bool moveDuringPeaceMode = true;

		[DrawIf("moveDuringPeaceMode", true)]
		public float peaceModeMovementDelay = 1;
	}

	public MovementSettings movementSettings;
	public bool initializeSelf = true;
	private float lastMovementDuringPeaceMode;
	private Coroutine peaceModeMovementCoroutine;

	protected override void Start()
	{
		base.Start();
		if (initializeSelf)
		{
			MobController.Instance.InitializeMob(this, spawnPoint);
		}
	}

	public override void Initialize(Vector2Int location)
	{
		base.Initialize(location);
		if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.Peace
		    && movementSettings.moveDuringPeaceMode
		    && peaceModeMovementCoroutine == null)
		{
			peaceModeMovementCoroutine = StartCoroutine(PeaceModeMovement());
		}
	}

	public void MakeAction()
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
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void AssignRandomDirection()
	{
		movementSettings.movementDirection =
			EnumUtilities.RandomEnumValue<MovementDirectionUtilities.MovementDirection>();
	}

	protected override void Die()
	{
		MobController.Instance.RemoveMob(this);
	}

	protected override void InteractWithObject(Obstacle obstacle)
	{
	}

	protected override void InteractWithObject(Unit unit)
	{
	}

	protected override void PeaceState()
	{
		base.PeaceState();
		if (movementSettings.moveDuringPeaceMode && peaceModeMovementCoroutine == null)
		{
			peaceModeMovementCoroutine = StartCoroutine(PeaceModeMovement());
		}
	}

	protected override void FightState()
	{
		base.FightState();
		if (peaceModeMovementCoroutine != null)
		{
			StopCoroutine(peaceModeMovementCoroutine);
			peaceModeMovementCoroutine = null;
		}
	}

	private IEnumerator PeaceModeMovement()
	{
		while (true)
		{
			yield return new WaitForSeconds(movementSettings.peaceModeMovementDelay);
			MakeAction();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		var size = new Vector3(1, 1, 0.2f);
		Gizmos.DrawCube(CubeLocation(spawnPoint), size);
		if (movementSettings.typeOfMovement == TypeOfMovement.DefinedArea)
		{
			Gizmos.color = Color.magenta;
			foreach (var location in movementSettings.definedArea.locations)
			{
				Gizmos.DrawCube( CubeLocation(location), size);
			}
		}

		Vector3 CubeLocation(Vector2Int point)
		{
			return (Vector3Int) point + new Vector3(0.5f, 0.5f, 0);
		}
	}
}