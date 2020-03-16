using UnityEngine;

public class PankPocRoomLocationRules : LocationRules
{
	[SerializeField]
	private PankPoc pankPoc;

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
		
	}

	public void PlayHideHeadset()
	{
		
	}
}