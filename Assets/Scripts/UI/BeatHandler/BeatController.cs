using System.Collections;
using UnityEngine;

public class BeatController : MonoBehaviour
{
	private static readonly int APPEAR_TRIGGER = Animator.StringToHash("Appear");
	private static readonly int DISAPPEAR_TRIGGER = Animator.StringToHash("Disappear");
	private static readonly int PULSE_TRIGGER = Animator.StringToHash("Pulse");

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
	private float beatDelay;
	
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
		firstBeatPlayed = false;
		beatGenerator = StartCoroutine(BeatGenerator());
		beatDelay = 60 / (float) music.bpm;
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
		const int beatsOnScreen = 5;
		var distancePerSecond = xDifference / beatsOnScreen / beatDelay;
		Debug.Log(distancePerSecond);
		while (true)
		{
			var beat = Instantiate(beatPrefab, beatHolder);
			beat.Initialize(distancePerSecond, leftSpawnPoint, xDifference, true);
			beat = Instantiate(beatPrefab, beatHolder);
			beat.Initialize(distancePerSecond, rightSpawnPoint, xDifference, false);
			yield return new WaitForSeconds(beatDelay);
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
		while (true)
		{
			pulsingObject.SetTrigger(PULSE_TRIGGER);
			yield return new WaitForSeconds(beatDelay);
		}
	}
}