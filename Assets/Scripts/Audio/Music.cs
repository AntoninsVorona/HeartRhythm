using UnityEngine;

[CreateAssetMenu(menuName = "Music/Music", fileName = "Music")]
public class Music : ScriptableObject
{
    public AudioClip introClip;
    public AudioClip audioClip;
    public bool loop = false;
    public int bpm = 60;
}