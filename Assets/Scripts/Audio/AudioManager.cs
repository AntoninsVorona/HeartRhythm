using UnityEngine;

public class AudioManager : MonoBehaviour
{
	[SerializeField]
	private AudioSource musicAudioSource;
	
	private Music currentMusic;
	
	private void Awake()
	{
		Instance = this;
	}

	public void InitializeBattle(Music fightMusic)
	{
		GameUI.Instance.beatController.StartBeat(fightMusic);
		currentMusic = fightMusic;
		musicAudioSource.Stop();
		musicAudioSource.loop = currentMusic.loop;
		musicAudioSource.clip = currentMusic.audioClip;
	}

	public void StartPlaying()
	{
		musicAudioSource.Play();
	}

	public static AudioManager Instance { get; private set; }
}