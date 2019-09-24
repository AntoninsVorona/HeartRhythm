using UnityEngine;

[CreateAssetMenu(menuName = "Music/Music", fileName = "Music")]
public class Music : ScriptableObject
{
	public AudioClip audioClip;
	public int bpm = 60;
}