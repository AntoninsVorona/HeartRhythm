using System.Collections;
using TMPro;
using UnityEngine;

public class AnnouncerTitle : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private CanvasGroup canvasGroup;

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public Coroutine Show(string title)
	{
		gameObject.SetActive(true);
		text.text = title;
		canvasGroup.alpha = 0;
		return StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime / 2;
			canvasGroup.alpha = t;
			yield return null;
		}
	}
}