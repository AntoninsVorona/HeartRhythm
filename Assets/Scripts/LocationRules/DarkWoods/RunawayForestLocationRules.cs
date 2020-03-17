using System.Collections;
using UnityEngine;

public class RunawayForestLocationRules : LocationRules
{
	[SerializeField]
	private Mob punka;

	public Coroutine PlayPunka()
	{
		return StartCoroutine(PunkaSequence());
	}

	private IEnumerator PunkaSequence()
	{
		for (var i = 18; i > 4; i--)
		{
			yield return punka.Move(new Vector2Int(0, i));
		}
		punka.animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		yield return new WaitForSeconds(1);
	}

	protected override void OnBeatDone()
	{
	}

	protected override void OnMoveDone()
	{
	}
}