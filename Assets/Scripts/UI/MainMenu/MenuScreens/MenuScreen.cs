using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MenuScreen : MonoBehaviour
{
	[SerializeField]
	protected Animator animator;

	[SerializeField]
	protected List<FillingButton> fillingButtons;

	public MainMenuUI.HeartSettings defaultHeartLocation = MainMenuUI.HeartSettings.DEFAULT_SETTINGS;
	public float openDuration = 1;
	public float closeDuration = 1;

	public virtual void Open(bool withAnimation = true)
	{
		foreach (var heartButton in fillingButtons)
		{
			heartButton.ResetFill();
		}
		
		gameObject.SetActive(true);
		if (withAnimation)
		{
			animator.SetTrigger(AnimatorUtilities.SHOW_TRIGGER);
		}
	}

	public virtual Coroutine Close(bool withAnimation = true)
	{
		if (withAnimation)
		{
			return StartCoroutine(CloseSequence());
		}

		gameObject.SetActive(false);
		return null;
	}

	private IEnumerator CloseSequence()
	{
		animator.SetTrigger(AnimatorUtilities.HIDE_TRIGGER);
		yield return new WaitForSeconds(closeDuration);
		gameObject.SetActive(false);
	}

	private void Reset()
	{
		animator = GetComponent<Animator>();
	}
}