using System;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset;

	private Vector3 targetPosition;

	private void Awake()
	{
		Instance = this;
	}

	private void LateUpdate()
	{
		transform.position = Vector3.Lerp(transform.position, targetPosition,
			20 * Time.deltaTime);
	}

	public void ChangeTargetPosition(Vector3 targetPosition, bool force = false)
	{
		this.targetPosition = targetPosition + offset;
		if (force)
		{
			transform.position = this.targetPosition;
		}
	}

	public static GameCamera Instance { get; private set; }
}