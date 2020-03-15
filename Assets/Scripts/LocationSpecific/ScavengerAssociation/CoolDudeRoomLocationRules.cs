using UnityEngine;

public class CoolDudeRoomLocationRules : LocationRules
{
	[HideInInspector]
	public CoolDudeDetection caughtPlayer;

	protected override void Awake()
	{
		base.Awake();
		caughtPlayer = null;
	}

	protected override void OnBeatDone()
	{
		
	}

	protected override void CreateObservers()
	{
		base.CreateObservers();
		var observer = new Observer(this, null, PlayerSpotted);
		sceneObjects.beatListeners.Add(observer);
	}

	private void PlayerSpotted()
	{
		if (caughtPlayer)
		{
			
		}
	}
}