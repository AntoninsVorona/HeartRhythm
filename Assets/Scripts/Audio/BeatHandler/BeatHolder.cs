using System.Collections.Generic;
using UnityEngine;

public class BeatHolder : MonoBehaviour
{
	private static readonly int APPEAR_TRIGGER = Animator.StringToHash("Appear");
	private static readonly int DISAPPEAR_TRIGGER = Animator.StringToHash("Disappear");

	public RectTransform beatHolder;

	[HideInNormalInspector]
	public Animator animator;

	[HideInInspector]
	public List<Beat> beats = new List<Beat>();

	public void Appear()
	{
		gameObject.SetActive(true);
		animator = GetComponent<Animator>();
		animator.SetTrigger(APPEAR_TRIGGER);
	}

	public void Disappear()
	{
		animator.SetTrigger(DISAPPEAR_TRIGGER);
		var leftoverBeats = new List<Beat>(beats);
		leftoverBeats.ForEach(RemoveBeat);
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
		var bs = new List<Beat>(beats);
		bs.ForEach(RemoveBeat);
	}

	public void RemoveBeat(Beat beat)
	{
		beats.Remove(beat);
		Destroy(beat.gameObject);
	}
}