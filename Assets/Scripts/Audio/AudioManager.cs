using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[Serializable]
	public class PulseEventSubscriber
	{
		public Action action;
		public double startTime;

		public PulseEventSubscriber(Action action, double startDelay)
		{
			this.action = action;
			startTime = PulseTime(startDelay);
		}

		public PulseEventSubscriber(Action action, double startDelay, double delay)
		{
			this.action = action;
			startTime = PulseTime(startDelay, delay);
		}

		private double PulseTime(double startDelay, double delay = 0)
		{
			return AudioSettings.dspTime + startDelay - Instance.beatDelay - delay;
		}
	}

	private const double TOLERANCE = 0.01;
	private const int BEATS_ON_SCREEN = 5;

	[HideInInspector]
	public List<PulseEventSubscriber> pulseSubscribers;

	public AudioSource musicAudioSource;

	[HideInNormalInspector]
	public double beatDelay;

	[HideInNormalInspector]
	public int bpm;

	[HideInNormalInspector]
	public double beatTravelTime;

	[HideInNormalInspector]
	public double startingDelay;

	private Music currentMusic;

	private Coroutine beatChecker;

	private bool isCurrentlyPlaying;

	private double currentTime;

	public double CurrentTime => currentTime;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		isCurrentlyPlaying = false;
	}

	public void InitializeMusic(Music music, bool isBattle)
	{
		beatDelay = 60 / (double) music.bpm;
		beatTravelTime = beatDelay * BEATS_ON_SCREEN;
		InitializeMusic(music, isBattle, beatTravelTime);
	}

	public void InitializeMusic(Music music, bool isBattle, double startingDelay)
	{
		bpm = music.bpm;
		beatDelay = 60 / (double) music.bpm;
		beatTravelTime = beatDelay * BEATS_ON_SCREEN;
		this.startingDelay = startingDelay;
		currentMusic = music;
		musicAudioSource.Stop();
		musicAudioSource.loop = currentMusic.loop;
		musicAudioSource.clip = currentMusic.audioClip;
		isCurrentlyPlaying = true;
		pulseSubscribers = new List<PulseEventSubscriber>();
		SchedulePlay();
		if (isBattle)
		{
			GameUI.Instance.beatController.InitializeBeatController();
		}
	}

	private void SchedulePlay()
	{
		musicAudioSource.PlayScheduled(AudioSettings.dspTime + startingDelay);
		beatChecker = StartCoroutine(BeatChecker(startingDelay));
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

			CheckPulses();

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

	private void CheckPulses()
	{
		var time = beatDelay - TOLERANCE;
		pulseSubscribers.RemoveAll(p => p == null);
		foreach (var pulseEventSubscriber in pulseSubscribers.Where(pulseEventSubscriber =>
			AudioSettings.dspTime - pulseEventSubscriber.startTime >= time))
		{
			pulseEventSubscriber.action();
			pulseEventSubscriber.startTime += beatDelay;
		}
	}

	public double GetTimeUntilNextPulse()
	{
		return AudioSettings.dspTime + currentTime;
	}

	public void ApplyBeat()
	{
		if (GameSessionManager.Instance.playState == GameSessionManager.PlayState.Basic)
		{
			GameSessionManager.Instance.currentSceneObjects.currentMobManager.MakeMobsActions();
		}
	}

	public static AudioManager Instance { get; private set; }
}