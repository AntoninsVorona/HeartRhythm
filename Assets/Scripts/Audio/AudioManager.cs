using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField]
	public AudioSource musicAudioSource;

	[HideInNormalInspector]
	public double beatDelay;

	[HideInNormalInspector]
	public int bpm;

	private Music currentMusic;
	private Coroutine beatChecker;
	private bool isCurrentlyPlaying;
	private double currentTime;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		isCurrentlyPlaying = false;
	}

	public void InitializeBattle(Music fightMusic)
	{
		bpm = fightMusic.bpm;
		beatDelay = 60 / (double) fightMusic.bpm;
		currentMusic = fightMusic;
		musicAudioSource.Stop();
		musicAudioSource.loop = currentMusic.loop;
		musicAudioSource.clip = currentMusic.audioClip;
		isCurrentlyPlaying = true;
		GameUI.Instance.beatController.StartBeat();
	}

	public void SchedulePlay(double travelTime)
	{
		musicAudioSource.PlayScheduled(AudioSettings.dspTime + travelTime);
		beatChecker = StartCoroutine(BeatChecker(travelTime));
	}

	public void StopBeat()
	{
		if (isCurrentlyPlaying)
		{
			musicAudioSource.Stop();
			GameUI.Instance.beatController.StopPlaying();
			if (beatChecker != null)
			{
				StopCoroutine(beatChecker);
			}

			PlayerInput.Instance.acceptor.BeatIsValid = false;
			PlayerInput.Instance.acceptor.ReceivedInputThisTimeFrame = false;
			isCurrentlyPlaying = false;
		}
	}

	private IEnumerator BeatChecker(double travelTime)
	{
		const double lowerAccuracy = 0.1;
		const double upperAccuracy = 0.1;
		var startTime = AudioSettings.dspTime + travelTime;
		var timeWasValidAFrameAgo = false;
		var timeInLowerBounds = false;
		while (true)
		{
			var timeIsValid = IsTimeValid();

			if (timeIsValid)
			{
				timeWasValidAFrameAgo = true;
				PlayerInput.Instance.acceptor.BeatIsValid = true;
			}
			else
			{
				PlayerInput.Instance.acceptor.BeatIsValid = false;
				if (PlayerInput.Instance.acceptor.ReceivedInputThisTimeFrame)
				{
					PlayerInput.Instance.acceptor.ReceivedInputThisTimeFrame = false;
				}
				else if (PlayerInput.Instance.acceptor.FirstBattleInputDone &&
				         timeWasValidAFrameAgo)
				{
					PlayerInput.Instance.MissedBeat();
					ApplyBeat();
				}

				timeWasValidAFrameAgo = false;
			}

			yield return null;
		}

		bool IsTimeValid()
		{
			currentTime = (AudioSettings.dspTime - startTime) % beatDelay;
			var timeIsValid = false;
			if (timeInLowerBounds)
			{
				if (currentTime > beatDelay - lowerAccuracy)
				{
					timeIsValid = true;
				}
				else
				{
					if (currentTime < upperAccuracy)
					{
						timeIsValid = true;
					}
					else
					{
						timeInLowerBounds = false;
					}
				}
			}
			else
			{
				timeInLowerBounds = currentTime > beatDelay - lowerAccuracy;
				if (timeInLowerBounds)
				{
					timeIsValid = true;
				}
			}

			return timeIsValid;
		}
	}

	public void ApplyBeat()
	{
		if (GameLogic.Instance.playState == GameLogic.PlayState.Basic)
		{
			GameLogic.Instance.currentSceneObjects.currentMobManager.MakeMobsActions();
		}
	}

	public static AudioManager Instance { get; private set; }
}