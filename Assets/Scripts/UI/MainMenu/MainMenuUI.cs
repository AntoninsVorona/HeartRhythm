using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Animations;

public class MainMenuUI : MonoBehaviour
{
	[Serializable]
	public class Screens
	{
		public MainScreen mainScreen;

		public void DisableScreens()
		{
			mainScreen.Close(false);
		}
	}

	[Serializable]
	public class HeartSettings
	{
		public Vector2 position;
		public float rotation;
		public float scale;

		public HeartSettings()
		{
			scale = 1;
		}

		public HeartSettings(Vector2 position, float rotation)
		{
			this.position = position;
			this.rotation = rotation;
			scale = 1;
		}

		public HeartSettings(Vector2 position, float rotation, float scale)
		{
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public static HeartSettings defaultSettings = new HeartSettings(new Vector2(0, 150), 0);
	}

	[SerializeField]
	private Screens screens;

	[SerializeField]
	private CanvasGroup globalCanvasGroup;

	[SerializeField]
	private Animator heartBeatAnimator;

	[SerializeField]
	private MainMenuIntroController introController;

	[SerializeField]
	private LetterController letterController;

	[SerializeField]
	private CanvasGroup pressAnyKeyText;

	[SerializeField]
	private Animator shaker;
	[SerializeField]
	private CustomStandaloneInputModule inputModule;

	[SerializeField]
	private GameObject separatorGameObject;

	private Coroutine musicLessHeartBeat;
	private Coroutine pressAnyKey;
	private LayerMask uiLayerMask;
	private HeartButton currentHeartButton;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		uiLayerMask = LayerMask.GetMask("UI");
		globalCanvasGroup.interactable = false;
		globalCanvasGroup.blocksRaycasts = false;
		screens.DisableScreens();
	}

	private void Update()
	{
		var hit = inputModule.CurrentHitWithLayer(uiLayerMask);
		if (hit)
		{
			var heartButton = hit.GetComponentInParent<HeartButton>();
			if (heartButton)
			{
				if (heartButton != currentHeartButton)
				{
					if (currentHeartButton)
					{
						currentHeartButton.Deselect();
					}

					currentHeartButton = heartButton;
					RepositionHeart(currentHeartButton.Select());
				}

				if (Input.GetMouseButtonDown(0))
				{
					heartButton.Click();
				}
			}
			else if (currentHeartButton && hit.gameObject != separatorGameObject)
			{
				Debug.Log(hit.name);
				currentHeartButton.Deselect();
				RepositionHeart(HeartSettings.defaultSettings);
				currentHeartButton = null;
			}
		}
		else if (currentHeartButton)
		{
			currentHeartButton.Deselect();
			RepositionHeart(HeartSettings.defaultSettings);
			currentHeartButton = null;
		}
	}

	private void RepositionHeart(HeartSettings heartSettings)
	{
		var heartTransform = (RectTransform) heartBeatAnimator.transform;
		heartTransform.anchoredPosition = heartSettings.position;
		heartTransform.rotation = Quaternion.Euler(0, 0, heartSettings.rotation);
		heartTransform.localScale = Vector3.one * heartSettings.scale;
	}

	public void Show()
	{
		introController.gameObject.SetActive(true);
		pressAnyKeyText.gameObject.SetActive(false);
		StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		introController.Initialize();
		var stages = introController.GetStagesAmount();
		yield return new WaitForSeconds(0.03f);
		for (var i = 1; i < stages; i++)
		{
			yield return new WaitForSeconds(0.97f);
			heartBeatAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
			yield return new WaitForSeconds(0.03f);
			introController.ChangeStage(i);
			if (i + 1 != stages)
			{
				shaker.SetTrigger($"Shake{i}");
			}
		}

		musicLessHeartBeat = StartCoroutine(MusicLessHeartBeat());
		pressAnyKey = StartCoroutine(PressAnyKey());

		yield return new WaitForSeconds(2f);
		yield return new WaitUntil(() => Input.anyKeyDown);
		if (pressAnyKey != null)
		{
			StopCoroutine(pressAnyKey);
		}

		pressAnyKeyText.gameObject.SetActive(false);
		yield return letterController.InitiateFlightSequence(heartBeatAnimator.transform.position);
		screens.mainScreen.Open();
		yield return new WaitForSeconds(0.7f);
		globalCanvasGroup.interactable = true;
		globalCanvasGroup.blocksRaycasts = true;
	}

	private IEnumerator MusicLessHeartBeat()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);
			heartBeatAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
		}
	}

	private IEnumerator PressAnyKey()
	{
		yield return new WaitForSeconds(2.5f);
		float t = 0;
		pressAnyKeyText.gameObject.SetActive(true);
		pressAnyKeyText.alpha = 0;
		while (t < 1)
		{
			yield return null;
			t += Time.fixedDeltaTime;
			pressAnyKeyText.alpha = t;
		}
	}

	public static MainMenuUI Instance { get; private set; }
}