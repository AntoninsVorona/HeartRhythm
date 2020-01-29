using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
	[Serializable]
	public class Screens
	{
		[HideInNormalInspector]
		public MenuScreen currentScreen;

		public MainScreen mainScreen;
		public LoadGameScreen loadGameScreen;
		private List<MenuScreen> allScreens;

		public void InitScreens()
		{
			allScreens = new List<MenuScreen> {mainScreen, loadGameScreen};
			DisableScreens();
		}

		public void DisableScreens()
		{
			allScreens.ForEach(s => s.Close(false));
		}

		public MenuScreen GetScreen<T>() where T : MenuScreen
		{
			return allScreens.First(s => s is T);
		}
	}

	[Serializable]
	public struct HeartSettings
	{
		public Vector2 position;
		public float rotation;
		public float scale;

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

		public static readonly HeartSettings DEFAULT_SETTINGS = new HeartSettings(new Vector2(0, 160), 0);
	}

	[SerializeField]
	private Screens screens;

	[SerializeField]
	private CanvasGroup globalCanvasGroup;

	[SerializeField]
	private Animator heartBeatAnimator;

	[SerializeField]
	private AnimationCurve heartMovementCurve;

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
		screens.InitScreens();
	}

	private void Update()
	{
		var hit = CurrentUIHit();
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
			else if (currentHeartButton && hit != separatorGameObject)
			{
				currentHeartButton.Deselect();
				RepositionHeart(screens.currentScreen.defaultHeartLocation);
				currentHeartButton = null;
			}
		}
		else if (currentHeartButton)
		{
			currentHeartButton.Deselect();
			RepositionHeart(screens.currentScreen.defaultHeartLocation);
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
		yield return OpenScreen<MainScreen>();
	}

	public IEnumerator FadeIntoPlay()
	{
		yield return screens.currentScreen.Close();
		//TODO Fade
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

	public Coroutine OpenScreen<T>() where T : MenuScreen
	{
		var menuScreen = screens.GetScreen<T>();
		return StartCoroutine(OpenScreenSequence(menuScreen));
	}

	private IEnumerator OpenScreenSequence(MenuScreen menuScreen)
	{
		globalCanvasGroup.interactable = false;
		globalCanvasGroup.blocksRaycasts = false;
		if (screens.currentScreen)
		{
			screens.currentScreen.Close();
			yield return new WaitForSeconds(screens.currentScreen.closeDuration);
			var startSettings = screens.currentScreen.defaultHeartLocation;
			screens.currentScreen = menuScreen;
			var currentSettings = screens.currentScreen.defaultHeartLocation;
			float t = 0;
			while (t < 1)
			{
				yield return null;
				t += Time.fixedDeltaTime * 3;
				var realT = heartMovementCurve.Evaluate(t);
				RepositionHeart(new HeartSettings(
					Vector2.Lerp(startSettings.position, currentSettings.position, realT),
					Mathf.Lerp(startSettings.rotation, currentSettings.rotation, realT),
					Mathf.Lerp(startSettings.scale, currentSettings.scale, realT)
				));
			}
			yield return new WaitForSeconds(0.25f);
		}
		else
		{
			screens.currentScreen = menuScreen;
			var currentSettings = screens.currentScreen.defaultHeartLocation;
			RepositionHeart(currentSettings);
		}

		menuScreen.Open();
		yield return new WaitForSeconds(screens.currentScreen.openDuration);
		globalCanvasGroup.interactable = true;
		globalCanvasGroup.blocksRaycasts = true;
	}

	public GameObject CurrentUIHit()
	{
		return inputModule.CurrentHitWithLayer(uiLayerMask);
	}

	public static MainMenuUI Instance { get; private set; }
}