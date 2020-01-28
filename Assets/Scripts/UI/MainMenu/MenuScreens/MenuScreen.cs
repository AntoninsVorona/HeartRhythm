using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MenuScreen : MonoBehaviour
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private float closeDuration = 1;
	
	public virtual void Open(bool withAnimation = true)
	{
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