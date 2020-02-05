using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class MenuScreen : MonoBehaviour
{
	[SerializeField]
	protected Animator animator;

	[SerializeField]
	protected List<FillingButton> fillingButtons;

	public AbstractMainMenu.HeartSettings defaultHeartLocation = AbstractMainMenu.HeartSettings.DEFAULT_SETTINGS;
	public float openDuration = 1;
	public float closeDuration = 1;

	[SerializeField]
	private GameObject separatorGameObject;

	private HeartButton currentHeartButton;

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

		currentHeartButton = null;
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
	
	protected virtual void Update()
	{
		var hit = AbstractMainMenu.Instance.CurrentUIHit();
		if (hit)
		{
			var heartButton = hit.GetComponentInParent<HeartButton>();
			if (heartButton)
			{
				if (heartButton != currentHeartButton)
				{
					if (currentHeartButton)
					{
						currentHeartButton.Deselect();
					}

					currentHeartButton = heartButton;
					AbstractMainMenu.Instance.uiHeart.Reposition(currentHeartButton.Select());
				}

				if (Input.GetMouseButtonDown(0))
				{
					heartButton.Click();
				}
			}
			else if (currentHeartButton && hit != separatorGameObject)
			{
				currentHeartButton.Deselect();
				AbstractMainMenu.Instance.uiHeart.Reposition(defaultHeartLocation);
				currentHeartButton = null;
			}
		}
		else if (currentHeartButton)
		{
			currentHeartButton.Deselect();
			AbstractMainMenu.Instance.uiHeart.Reposition(defaultHeartLocation);
			currentHeartButton = null;
		}
	}

	private void Reset()
	{
		animator = GetComponent<Animator>();
	}

	public abstract void ApplyCancel();
}