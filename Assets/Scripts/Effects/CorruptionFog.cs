using System;
using System.Collections;
using UnityEngine;

public class CorruptionFog : MonoBehaviour
{
	[SerializeField]
	private AnimationCurve flyCurve;

	[SerializeField]
	private Animator animator;

	public void Fly(Action action)
	{
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		StartCoroutine(FlySequence(action));
	}

	private IEnumerator FlySequence(Action action)
	{
		var startPosition = transform.position;
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime * 3;
			var target = Player.Instance.transform.position + new Vector3(0, 0.5f);
			transform.position = Vector3.Lerp(startPosition, target, flyCurve.Evaluate(t));
			yield return null;
		}

		action?.Invoke();
		Destroy(gameObject);
	}
}