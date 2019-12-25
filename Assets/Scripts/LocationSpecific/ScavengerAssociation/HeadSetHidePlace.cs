using UnityEngine;

public class HeadSetHidePlace : Obstacle
{
	[Header("Additional Data")]
	[SerializeField]
	private bool canHide;

	[SerializeField]
	private Interaction interactionToAdd;

	[SerializeField]
	private Animator animator;
	
	[SerializeField]
	private HeadSetHidePlacesController headSetHidePlacesController;

	protected override void Start()
	{
		base.Start();
		talksWhenInteractedWith = false;
		interactions.Clear();
		animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
	}

	public void StartBlinking()
	{
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		if (canHide)
		{
			interactions.Add(interactionToAdd);
			InitializeInteractions();
		}
		else
		{
			talksWhenInteractedWith = true;
		}
	}

	public void Interacted()
	{
		headSetHidePlacesController.HeadSetIsHidden();
	}
	
	public void HeadSetIsHidden()
	{
		talksWhenInteractedWith = false;
		interactions.Clear();
		animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
	}
}