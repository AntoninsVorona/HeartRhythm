using System.Collections;
using UnityEngine;

public class Player : Unit
{
	private bool acceptInput;

	private void Awake()
	{
		Instance = this;
		acceptInput = true;
	}

	private void Update()
	{
		if (acceptInput)
		{
			var horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
			var vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
			if (horizontal != 0)
			{
				Move(currentPosition + new Vector3Int(horizontal, 0, 0));
			}
			else if (vertical != 0)
			{
				Move(currentPosition + new Vector3Int(0, vertical, 0));
			}
		}
	}

	protected override IEnumerator MovementSequence(Vector3Int newPosition, float time = 0.2f)
	{
		acceptInput = false;
		yield return base.MovementSequence(newPosition, time);
		acceptInput = true;
	}

	protected override IEnumerator CantMoveSequence(Vector3Int newPosition, bool isHorizontal, float time = 0.2f)
	{
		acceptInput = false;
		yield return base.CantMoveSequence(newPosition, isHorizontal, time);
		acceptInput = true;
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