using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class GameCamera : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset;

	private Vector3 targetPosition;

	[HideInNormalInspector]
	public new Camera camera;

	[HideInNormalInspector]
	public bool staticView;

	private Coroutine shakeCoroutine;
	private Vector3 shakeOffset, shakeRot;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
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
		newPosition += shakeOffset;
		transform.position = newPosition;
		transform.rotation = Quaternion.Euler(shakeRot);
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

	public void Shake(float duration = 0.4f, float posPower = 0.075f, float rotPower = 2.5f)
	{
		if (shakeCoroutine != null)
		{
			StopCoroutine(shakeCoroutine);
		}

		shakeCoroutine = StartCoroutine(ShakeSequence(duration, posPower, rotPower));
	}

	private IEnumerator ShakeSequence(float duration, float posPower, float rotPower)
	{
		const int shakeSegments = 10;
		float t = 0;
		float t2Duration = duration / shakeSegments;
		float tSegmentSize = 1f / shakeSegments;

		while (t < 1)
		{
			float t2 = 0;

			float posAmplitude = Mathf.Lerp(posPower, 0, t);
			float rotAmplitude = Mathf.Lerp(rotPower, 0, t);

			Vector3 shakeRotStart = shakeRot;
			Vector3 shakeOffsetStart = shakeOffset;
			Vector3 shakeRotEnd = Vector3.zero;
			Vector3 shakeOffsetEnd = Vector3.zero;

			if (t >= 1 - tSegmentSize)
			{
				shakeRotEnd = Vector3.zero;
				shakeOffsetEnd = Vector3.zero;
			}
			else
			{
				shakeRotEnd = new Vector3(0, 0, Random.Range(-rotAmplitude, rotAmplitude));
				shakeOffsetEnd = Random.insideUnitSphere * posAmplitude;
			}

			while (t2 < 1)
			{
				t2 += Time.deltaTime / t2Duration;

				shakeRot = Vector3.Lerp(shakeRotStart, shakeRotEnd, t2);
				shakeOffset = Vector3.Lerp(shakeOffsetStart, shakeOffsetEnd, t2);

				yield return null;
			}

			t += tSegmentSize;
		}

		shakeRot = Vector3.zero;
		shakeOffset = Vector3.zero;
	}

	public static GameCamera Instance { get; private set; }
}