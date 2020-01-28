using UnityEngine;

public class LoadingUI : MonoBehaviour
{
	[SerializeField]
	private GameObject canvas;

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

		StopLoading();
	}

	public void StartLoading()
	{
		canvas.SetActive(true);
	}

	public void StopLoading()
	{
		canvas.SetActive(false);
	}

	public static LoadingUI Instance { get; private set; }
}