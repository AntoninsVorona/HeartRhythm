using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
	public Grid grid;

	[SerializeField]
	private Vector3 characterDisplace;

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

	public void Move(Vector3Int newPosition, bool force = false)
	{
		if (World.Instance.CanWalk(newPosition))
		{
			currentPosition = newPosition;
			if (force)
			{
				transform.position = GetCurrentPosition();
			}
			else
			{
				StartCoroutine(MovementSequence(newPosition));
			}
		}
		else
		{
			StartCoroutine(CantMoveSequence(newPosition));
		}
	}

	private void Update()
	{
		if (acceptInput)
		{
			var horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
			var vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
			if (horizontal != 0 || vertical != 0)
			{
				Move(currentPosition + new Vector3Int(horizontal, vertical, 0));
			}
		}
	}

	private IEnumerator MovementSequence(Vector3Int newPosition, float time = 0.2f)
	{
		acceptInput = false;
		var start = transform.position;
		var end = GetPosition(newPosition);
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

			CharacterMovement(t, t, start, end, jumpStart);
		}

		acceptInput = true;
	}

	private IEnumerator CantMoveSequence(Vector3Int newPosition, float time = 0.2f)
	{
		acceptInput = false;
		GameCamera.Instance.StopFollowing();
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

			CharacterMovement(t, t, start, end, jumpStart);
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
			
			CharacterMovement(movementT, t, start, end, jumpStart);
		}

		GameCamera.Instance.StartFollowing();
		acceptInput = true;
	}

	private Vector3 GetCurrentPosition()
	{
		return grid.GetCellCenterWorld(currentPosition) + characterDisplace;
	}

	private Vector3 GetPosition(Vector3Int position)
	{
		return grid.GetCellCenterWorld(position) + characterDisplace;
	}

	private void CharacterMovement(float movementT, float characterDisplaceT, Vector3 start, Vector3 end,
		Vector3 jumpStart)
	{
		var x = Mathf.Lerp(start.x, end.x, movementT);
		var y = Mathf.Lerp(start.y, end.y, movementT);
		var spriteJump = jumpStart.y + movementDisplaceCurve.Evaluate(characterDisplaceT);
		transform.position = new Vector3(x, y, end.z);
		sprite.transform.localPosition = new Vector3(jumpStart.x, spriteJump, jumpStart.z);
	}

	public static Player Instance { get; private set; }
}