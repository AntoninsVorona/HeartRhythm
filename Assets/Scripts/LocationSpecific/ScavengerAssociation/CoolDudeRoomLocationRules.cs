using UnityEngine;

public class CoolDudeRoomLocationRules : LocationRules
{
	[HideInInspector]
	public bool spotted;

	[SerializeField]
	private LevelData levelToLoad;

	protected override void Awake()
	{
		base.Awake();
		spotted = false;
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
			GameCamera.Instance.ChangeTargetPosition(coolDudeDetection.transform.position, true);
			GameCamera.Instance.staticView = true;
			GameSessionManager.Instance.StartConversation("CoolDudeSpotted", transform);
		}
	}

	public void LoadLevel()
	{
		GameSessionManager.Instance.LoadLevel(levelToLoad, 2);
	}
}