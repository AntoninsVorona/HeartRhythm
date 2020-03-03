using System;
using System.Collections;
using UnityEngine;

public class Beat : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	private bool odd;
	private static readonly int LEFT = Animator.StringToHash("Left");
	private static readonly int RIGHT = Animator.StringToHash("Right");

	public void Initialize(Vector2 startPosition, Vector2 endPosition, double startTime, double endTime, bool odd)
	{
		this.odd = odd;
		AudioManager.Instance.pulseSubscribers.Add(new AudioManager.PulseEventSubscriber(this, Bonk));
		StartCoroutine(Move(startPosition, endPosition, startTime, endTime));
	}

	private IEnumerator Move(Vector2 startPosition, Vector2 endPosition, double startTime, double endTime)
	{
		((RectTransform) transform).anchoredPosition = startPosition;
		float t = 0;
		while (t < 1)
		{
			t = Mathf.InverseLerp((float) startTime, (float) endTime, (float) AudioSettings.dspTime);

			((RectTransform) transform).anchoredPosition = Vector3.Lerp(startPosition, endPosition, t);
			if (t >= 1)
			{
				GameUI.Instance.beatController.BeatPlayed(this);
				yield break;
			}

			yield return new WaitForFixedUpdate();
		}
	}

	private void Bonk()
	{
		var bonkLeft = BeatController.bonkLeft;
		if (odd)
		{
			bonkLeft = !bonkLeft;
		}

		animator.SetTrigger(bonkLeft ? LEFT : RIGHT);
	}
}