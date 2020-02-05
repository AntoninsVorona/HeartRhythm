using UnityEngine;

public class UIHeart : MonoBehaviour
{
	[SerializeField]
	private Animator heartBeatAnimator;

	public void Reposition(AbstractMainMenu.HeartSettings heartSettings)
	{
		var heartTransform = (RectTransform) transform;
		heartTransform.anchoredPosition = heartSettings.position;
		heartTransform.rotation = Quaternion.Euler(0, 0, heartSettings.rotation);
		heartTransform.localScale = Vector3.one * heartSettings.scale;
	}

	public void Beat()
	{
		heartBeatAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
	}
}