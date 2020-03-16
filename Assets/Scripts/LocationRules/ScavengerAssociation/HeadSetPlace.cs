using UnityEngine;

public class HeadSetPlace : Obstacle
{
	[Header("Additional Data")]
	[SerializeField]
	private Interaction acquireHeadsetInteraction;

	protected override void Start()
	{
		base.Start();
		if (Player.Instance.HasItem("HeadsetChestKey"))
		{
			ActivateHidePlace();
		}
		else
		{
			DisableHidePlace();
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