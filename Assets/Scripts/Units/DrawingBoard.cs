using System.Collections;
using TMPro;
using UnityEngine;

public class DrawingBoard : MonoBehaviour
{
	public Canvas canvas;
	public Animator drawingBoardAnimator;
	public TextMeshProUGUI displayText;
	[HideInInspector]
	public float talkTimer;
	[HideInInspector]
	public Coroutine talkCoroutine;
	private bool canUpdateTalkTimer;
	
	public void Initialize()
	{
		gameObject.SetActive(true);
		canvas.worldCamera = GameCamera.Instance.camera;
		canvas.gameObject.SetActive(false);
	}
	
	public void Talk(MonoBehaviour coroutineStarter, string text)
	{
		if (talkCoroutine != null)
		{
			if (canUpdateTalkTimer)
			{
				UpdateTalkTimer(text);
			}
			else
			{
				coroutineStarter.StopCoroutine(talkCoroutine);
				talkCoroutine = coroutineStarter.StartCoroutine(TalkCoroutine(text));
			}
		}
		else
		{
			talkCoroutine = coroutineStarter.StartCoroutine(TalkCoroutine(text));
		}
	}

	public void StopTalk(MonoBehaviour coroutineStarter, bool force)
	{
		if (force)
		{
			if (canvas)
			{
				canvas.gameObject.SetActive(false);
			}

			if (talkCoroutine != null)
			{
				coroutineStarter.StopCoroutine(talkCoroutine);
				talkCoroutine = null;
			}
		}
		else
		{
			if (talkCoroutine != null)
			{
				talkTimer = 0;
			}
		}
	}
	
	private void UpdateTalkTimer(string text = null)
	{
		if (text != null)
		{
			displayText.text = text;
		}

		talkTimer = Time.time + 3;
	}

	private IEnumerator TalkCoroutine(string text)
	{
		canUpdateTalkTimer = true;
		displayText.text = text;
		canvas.gameObject.SetActive(true);
		displayText.gameObject.SetActive(false);
		drawingBoardAnimator.SetTrigger(AnimatorUtilities.SHOW_TRIGGER);
		yield return new WaitForSeconds(0.6f);
		displayText.gameObject.SetActive(true);
		UpdateTalkTimer();
		yield return new WaitUntil(() => Time.time > talkTimer);
		canUpdateTalkTimer = false;
		displayText.gameObject.SetActive(false);
		drawingBoardAnimator.SetTrigger(AnimatorUtilities.HIDE_TRIGGER);
		yield return new WaitForSeconds(0.6f);
		canvas.gameObject.SetActive(false);
		talkCoroutine = null;
	}
}