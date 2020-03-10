using System.Collections;
using UnityEngine;

public class GameLogo : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup canvasGroup;
	
	public Coroutine Show()
	{
		gameObject.SetActive(true);
		return StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime * 3;
			canvasGroup.alpha = t;
			yield return null;
		}
	}

	public Coroutine Hide()
	{
		return StartCoroutine(HideSequence());
	}

	private IEnumerator HideSequence()
	{
		float t = 1;
		while (t > 0)
		{
			t -= Time.deltaTime * 3;
			canvasGroup.alpha = t;
			yield return null;
		}

		Deactivate();
	}
	
	public void Deactivate()
	{
		gameObject.SetActive(false);
	}
}