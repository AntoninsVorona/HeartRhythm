using UnityEngine;

public class PankPocRoomLocationRules : LocationRules
{
	[SerializeField]
	private PankPoc pankPoc;

	[SerializeField]
	private Obstacle chest;

	[SerializeField]
	private PankPocVisitCutScene pankPocVisitCutScene;

	protected override void Awake()
	{
		base.Awake();
		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedPankPoc)
		{
			SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedPankPoc = true;
			GameSessionManager.Instance.PlayCutScene(pankPocVisitCutScene);
		}
	}

	protected override void OnBeatDone()
	{
	}

	protected override void OnMoveDone()
	{
	}

	public void RotatePankPoc()
	{
		pankPoc.TurnAround();
	}
	
	public void MovePankPoc(Vector2Int pos)
	{
		pankPoc.Move(pos);
	}

	public void PlayShowHeadset()
	{
		pankPoc.animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		chest.animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
	}

	public void PlayHideHeadset()
	{
		pankPoc.animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
		chest.animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
	}
}