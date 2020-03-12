using UnityEngine;

public class HeadSetPlace : Obstacle
{
	public enum HeadSetState
	{
		CantGetYet = 0,
		CanGet = 1, //TODO
		OnPlayer = 2
	}

	[Header("Additional Data")]
	[SerializeField]
	private Interaction acquireHeadsetInteraction;

	private void Awake()
	{
		HeadSetStateChanged();
	}

	public void ChangeHeadSetState(HeadSetState headSetState)
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.headSetState = headSetState;
		HeadSetStateChanged();
	}

	private void HeadSetStateChanged()
	{
		var headSetState = SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.headSetState;
		switch (headSetState)
		{
			case HeadSetState.CanGet:
				ActivateHidePlace();
				break;
			default:
				DisableHidePlace();
				break;
		}
	}

	private void ActivateHidePlace()
	{
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		headsetLessInteraction = acquireHeadsetInteraction;
		InitializeInteractions();
	}

	private void DisableHidePlace()
	{
		talksWhenInteractedWith = false;
		interactions.Clear();
		headsetLessInteraction = null;
		animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
	}
}