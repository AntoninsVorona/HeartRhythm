public class InGameAudioManager : AudioManager
{
	protected override void StopOtherElements()
	{
		base.StopOtherElements();
		GameUI.Instance.beatController.StopPlaying();
		PlayerInput.Instance.acceptor.BeatIsValid = false;
		PlayerInput.Instance.acceptor.ReceivedInputThisTimeFrame = false;
	}
	
	protected override void TimeIsValid()
	{
		base.TimeIsValid();
		PlayerInput.Instance.acceptor.BeatIsValid = true;
	}

	protected override void TimeIsInvalid(bool timeWasValidAFrameAgo)
	{
		base.TimeIsInvalid(timeWasValidAFrameAgo);
		var playerInput = PlayerInput.Instance;
		playerInput.acceptor.BeatIsValid = false;
		if (playerInput.acceptor.ReceivedInputThisTimeFrame)
		{
			playerInput.acceptor.ReceivedInputThisTimeFrame = false;
		}
		else if (playerInput.acceptor.FirstBattleInputDone &&
		         timeWasValidAFrameAgo)
		{
			playerInput.MissedBeat();
			ApplyBeat();
		}
	}

	public void ApplyBeat()
	{
		if (GameSessionManager.Instance.playState == GameSessionManager.PlayState.Basic)
		{
			GameSessionManager.Instance.currentSceneObjects.currentMobManager.MakeMobsActions();
		}
	}
}