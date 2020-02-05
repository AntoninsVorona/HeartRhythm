using System.Collections;
using UnityEngine;

public class LetterPart : MonoBehaviour
{
	private const float VIBRATION_MIN_ROTATION = -2f;
	private const float VIBRATION_MAM_ROTATION = 2f;
	private const float SPEED = 250f;
	private const float SHRINK_END = 0.1f;
	private Coroutine vibrateCoroutine;

	[HideInInspector]
	public bool done;

	public void Vibrate()
	{
		done = false;
		vibrateCoroutine = StartCoroutine(VibrateSequence());
	}

	private IEnumerator VibrateSequence()
	{
		while (true)
		{
			transform.rotation = Quaternion.Euler(0, 0, Random.Range(VIBRATION_MIN_ROTATION, VIBRATION_MAM_ROTATION));
			yield return null;
		}
	}

	public void Fly(Vector2 point)
	{
		StopCoroutine(vibrateCoroutine);
		StartCoroutine(FlySequence(point));
	}

	private IEnumerator FlySequence(Vector2 point)
	{
		transform.rotation = Quaternion.identity;
		var rotation = Random.Range(VIBRATION_MIN_ROTATION, VIBRATION_MAM_ROTATION);
		// yield return RotateAround(point, rotation);
		yield return SuckIn(point, rotation);
	}

	private IEnumerator SuckIn(Vector2 point, float rotation)
	{
		const float duration = 1f;
		float t = 0;
		var startScale = Vector3.one;
		var endScale = Vector3.one * SHRINK_END;
		var closingDelta = Vector3.Distance(point, transform.position);
		while (t < 1)
		{
			yield return new WaitForFixedUpdate();
			var angleDelta = Time.fixedDeltaTime * SPEED;
			transform.Rotate(0, 0, rotation);
			transform.RotateAround(point, Vector3.forward, angleDelta);
			transform.position += ((Vector3) point - transform.position).normalized *
			                      (Time.fixedDeltaTime * closingDelta);
			t += Time.fixedDeltaTime / duration;
			transform.localScale = Vector3.Lerp(startScale, endScale, t);
		}

		transform.gameObject.SetActive(false);
		done = true;
	}
}