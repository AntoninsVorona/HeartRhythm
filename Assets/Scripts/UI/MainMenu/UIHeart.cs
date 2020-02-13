using UnityEngine;

public class UIHeart : MonoBehaviour
{
	[SerializeField]
	private Animator heartBeatAnimator;

	[SerializeField]
	private AudioSource heartBeatSound;

	private AudioManager.PulseEventSubscriber pulseEventSubscriber;

	public void Reposition(AbstractMainMenu.HeartSettings heartSettings)
	{
		var heartTransform = (RectTransform) transform;
		heartTransform.anchoredPosition = heartSettings.position;
		heartTransform.rotation = Quaternion.Euler(0, 0, heartSettings.rotation);
		heartTransform.localScale = Vector3.one * heartSettings.scale;
	}

	public void Beat(bool sound)
	{
		heartBeatAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		if (sound)
		{
			heartBeatSound.Play();
		}
	}

	private void SoundlessBeat()
	{
		Beat(false);
	}

	public void Subscribe()
	{
		pulseEventSubscriber = new AudioManager.PulseEventSubscriber(this, SoundlessBeat,
			AudioManager.Instance.GetTimeUntilNextPulse(), -0.03f);
		AudioManager.Instance.pulseSubscribers.Add(pulseEventSubscriber);
	}

	public void Unsubscribe()
	{
		if (pulseEventSubscriber != null)
		{
			AudioManager.Instance.pulseSubscribers.Remove(pulseEventSubscriber);
			pulseEventSubscriber = null;
		}
	}

	public void Reset()
	{
		heartBeatAnimator.SetTrigger(AnimatorUtilities.RESET_TRIGGER);
	}
}