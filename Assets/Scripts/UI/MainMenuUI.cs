using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
	public CanvasGroup globalCanvasGroup;
	public Animator fadeAnimator;
	public Animator heartBeatAnimator;
	
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

		globalCanvasGroup.interactable = false;
	}

	public void Show()
	{
		StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		fadeAnimator.SetTrigger(AnimatorUtilities.HIDE_TRIGGER);
		yield return new WaitForSeconds(1);
		globalCanvasGroup.interactable = true;
		heartBeatAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
	}
	
	public static MainMenuUI Instance { get; private set; }
}