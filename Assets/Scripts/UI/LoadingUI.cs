using UnityEngine;

public class LoadingUI : MonoBehaviour
{
	[SerializeField]
	private GameObject canvas;

	private bool loading;

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

		loading = true;
		StopLoading();
	}

	public void StartLoading()
	{
		if (loading)
		{
			return;
		}

		canvas.SetActive(true);
		loading = true;
	}

	public void StopLoading()
	{
		if (!loading)
		{
			return;
		}

		canvas.SetActive(false);
		loading = false;
	}

	public static LoadingUI Instance { get; private set; }
}