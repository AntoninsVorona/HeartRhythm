using UnityEngine;

public class CoolDudeRoomLocationRules : LocationRules
{
	[HideInInspector]
	public bool spotted;

	[SerializeField]
	private CoolDudeCutScene coolDudeCutScene;

	[SerializeField]
	private LevelData levelToLoad;

	[SerializeField]
	private SpriteRenderer bananaPortrait;

	[SerializeField]
	private Sprite bananaSprite;

	[SerializeField]
	private Sprite noBananaSprite;

	[SerializeField]
	private Obstacle bananaObstacle;

	protected override void Awake()
	{
		base.Awake();
		spotted = false;
		UpdateBananaSprite();
		if (!SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedCoolDude)
		{
			SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.visitedCoolDude = true;
			GameSessionManager.Instance.PlayCutScene(coolDudeCutScene);
		}
	}

	public void BananaTaken()
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.bananaTaken = true;
		Player.Instance.PickUpItem(ItemManager.Instance.GetItemByName("Banana"));
		UpdateBananaSprite();
	}

	private void UpdateBananaSprite()
	{
		if (SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.bananaTaken)
		{
			bananaPortrait.sprite = noBananaSprite;
			bananaObstacle.headsetLessInteraction = null;
			bananaObstacle.interactions.Clear();
			bananaObstacle.talksWhenInteractedWith = true;
		}
		else
		{
			bananaPortrait.sprite = bananaSprite;
		}
	}

	protected override void OnBeatDone()
	{
	}

	protected override void OnMoveDone()
	{
		sceneObjects.currentMobManager.ApplyActionOnUnits(m =>
		{
			if (m is CoolDudeDetection coolDudeDetection)
			{
				coolDudeDetection.CheckPlayer();
			}
		});
	}

	public void PlayerSpotted(CoolDudeDetection coolDudeDetection)
	{
		if (!spotted)
		{
			spotted = true;
			GameCamera.Instance.ChangeTargetPosition(coolDudeDetection.transform.position);
			GameCamera.Instance.staticView = true;
			GameSessionManager.Instance.StartConversation("CoolDudeSpotted", transform);
		}
	}

	public void LoadLevel()
	{
		GameSessionManager.Instance.LoadLevel(levelToLoad, 2);
	}
}