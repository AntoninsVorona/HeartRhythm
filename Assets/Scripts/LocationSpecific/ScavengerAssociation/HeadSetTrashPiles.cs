using UnityEngine;

public class HeadSetTrashPiles : Obstacle
{
	[Header("Additional Data")]
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private HeadSetHidePlacesController headSetHidePlacesController;

	protected override void Start()
	{
		base.Start();
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER); //TODO Load
	}

	public void Interacted()
	{
		animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
		talksWhenInteractedWith = true;
		headSetHidePlacesController.ActivateHidePlaces();
	}
}