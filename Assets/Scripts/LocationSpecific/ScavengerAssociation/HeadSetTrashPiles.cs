using System;
using UnityEngine;

public class HeadSetTrashPiles : Obstacle
{
	[Header("Additional Data")]
	[SerializeField]
	private Animator animator;

	public void HeadSetStateChanged(HeadSetHideAndSeekController.HeadSetState newState)
	{
		switch (newState)
		{
			case HeadSetHideAndSeekController.HeadSetState.InTrash:
				talksWhenInteractedWith = false;
				animator.SetTrigger(AnimatorUtilities.START_TRIGGER); 
				break;
			default:
				talksWhenInteractedWith = true;
				animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
				break;
		}
	}

	public static void Interacted()
	{
		HeadSetHideAndSeekController.Instance.ChangeHeadSetState(HeadSetHideAndSeekController.HeadSetState.FindSeekPlace);
	}
}