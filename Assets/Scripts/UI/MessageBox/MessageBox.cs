using System.Collections;
using TMPro;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TextMeshProUGUI text;

	private Coroutine showCoroutine;
	private Coroutine hideCoroutine;
	private bool showing;

	public void Show(string newText)
	{
		gameObject.SetActive(true);
		text.text = newText;
		showing = true;
		if (showCoroutine != null)
		{
			StopCoroutine(showCoroutine);
			showCoroutine = null;
		}

		if (hideCoroutine != null)
		{
			StopCoroutine(hideCoroutine);
			hideCoroutine = null;
		}

		showCoroutine = StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		animator.SetTrigger(AnimatorUtilities.SHOW_TRIGGER);
		yield return new WaitForSeconds(2);
		showCoroutine = null;
		Hide();
	}

	public void Hide(bool force = false)
	{
		if (force)
		{
			gameObject.SetActive(false);
			if (showCoroutine != null)
			{
				StopCoroutine(showCoroutine);
				showCoroutine = null;
			}

			if (hideCoroutine != null)
			{
				StopCoroutine(hideCoroutine);
				hideCoroutine = null;
			}

			showing = false;
		}
		else if (showing)
		{
			if (showCoroutine != null)
			{
				StopCoroutine(showCoroutine);
				showCoroutine = null;
			}

			if (hideCoroutine == null)
			{
				hideCoroutine = StartCoroutine(HideSequence());
			}
		}
	}

	private IEnumerator HideSequence()
	{
		animator.SetTrigger(AnimatorUtilities.HIDE_TRIGGER);
		yield return new WaitForSeconds(2);
		gameObject.SetActive(false);
		showing = false;
		hideCoroutine = null;
	}
}