using UnityEngine;

public class MainCharacterRoomLocationRules : LocationRules
{
	protected override void Start()
	{
		base.Start();
		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.alreadyVisitedMainCharacterRoom)
		{
			SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.alreadyVisitedMainCharacterRoom =
				true;
			GameSessionManager.Instance.currentSceneObjects.currentObstacleManager.SpawnItemOnGround(
				ItemManager.Instance.GetItemByName("Funky"), 1, new Vector2Int(4, 0)
			);
		}
	}

	protected override void OnBeatDone()
	{
	}

	protected override void OnMoveDone()
	{
	}
}