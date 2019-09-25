using UnityEngine;

public class Beat : MonoBehaviour
{
	private Vector2 startPosition;
	private float distanceToTravel;
	private float distanceTraveled;
	private bool goingRight;

	public void Initialize(Vector2 startPosition, float distanceToTravel, bool goingRight)
	{
		this.startPosition = startPosition;
		((RectTransform) transform).anchoredPosition = this.startPosition;
		this.distanceToTravel = distanceToTravel;
		this.goingRight = goingRight;
	}

	private void Update()
	{
		var delta = GameUI.Instance.beatController.distancePerSecond * Time.deltaTime;
		distanceTraveled += delta;
		if (goingRight)
		{
			((RectTransform) transform).anchoredPosition += new Vector2(delta, 0);
		}
		else
		{
			((RectTransform) transform).anchoredPosition -= new Vector2(delta, 0);
		}

		if (distanceTraveled >= distanceToTravel)
		{
			GameUI.Instance.beatController.BeatPlayed(this);
		}
	}
}