using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
	[Serializable]
	public class MusicSettings
	{
		public const float NORMAL_LOWPASS = 22000;
		public const float MIN_LOWPASS = 300;
		public float lowPass;

		public MusicSettings(float lowPass)
		{
			this.lowPass = lowPass;
		}

		public bool Equals(MusicSettings musicSettings)
		{
			return Mathf.RoundToInt(musicSettings.lowPass) == Mathf.RoundToInt(lowPass);
		}

		public static readonly MusicSettings DEFAULT_SETTINGS = new MusicSettings(NORMAL_LOWPASS);
	}

	[Serializable]
	public class PulseEventSubscriber
	{
		public UnityEngine.Object owner;
		public Action action;
		public double startTime;
		public bool ignoreMusicStartTime;

		public PulseEventSubscriber(UnityEngine.Object owner, Action action)
		{
			this.owner = owner;
			this.action = action;
			startTime = PulseTime(Instance.GetTimeUntilNextPulse());
			ignoreMusicStartTime = false;
		}

		public PulseEventSubscriber(UnityEngine.Object owner, Action action, double startDelay)
		{
			this.owner = owner;
			this.action = action;
			startTime = PulseTime(startDelay);
			ignoreMusicStartTime = false;
		}

		public PulseEventSubscriber(UnityEngine.Object owner, Action action, double startDelay, double delay)
		{
			this.owner = owner;
			this.action = action;
			startTime = PulseTime(startDelay, delay);
			ignoreMusicStartTime = false;
		}

		private double PulseTime(double startDelay, double delay = 0)
		{
			return AudioSettings.dspTime + startDelay - Instance.beatDelay + delay;
		}
	}

	private const double TOLERANCE = 0.01;
	private const int BEATS_ON_SCREEN = 5;

	[HideInInspector]
	public List<PulseEventSubscriber> pulseSubscribers;

	[HideInInspector]
	public List<PulseEventSubscriber> pulseSubscribersForNextPlay = new List<PulseEventSubscriber>();

	[SerializeField]
	private AudioMixer masterMixer;

	public AudioSource introAudioSource;
	public AudioSource musicAudioSource;

	[HideInNormalInspector]
	public double beatDelay;

	[HideInNormalInspector]
	public int bpm;

	[HideInNormalInspector]
	public double beatTravelTime;

	[HideInNormalInspector]
	public double startingDelay;

	private double startTime;

	private Music currentMusic;

	private Coroutine beatChecker;
	private Coroutine changeVolumeCoroutine;

	private bool isCurrentlyPlaying;

	public bool IsCurrentlyPlaying => isCurrentlyPlaying;

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
		startTime = this.startingDelay + AudioSettings.dspTime;
		currentMusic = music;
		currentTime = 0;
		musicAudioSource.Stop();
		musicAudioSource.loop = currentMusic.loop;
		musicAudioSource.clip = currentMusic.audioClip;
		introAudioSource.loop = false;
		introAudioSource.clip = currentMusic.introClip;
		isCurrentlyPlaying = true;
		pulseSubscribersForNextPlay.RemoveAll(p => p.owner == null);
		pulseSubscribersForNextPlay.ForEach(p => p.startTime = startTime);
		pulseSubscribers = new List<PulseEventSubscriber>(pulseSubscribersForNextPlay);
		pulseSubscribersForNextPlay.Clear();
		SchedulePlay();
		ChangeVolume(1, true);
		if (isBattle)
		{
			GameUI.Instance.beatController.InitializeBeatController();
		}
		else
		{
			ApplyMusicSettings(MusicSettings.DEFAULT_SETTINGS);
		}
	}

	public void ApplyMusicSettings(MusicSettings settings)
	{
		masterMixer.SetFloat("Lowpass", settings.lowPass);
	}

	private void SchedulePlay()
	{
		if (introAudioSource.clip != null)
		{
			introAudioSource.PlayScheduled(startTime);
			musicAudioSource.PlayScheduled(startTime + introAudioSource.clip.length);
		}
		else
		{
			musicAudioSource.PlayScheduled(startTime);
		}

		beatChecker = StartCoroutine(BeatChecker());
	}

	public void StopBeat()
	{
		if (isCurrentlyPlaying)
		{
			musicAudioSource.Stop();
			if (beatChecker != null)
			{
				StopCoroutine(beatChecker);
			}

			StopOtherElements();
			isCurrentlyPlaying = false;
		}
	}

	protected virtual void StopOtherElements()
	{
	}

	private IEnumerator BeatChecker()
	{
		const double lowerAccuracy = 0.1;
		const double upperAccuracy = 0.15;
		var timeWasValidAFrameAgo = false;
		var timeInLowerBounds = false;
		while (true)
		{
			var timeIsValid = IsTimeValid();

			if (timeIsValid)
			{
				timeWasValidAFrameAgo = true;
				TimeIsValid();
			}
			else
			{
				TimeIsInvalid(timeWasValidAFrameAgo);
				timeWasValidAFrameAgo = false;
			}

			CheckPulses();

			yield return new WaitForFixedUpdate();
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

	protected virtual void TimeIsInvalid(bool timeWasValidAFrameAgo)
	{
	}

	protected virtual void TimeIsValid()
	{
	}

	private void CheckPulses()
	{
		var time = beatDelay - TOLERANCE;
		pulseSubscribers.RemoveAll(p => p.owner == null);
		var triggered = pulseSubscribers.Where(pulseEventSubscriber =>
			(pulseEventSubscriber.ignoreMusicStartTime || AudioSettings.dspTime >= startTime) &&
			AudioSettings.dspTime - pulseEventSubscriber.startTime >= time).ToList();

		foreach (var pulseEventSubscriber in triggered)
		{
			pulseEventSubscriber.action();
			pulseEventSubscriber.startTime += beatDelay;
		}
	}

	public double GetTimeUntilNextPulse()
	{
		return beatDelay - currentTime;
	}

	public float GetVolume()
	{
		return musicAudioSource.volume;
	}

	public void SetVolume(float volume)
	{
		introAudioSource.volume = volume;
		musicAudioSource.volume = volume;
	}

	public void ChangeVolume(float newVolume, bool instant = false)
	{
		if (changeVolumeCoroutine != null)
		{
			StopCoroutine(changeVolumeCoroutine);
			changeVolumeCoroutine = null;
		}

		if (instant || !isCurrentlyPlaying)
		{
			introAudioSource.volume = newVolume;
			musicAudioSource.volume = newVolume;
		}
		else
		{
			changeVolumeCoroutine = StartCoroutine(ChangeVolumeSequence(newVolume));
		}
	}

	private IEnumerator ChangeVolumeSequence(float newVolume)
	{
		const float duration = 0.5f;
		var currentVolume = musicAudioSource.volume;
		float t = 0;
		while (t < 1)
		{
			t += Time.deltaTime / duration;
			var vol = Mathf.Lerp(currentVolume, newVolume, t);
			introAudioSource.volume = vol;
			musicAudioSource.volume = vol;
			yield return null;
		}

		changeVolumeCoroutine = null;
	}

	public static AudioManager Instance { get; private set; }
}