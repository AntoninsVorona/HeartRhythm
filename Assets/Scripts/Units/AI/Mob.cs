using System;
using System.Collections;
using UnityEngine;

public class Mob : Unit
{
	public enum TypeOfMovement
	{
		None = 0,
		Constant = 1,
		Random = 2,
		FollowPlayer = 3
	}

	[Serializable]
	public class MovementSettings
	{
		public TypeOfMovement typeOfMovement;
		public MovementDirectionUtilities.MovementDirection movementDirection;
		public bool moveDuringPeaceMode = true;
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

	public override void Initialize(Vector3Int location)
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
			case TypeOfMovement.FollowPlayer:
				AssignRandomDirection();
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

	protected override void GameStateChanged()
	{
		var newGameState = GameLogic.Instance.CurrentGameState;
		switch (newGameState)
		{
			case GameLogic.GameState.Peace:
				movementSpeed = defaultMovementSpeed;
				if (movementSettings.moveDuringPeaceMode && peaceModeMovementCoroutine == null)
				{
					peaceModeMovementCoroutine = StartCoroutine(PeaceModeMovement());
				}

				break;
			case GameLogic.GameState.Fight:
				movementSpeed = 3 + 0.03f * AudioManager.Instance.bpm;
				if (peaceModeMovementCoroutine != null)
				{
					StopCoroutine(peaceModeMovementCoroutine);
					peaceModeMovementCoroutine = null;
				}

				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(newGameState), newGameState, null);
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

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawCube(spawnPoint + new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0.2f));
	}
}