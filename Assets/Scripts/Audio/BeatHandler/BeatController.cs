using System.Collections;
using UnityEngine;

public class BeatController : MonoBehaviour
{
	private static readonly int PULSE_TRIGGER = Animator.StringToHash("Pulse");

	[SerializeField]
	private BeatHolder beatHolder;

	[SerializeField]
	private Beat beatPrefab;

	[SerializeField]
	private Animator pulsingObject;

	private Vector2 leftSpawnPoint;
	private Vector2 rightSpawnPoint;
	private Vector2 middlePoint;

	public void InitializeBeatController()
	{
		var beatHolderRect = beatHolder.beatHolder.rect;
		middlePoint = beatHolderRect.center;
		leftSpawnPoint = new Vector2(beatHolderRect.xMin, middlePoint.y);
		rightSpawnPoint = new Vector2(beatHolderRect.xMax, middlePoint.y);
		SchedulePulse();
		ScheduleBeats();
	}

	public void StopPlaying()
	{
		Disappear();
	}

	private void Appear()
	{
		beatHolder.Appear();
	}

	private void Disappear()
	{
		beatHolder.Disappear();
	}

	public void Deactivate()
	{
		beatHolder.Deactivate();
	}

	private void ScheduleBeats()
	{
		Appear();
		var startTime = AudioManager.Instance.beatTravelTime - AudioManager.Instance.startingDelay;
		while (startTime > 0)
		{
			var time = AudioSettings.dspTime - startTime;
			CreateBeat(leftSpawnPoint, time);
			CreateBeat(rightSpawnPoint, time);
			startTime -= AudioManager.Instance.beatDelay;
		}

		var leftBeatEvent =
			new AudioManager.PulseEventSubscriber(this, CreateLeftBeat, -startTime) {ignoreMusicStartTime = true};
		var rightBeatEvent =
			new AudioManager.PulseEventSubscriber(this, CreateRightBeat, -startTime) {ignoreMusicStartTime = true};
		AudioManager.Instance.pulseSubscribers.Add(leftBeatEvent);
		AudioManager.Instance.pulseSubscribers.Add(rightBeatEvent);
	}

	private void CreateLeftBeat()
	{
		CreateBeat(leftSpawnPoint, AudioSettings.dspTime);
	}

	private void CreateRightBeat()
	{
		CreateBeat(rightSpawnPoint, AudioSettings.dspTime);
	}

	private void CreateBeat(Vector2 startPoint, double startTime)
	{
		var beat = Instantiate(beatPrefab, beatHolder.beatHolder);
		beat.Initialize(startPoint, middlePoint, startTime,
			startTime + AudioManager.Instance.beatTravelTime);
		beatHolder.beats.Add(beat);
	}

	public void BeatPlayed(Beat beat)
	{
		beatHolder.RemoveBeat(beat);
	}

	private void Pulse()
	{
		pulsingObject.SetTrigger(PULSE_TRIGGER);
		// Debug.LogError($"PULSE: Time: {AudioManager.Instance.CurrentTime} | Music: {AudioManager.Instance.musicAudioSource.time}");
	}

	private void EqualizerBump()
	{
		// Debug.LogError($"PULSE: Time: {AudioManager.Instance.currentTime} | Music: {AudioManager.Instance.musicAudioSource.time}");
		var equalizerController = GameUI.Instance.equalizerController;
		if (equalizerController.active)
		{
			equalizerController.CreateBump();
		}
	}

	private void SchedulePulse()
	{
		var bumpEvent =
			new AudioManager.PulseEventSubscriber(this, EqualizerBump, AudioManager.Instance.startingDelay);
		var pulseEvent =
			new AudioManager.PulseEventSubscriber(this, Pulse, AudioManager.Instance.startingDelay);
		AudioManager.Instance.pulseSubscribers.Add(bumpEvent);
		AudioManager.Instance.pulseSubscribers.Add(pulseEvent);
	}
}