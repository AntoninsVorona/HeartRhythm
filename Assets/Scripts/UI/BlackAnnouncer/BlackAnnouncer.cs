using System;
using System.Collections;
using UnityEngine;

public class BlackAnnouncer : MonoBehaviour
{
	[Serializable]
	public class AnnouncementData
	{
		public string middleTitle;
		public string smallTitle;

		public AnnouncementData(string middleTitle, string smallTitle)
		{
			this.middleTitle = middleTitle;
			this.smallTitle = smallTitle;
		}
	}

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private AnnouncerTitle middleTitle;

	[SerializeField]
	private AnnouncerTitle smallTitle;

	public Coroutine Show(AnnouncementData announcementData)
	{
		gameObject.SetActive(true);
		return StartCoroutine(ShowSequence(announcementData));
	}

	private IEnumerator ShowSequence(AnnouncementData announcementData)
	{
		smallTitle.Deactivate();
		if (string.IsNullOrEmpty(announcementData.middleTitle))
		{
			middleTitle.Deactivate();
		}
		else
		{
			yield return middleTitle.Show(announcementData.middleTitle);
		}

		if (!string.IsNullOrEmpty(announcementData.smallTitle))
		{
			yield return new WaitForSeconds(1);
			yield return smallTitle.Show(announcementData.smallTitle);
		}

		yield return new WaitForSeconds(2);
	}

	public Coroutine Close(bool force = false)
	{
		if (force)
		{
			gameObject.SetActive(false);
			return null;
		}

		return StartCoroutine(CloseSequence());
	}

	private IEnumerator CloseSequence()
	{
		float t = 1;
		while (t > 0)
		{
			t -= Time.deltaTime / 2;
			canvasGroup.alpha = t;
			yield return null;
		}

		gameObject.SetActive(false);
	}
}