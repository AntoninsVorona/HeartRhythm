using System.Collections;
using UnityEngine;

public class BeatController : MonoBehaviour
{
    public const int BEATS_ON_SCREEN = 5;
    private const double TOLERANCE = 0.01;
    private static readonly int PULSE_TRIGGER = Animator.StringToHash("Pulse");

    [SerializeField]
    private BeatHolder beatHolder;

    [SerializeField]
    private Beat beatPrefab;

    [SerializeField]
    private Animator pulsingObject;

    private Coroutine beatGenerator;
    private Coroutine pulse;

    private Vector2 leftSpawnPoint;
    private Vector2 rightSpawnPoint;
    private Vector2 middlePoint;
    private double travelTime;

    public void StartBeat()
    {
        var beatHolderRect = beatHolder.beatHolder.rect;
        middlePoint = beatHolderRect.center;
        leftSpawnPoint = new Vector2(beatHolderRect.xMin, middlePoint.y);
        rightSpawnPoint = new Vector2(beatHolderRect.xMax, middlePoint.y);
        travelTime = AudioManager.Instance.beatDelay * BEATS_ON_SCREEN;
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
        pulse = StartCoroutine(SchedulePulse());
        CreateBeat(leftSpawnPoint);
        CreateBeat(rightSpawnPoint);
        var startTime = AudioSettings.dspTime;
        while (true)
        {
            if (AudioSettings.dspTime - startTime >= AudioManager.Instance.beatDelay - TOLERANCE)
            {
                CreateBeat(leftSpawnPoint);
                CreateBeat(rightSpawnPoint);
                startTime += AudioManager.Instance.beatDelay;
            }
            
            yield return null;
        }
    }

    private void CreateBeat(Vector2 startPoint)
    {
        var beat = Instantiate(beatPrefab, beatHolder.beatHolder);
        beat.Initialize(startPoint, middlePoint, AudioSettings.dspTime, AudioSettings.dspTime + travelTime);
        beatHolder.beats.Add(beat);
    }

    public void BeatPlayed(Beat beat)
    {
        beatHolder.RemoveBeat(beat);
    }

    private IEnumerator SchedulePulse()
    {
        var startTime = AudioSettings.dspTime + travelTime - AudioManager.Instance.beatDelay;
        AudioManager.Instance.SchedulePlay(travelTime);
        while (true)
        {
            if (AudioSettings.dspTime - startTime >= AudioManager.Instance.beatDelay - TOLERANCE)
            {
//				Debug.LogError($"PULSE: Time: {AudioManager.Instance.currentTime} | Music: {AudioManager.Instance.musicAudioSource.time}");
                pulsingObject.SetTrigger(PULSE_TRIGGER);
                startTime += AudioManager.Instance.beatDelay;
            }
            
            yield return null;
        }
    }
}