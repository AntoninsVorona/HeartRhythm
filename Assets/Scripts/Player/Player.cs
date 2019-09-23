using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
	public Grid grid;

	[SerializeField]
	private Vector3 characterDisplace;

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

	private void Start()
	{
		Move(Vector3Int.zero);
	}

	private void Move(Vector3Int newPosition)
	{
		currentPosition = newPosition;
		StartCoroutine(MovementSequence());
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

	private IEnumerator MovementSequence(float time = 0.2f)
	{
		acceptInput = false;
		var start = transform.position;
		var end = grid.GetCellCenterWorld(currentPosition) + characterDisplace;
		float t = 0;
		while (t < 1)
		{
			yield return null;
			t += Time.deltaTime * 5;
			var speed = movementSpeedCurve.Evaluate(t);
			var x = Mathf.Lerp(start.x, end.x, speed);
			var y = Mathf.Lerp(start.y, end.y, speed);
			var z = start.z - movementDisplaceCurve.Evaluate(t);
			transform.position = new Vector3(x, y, z);
		}

		acceptInput = true;
	}

	public static Player Instance { get; private set; }
}