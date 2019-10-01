using System.Collections;
using UnityEngine;

public class BeatController : MonoBehaviour
{
	public const int BEATS_ON_SCREEN = 5;
	private static readonly int PULSE_TRIGGER = Animator.StringToHash("Pulse");

	[HideInInspector]
	public float distancePerSecond;

	[SerializeField]
	private BeatHolder beatHolder;

	[SerializeField]
	private Beat beatPrefab;

	[SerializeField]
	private Animator pulsingObject;

	private bool firstBeatPlayed;
	private Coroutine beatGenerator;
	private Coroutine pulse;

	private Vector2 leftSpawnPoint;
	private Vector2 rightSpawnPoint;
	private float xDifference;

	public void StartBeat()
	{
		var beatHolderRect = beatHolder.beatHolder.rect;
		var middlePoint = beatHolderRect.center;
		leftSpawnPoint = new Vector2(beatHolderRect.xMin, middlePoint.y);
		rightSpawnPoint = new Vector2(beatHolderRect.xMax, middlePoint.y);
		xDifference = middlePoint.x - beatHolderRect.xMin;
		distancePerSecond = xDifference / BEATS_ON_SCREEN / AudioManager.Instance.beatDelay;
		firstBeatPlayed = false;
		beatGenerator = StartCoroutine(BeatGenerator());
	}

	public void StopPlaying()
	{
		Disappear();
		if (beatGenerator != null)
		{
			StopCoroutine(beatGenerator);
		}

		if (pulse != null)
		{
			StopCoroutine(pulse);
		}
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

	private IEnumerator BeatGenerator()
	{
		Appear();
		yield return new WaitForSeconds(1);
		AudioManager.Instance.SchedulePlay();
		CreateBeats();
		float t = 0;
		while (true)
		{
			if (t >= AudioManager.Instance.beatDelay - Time.deltaTime / 2)
			{
				CreateBeats();
				t -= AudioManager.Instance.beatDelay;
			}

			yield return null;
			t += Time.deltaTime;
		}
	}

	private void CreateBeats()
	{
		var beat = Instantiate(beatPrefab, beatHolder.beatHolder);
		beat.Initialize(leftSpawnPoint, xDifference, true);
		beatHolder.beats.Add(beat);
		beat = Instantiate(beatPrefab, beatHolder.beatHolder);
		beat.Initialize(rightSpawnPoint, xDifference, false);
		beatHolder.beats.Add(beat);
	}

	public void BeatPlayed(Beat beat)
	{
		beatHolder.RemoveBeat(beat);
		if (firstBeatPlayed)
		{
			return;
		}

		pulse = StartCoroutine(StartPulsing());
		firstBeatPlayed = true;
		AudioManager.Instance.StartPlaying();
	}

	private IEnumerator StartPulsing()
	{
//		Debug.LogError($"Time: {AudioManager.Instance.time} | Music: {AudioManager.Instance.musicAudioSource.time}");
		pulsingObject.SetTrigger(PULSE_TRIGGER);
		float t = 0;
		while (true)
		{
			if (t >= AudioManager.Instance.beatDelay - Time.deltaTime / 2)
			{
//				Debug.LogError($"Time: {AudioManager.Instance.time} | Music: {AudioManager.Instance.musicAudioSource.time}");
				pulsingObject.SetTrigger(PULSE_TRIGGER);
				t -= AudioManager.Instance.beatDelay;
			}

			yield return null;
			t += Time.deltaTime;
		}
	}
}