using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer sprite;

	[SerializeField]
	private AnimationCurve movementSpeedCurve;

	[SerializeField]
	private AnimationCurve movementDisplaceCurve;

	private Vector3Int currentPosition;
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

	public void Move(Vector3Int newPosition, bool force = false)
	{
		if (World.Instance.CanWalk(newPosition))
		{
			currentPosition = newPosition;
			if (force)
			{
				transform.position = GetCurrentPosition();
				GameCamera.Instance.ChangeTargetPosition(transform.position, true);
			}
			else
			{
				StartCoroutine(MovementSequence(newPosition));
			}
		}
		else
		{
			StartCoroutine(CantMoveSequence(newPosition, currentPosition.x != newPosition.x));
		}
	}

	private IEnumerator MovementSequence(Vector3Int newPosition, float time = 0.2f)
	{
		acceptInput = false;
		var start = transform.position;
		var end = GetPosition(newPosition);
		sprite.flipX = start.x > end.x;
		var jumpStart = sprite.transform.localPosition;
		float t = 0;
		while (t < 1)
		{
			yield return null;
			t += Time.deltaTime * 5;
			if (t > 1)
			{
				t = 1;
			}

			CharacterMovement(t, t, start, end, true, jumpStart);
		}

		acceptInput = true;
	}

	private IEnumerator CantMoveSequence(Vector3Int newPosition, bool isHorizontal, float time = 0.2f)
	{
		acceptInput = false;
		var start = transform.position;
		var end = GetPosition(newPosition);
		var jumpStart = sprite.transform.localPosition;
		float t = 0;
		const float moveUntil = 0.3f;
		while (t < moveUntil)
		{
			yield return null;
			t += Time.deltaTime * 5;
			if (t > moveUntil)
			{
				t = moveUntil;
			}

			CharacterMovement(t, t, start, end, isHorizontal, jumpStart, false);
		}

		end = start;
		start = transform.position;
		const float modifier = 1 / moveUntil;
		float movementT = 0;
		while (t < 1f)
		{
			yield return null;
			var timeTick = Time.deltaTime * 5;
			t += timeTick;
			if (t > 1)
			{
				t = 1;
			}

			movementT += timeTick * modifier;

			if (movementT > 1)
			{
				movementT = 1;
			}

			CharacterMovement(movementT, t, start, end, isHorizontal, jumpStart, false);
		}

		acceptInput = true;
	}

	private Vector3 GetCurrentPosition()
	{
		return World.Instance.grid.GetCellCenterWorld(currentPosition);
	}

	private Vector3 GetPosition(Vector3Int position)
	{
		return World.Instance.grid.GetCellCenterWorld(position);
	}

	private void CharacterMovement(float movementT, float characterDisplaceT, Vector3 start, Vector3 end,
		bool jump, Vector3 jumpStart, bool updateCamera = true)
	{
		var speed = movementSpeedCurve.Evaluate(movementT);
		var x = Mathf.Lerp(start.x, end.x, speed);
		var y = Mathf.Lerp(start.y, end.y, speed);
		transform.position = new Vector3(x, y, end.z);
		if (jump)
		{
			var spriteJump = jumpStart.y + movementDisplaceCurve.Evaluate(characterDisplaceT);
			sprite.transform.localPosition = new Vector3(jumpStart.x, spriteJump, jumpStart.z);
		}

		if (updateCamera)
		{
			GameCamera.Instance.ChangeTargetPosition(transform.position);
		}
	}

	public static Player Instance { get; private set; }
}