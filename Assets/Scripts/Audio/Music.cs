using UnityEngine;

[CreateAssetMenu(menuName = "Music/Music", fileName = "Music")]
public class Music : ScriptableObject
{
	public AudioClip audioClip;
	public bool loop = false;
	public int bpm = 60;
}