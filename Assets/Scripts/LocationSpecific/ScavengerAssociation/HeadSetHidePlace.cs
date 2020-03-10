using UnityEngine;

public class HeadSetHidePlace : Obstacle
{
	[Header("Additional Data")]
	[SerializeField]
	private bool canHide;

	[SerializeField]
	private Interaction hideInteraction;

	[SerializeField]
	private Interaction getHeadSetBack;

	public void HeadSetStateChanged(HeadSetHideAndSeekController.HeadSetState newState)
	{
		switch (newState)
		{
			case HeadSetHideAndSeekController.HeadSetState.FindSeekPlace:
				ActivateHidePlace(true);
				break;
			case HeadSetHideAndSeekController.HeadSetState.Hidden
				when canHide &&
				     !SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.playerHasToSleepFirst:
				ActivateHidePlace(false);
				break;
			default:
				DisableHidePlace();
				break;
		}
	}

	public static void HeadSetHidden()
	{
		HeadSetHideAndSeekController.Instance.ChangeHeadSetState(HeadSetHideAndSeekController.HeadSetState.Hidden);
	}

	public static void HeadSetTakenBack()
	{
		HeadSetHideAndSeekController.Instance.ChangeHeadSetState(HeadSetHideAndSeekController.HeadSetState.OnPlayer);
	}

	private void ActivateHidePlace(bool hide)
	{
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		if (canHide)
		{
			headsetLessInteraction = hide ? hideInteraction : getHeadSetBack;
			InitializeInteractions();
		}
		else
		{
			talksWhenInteractedWith = true;
		}
	}

	private void DisableHidePlace()
	{
		talksWhenInteractedWith = false;
		interactions.Clear();
		headsetLessInteraction = null;
		animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
	}
}