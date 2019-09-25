using System.Collections;
using UnityEngine;

public class Player : Unit
{
	private void Awake()
	{
		Instance = this;
	}
	
	public void ReceiveInput(int horizontal, int vertical)
	{
		if (horizontal != 0)
		{
			Move(currentPosition + new Vector3Int(horizontal, 0, 0));
		}
		else if (vertical != 0)
		{
			Move(currentPosition + new Vector3Int(0, vertical, 0));
		}
	}

	protected override IEnumerator MovementSequence(Vector3Int newPosition)
	{
		PlayerInput.Instance.acceptor.PlayerReadyForInput = false;
		yield return base.MovementSequence(newPosition);
		PlayerInput.Instance.acceptor.PlayerReadyForInput = true;
	}

	protected override IEnumerator CantMoveSequence(Vector3Int newPosition, bool isHorizontal)
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

	public static Player Instance { get; private set; }
}