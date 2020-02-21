using UnityEngine;

public class RhythmObstacle : Obstacle
{
	[SerializeField]
	private Animator animator;

	private AudioManager.PulseEventSubscriber pulseEventSubscriber;

	public override void Initialize(Vector2Int location)
	{
		base.Initialize(location);
		Subscribe();
	}

	private void Pulse()
	{
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
	}

	private void Subscribe()
	{
		if (pulseEventSubscriber == null)
		{
			pulseEventSubscriber =
				new AudioManager.PulseEventSubscriber(this, Pulse);
			if (AudioManager.Instance.IsCurrentlyPlaying)
			{
				AudioManager.Instance.pulseSubscribers.Add(pulseEventSubscriber);
			}
			else
			{
				AudioManager.Instance.pulseSubscribersForNextPlay.Add(pulseEventSubscriber);
			}
		}
	}

	public void Unsubscribe()
	{
		if (pulseEventSubscriber != null)
		{
			if (AudioManager.Instance.IsCurrentlyPlaying)
			{
				AudioManager.Instance.pulseSubscribers.Remove(pulseEventSubscriber);
			}
			else
			{
				AudioManager.Instance.pulseSubscribersForNextPlay.Remove(pulseEventSubscriber);
			}

			pulseEventSubscriber = null;
		}
	}
}