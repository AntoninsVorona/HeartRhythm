using System;
using System.Collections;
using UnityEngine;

public class Beat : MonoBehaviour
{
	public void Initialize(Vector2 startPosition, Vector2 endPosition, double startTime, double endTime)
	{
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

			yield return null;
		}
	}
}