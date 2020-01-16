using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class GameCamera : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset;

	private Vector3 targetPosition;
	[HideInNormalInspector]
	public new Camera camera;

	[HideInNormalInspector]
	public bool staticView;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		camera = GetComponent<Camera>();
	}

	private void LateUpdate()
	{
		var newPosition = Vector3.Lerp(transform.position, targetPosition,
			20 * Time.deltaTime);
		newPosition.z = offset.z;
		transform.position = newPosition;
	}

	public void ChangeTargetPosition(Vector3 targetPosition, bool force = false)
	{
		if (!staticView)
		{
			this.targetPosition = targetPosition + offset;
			if (force)
			{
				transform.position = this.targetPosition;
			}
		}
	}
	
	public static GameCamera Instance { get; private set; }
}