using System.Collections;
using UnityEngine;

public class BeatController : MonoBehaviour
{
	private static readonly int APPEAR_TRIGGER = Animator.StringToHash("Appear");
	private static readonly int DISAPPEAR_TRIGGER = Animator.StringToHash("Disappear");
	private static readonly int PULSE_TRIGGER = Animator.StringToHash("Pulse");

	[HideInInspector]
	public float distancePerSecond;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private RectTransform beatHolder;

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

	public void StartBeat(Music music)
	{
		var beatHolderRect = beatHolder.rect;
		var middlePoint = beatHolderRect.center;
		leftSpawnPoint = new Vector2(beatHolderRect.xMin, middlePoint.y);
		rightSpawnPoint = new Vector2(beatHolderRect.xMax, middlePoint.y);
		xDifference = middlePoint.x - beatHolderRect.xMin;
		const int beatsOnScreen = 5;
		distancePerSecond = xDifference / beatsOnScreen / AudioManager.Instance.beatDelay;
		firstBeatPlayed = false;
		beatGenerator = StartCoroutine(BeatGenerator());
	}

	public void StopPlaying()
	{
		Disappear();
		StopCoroutine(beatGenerator);
		StopCoroutine(pulse);
	}

	private void Appear()
	{
		animator.SetTrigger(APPEAR_TRIGGER);
	}

	private void Disappear()
	{
		animator.SetTrigger(DISAPPEAR_TRIGGER);
	}

	private IEnumerator BeatGenerator()
	{
		Appear();
		yield return new WaitForSeconds(1);
		var t = AudioManager.Instance.beatDelay;
		while (true)
		{
			if (t >= AudioManager.Instance.beatDelay - Time.deltaTime / 2)
			{
				var beat = Instantiate(beatPrefab, beatHolder);
				beat.Initialize(leftSpawnPoint, xDifference, true);
				beat = Instantiate(beatPrefab, beatHolder);
				beat.Initialize(rightSpawnPoint, xDifference, false);
				t -= AudioManager.Instance.beatDelay;
			}

			yield return null;
			t += Time.deltaTime;
		}
	}

	public void BeatPlayed(Beat beat)
	{
		Destroy(beat.gameObject);
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
		var t = AudioManager.Instance.beatDelay;
		while (true)
		{
			if (t >= AudioManager.Instance.beatDelay - Time.deltaTime / 2)
			{
				pulsingObject.SetTrigger(PULSE_TRIGGER);
				t -= AudioManager.Instance.beatDelay;
			}

			yield return null;
			t += Time.deltaTime;

//			yield return new WaitForSeconds(beatDelay);
		}
	}
}