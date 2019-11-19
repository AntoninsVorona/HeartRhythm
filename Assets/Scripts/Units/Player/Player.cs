using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
	private Unit interactingWithUnit;
	
	private void Awake()
	{
		Instance = this;
	}

	protected override void Start()
	{
		base.Start();
		Initialize(spawnPoint);
	}

	protected override void ForceMove()
	{
		base.ForceMove();
		GameCamera.Instance.ChangeTargetPosition(transform.position, true);
	}

	public void ReceiveInput(MovementDirectionUtilities.MovementDirection movementDirection, bool spaceClicked)
	{
		switch (GameLogic.Instance.playState)
		{
			case GameLogic.PlayState.Basic:
				Move(movementDirection);
				break;
			case GameLogic.PlayState.DanceMove:
				if (spaceClicked)
				{
					EndDanceMove();
				}
				else
				{
					ReceiveDanceMove(movementDirection);
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ReceiveDanceMove(MovementDirectionUtilities.MovementDirection movementDirection)
	{
		//TODO Play Animation
		PlayerInput.Instance.acceptor.danceMoveSet.Add(movementDirection);
	}

	private void EndDanceMove()
	{
		//TODO Play Animation
		GameLogic.Instance.FinishDanceMove();
	}

	protected override IEnumerator MovementSequence(Vector2Int newPosition)
	{
		PlayerInput.Instance.acceptor.PlayerReadyForInput = false;
		yield return base.MovementSequence(newPosition);
		PlayerInput.Instance.acceptor.PlayerReadyForInput = true;
	}

	protected override IEnumerator CantMoveSequence(Vector2Int newPosition, bool isHorizontal)
	{
		PlayerInput.Instance.acceptor.PlayerReadyForInput = false;
		yield return base.CantMoveSequence(newPosition, isHorizontal);
		PlayerInput.Instance.acceptor.PlayerReadyForInput = true;
	}

	protected override void CharacterMovement(float movementT, float characterDisplaceT, Vector3 start, Vector3 end,
		bool jump, Vector3 jumpStart, bool updateCamera = true)
	{
		base.CharacterMovement(movementT, characterDisplaceT, start, end, jump, jumpStart, updateCamera);
		if (updateCamera)
		{
			GameCamera.Instance.ChangeTargetPosition(transform.position);
		}
	}

	protected override void Die()
	{
		//TODO Lose Game
	}

	protected override void InteractWithObject(Unit unit)
	{
		interactingWithUnit = unit;
		GameLogic.Instance.StartDanceMove();
	}

	public void ApplyDanceMoveSet(List<MovementDirectionUtilities.MovementDirection> acceptorDanceMoveSet)
	{
		var interaction = interactingWithUnit.InteractionMatches(acceptorDanceMoveSet);
		if (interaction != null)
		{
			//TODO Interaction
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawCube((Vector3Int) spawnPoint + new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0.2f));
	}

	public static Player Instance { get; private set; }
}