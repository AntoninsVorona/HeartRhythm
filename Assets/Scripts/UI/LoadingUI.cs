using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
	private const float FADE_DURATION = 0.5f;

	[SerializeField]
	private GameObject canvas;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TextMeshProUGUI loadingText;

	private bool loading;
	private Coroutine startSequence;
	private Coroutine stopSequence;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		canvas.SetActive(false);
		loading = false;
	}

	public Coroutine StartLoading(bool animate = true)
	{
		if (loading)
		{
			return startSequence;
		}

		loading = true;
		canvas.SetActive(true);
		if (animate)
		{
			startSequence = StartCoroutine(StartSequence());
			return startSequence;
		}

		return null;
	}

	private IEnumerator StartSequence()
	{
		loadingText.gameObject.SetActive(false);
		animator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		yield return new WaitForSeconds(FADE_DURATION);
		loadingText.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		startSequence = null;
	}

	public Coroutine StopLoading()
	{
		if (!loading)
		{
			return stopSequence;
		}

		stopSequence = StartCoroutine(StopSequence());
		return stopSequence;
	}

	private IEnumerator StopSequence()
	{
		loadingText.gameObject.SetActive(false);
		animator.SetTrigger(AnimatorUtilities.STOP_TRIGGER);
		yield return new WaitForSeconds(FADE_DURATION);
		canvas.SetActive(false);
		loading = false;
		stopSequence = null;
	}

	public static LoadingUI Instance { get; private set; }
}