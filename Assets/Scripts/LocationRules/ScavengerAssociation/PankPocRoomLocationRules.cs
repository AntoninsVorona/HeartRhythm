using UnityEngine;

public class PankPocRoomLocationRules : LocationRules
{
	[SerializeField]
	private PankPoc pankPoc;

	[SerializeField]
	private Obstacle chest;

	[SerializeField]
	private PankPocVisitCutScene pankPocVisitCutScene;
	[SerializeField]
	private GetKeyCutScene getKeyCutScene;

	protected override void Awake()
	{
		base.Awake();
		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedPankPoc)
		{
			SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedPankPoc = true;
			GameSessionManager.Instance.PlayCutScene(pankPocVisitCutScene);
		}
		else if (Player.Instance.HasItem("Microscheme"))
		{
			GameSessionManager.Instance.PlayCutScene(getKeyCutScene);
		}
		
		const string interceptionGuardName = "InterceptionGuard";
		var interceptionGuardData = GameSessionManager.Instance.currentLevelState.GetDataByName(interceptionGuardName);
		if (interceptionGuardData != null)
		{
			var interceptionGuard = (InterceptionGuard) sceneObjects.currentMobManager.SpawnMob(interceptionGuardName,
				interceptionGuardData.currentPosition,
				new Mob.MovementSettings {moveDuringPeaceMode = false});
			interceptionGuard.Rotate();
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

	public void Talk()
	{
		pankPoc.Talk("Sorry, Beatrice...");
	}
}