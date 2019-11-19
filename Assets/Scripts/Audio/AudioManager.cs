using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public AudioSource musicAudioSource;

    [HideInNormalInspector]
    public float beatDelay;

    [HideInNormalInspector]
    public float bpm;

    [HideInNormalInspector]
    public float time;

    private Music currentMusic;
    private Coroutine beatChecker;
    private bool isCurrentlyPlaying;

    private void Awake()
    {
        Instance = this;
        isCurrentlyPlaying = false;
    }

    public void InitializeBattle(Music fightMusic)
    {
        bpm = fightMusic.bpm;
        beatDelay = 60 / (float) fightMusic.bpm;
        GameUI.Instance.beatController.StartBeat();
        currentMusic = fightMusic;
        musicAudioSource.Stop();
        musicAudioSource.loop = currentMusic.loop;
        musicAudioSource.clip = currentMusic.audioClip;
        isCurrentlyPlaying = true;
    }

    public void StartPlaying()
    {
        PlayerInput.Instance.acceptor.WaitingForPlayerInput = true;
        PlayerInput.Instance.acceptor.FirstBattleInputDone = false;
        beatChecker = StartCoroutine(BeatChecker());
    }

    public void SchedulePlay()
    {
        musicAudioSource.PlayScheduled(AudioSettings.dspTime + BeatController.BEATS_ON_SCREEN * beatDelay);
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
            PlayerInput.Instance.acceptor.WaitingForPlayerInput = false;
            isCurrentlyPlaying = false;
        }
    }

    private IEnumerator BeatChecker()
    {
        const float lowerAccuracy = 0.1f;
        const float upperAccuracy = 0.1f;
        var lowerThreshold = beatDelay * lowerAccuracy;
        var upperThreshold = beatDelay * upperAccuracy;
        time = 0;
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
                         PlayerInput.Instance.acceptor.WaitingForPlayerInput &&
                         timeWasValidAFrameAgo)
                {
                    PlayerInput.Instance.MissedBeat();
                    ApplyBeat();
                }

                timeWasValidAFrameAgo = false;
            }

            yield return null;
            time += Time.deltaTime;
        }

        bool IsTimeValid()
        {
            var currentTime = time % beatDelay;
            var timeIsValid = false;
            if (timeInLowerBounds)
            {
                if (currentTime > beatDelay - lowerThreshold)
                {
                    timeIsValid = true;
                }
                else
                {
                    if (currentTime < upperThreshold)
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
                timeInLowerBounds = currentTime > beatDelay - lowerThreshold;
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
            MobController.Instance.MakeMobsActions();
        }
    }

    public static AudioManager Instance { get; private set; }
}