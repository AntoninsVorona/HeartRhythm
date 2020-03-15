using UnityEngine;

public class CoolDudeRoomLocationRules : LocationRules
{
	[HideInInspector]
	public bool spotted;

	protected override void Awake()
	{
		base.Awake();
		spotted = false;
	}

	protected override void OnBeatDone()
	{
		
	}

	public void PlayerSpotted(CoolDudeDetection coolDudeDetection)
	{
		if (!spotted)
		{
			spotted = true;
			GameCamera.Instance.ChangeTargetPosition(coolDudeDetection.transform.position);
			GameCamera.Instance.staticView = true;
			GameSessionManager.Instance.StartConversation("CoolDudeSpotted");
		}
	}
}