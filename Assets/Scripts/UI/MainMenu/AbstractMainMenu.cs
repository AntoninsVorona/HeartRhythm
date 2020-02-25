using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractMainMenu : MonoBehaviour
{
	[Serializable]
	public class Screens
	{
		[HideInNormalInspector]
		public MenuScreen currentScreen;

		public MainScreen mainScreen;
		public LoadGameScreen loadGameScreen;
		public SaveGameScreen saveGameScreen;
		private List<MenuScreen> allScreens;

		public void InitScreens()
		{
			allScreens = new List<MenuScreen> {mainScreen, loadGameScreen, saveGameScreen};
			SaveLoadGameScreen.savesInitialized = false;
			DisableScreens();
		}

		public void DisableScreens()
		{
			allScreens.ForEach(s =>
			{
				if (s)
				{
					s.Close(false);
				}
			});
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
	protected Screens screens;

	[SerializeField]
	protected CanvasGroup globalCanvasGroup;

	[SerializeField]
	protected AnimationCurve heartMovementCurve;

	public UIHeart uiHeart;
	private LayerMask uiLayerMask;

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

	public Coroutine OpenScreen<T>(bool withAnimation = true) where T : MenuScreen
	{
		var menuScreen = screens.GetScreen<T>();
		return StartCoroutine(OpenScreenSequence(menuScreen, withAnimation));
	}

	private IEnumerator OpenScreenSequence(MenuScreen menuScreen, bool withAnimation)
	{
		globalCanvasGroup.interactable = false;
		globalCanvasGroup.blocksRaycasts = false;
		if (screens.currentScreen)
		{
			if (withAnimation)
			{
				screens.currentScreen.Close();
				yield return new WaitForSeconds(screens.currentScreen.closeDuration);
				var startSettings = screens.currentScreen.defaultHeartLocation;
				screens.currentScreen = menuScreen;
				var currentSettings = screens.currentScreen.defaultHeartLocation;
				float t = 0;
				while (t < 1)
				{
					yield return new WaitForFixedUpdate();
					t += Time.fixedDeltaTime * 3;
					var realT = heartMovementCurve.Evaluate(t);
					uiHeart.Reposition(new HeartSettings(
						Vector2.Lerp(startSettings.position, currentSettings.position, realT),
						Mathf.Lerp(startSettings.rotation, currentSettings.rotation, realT),
						Mathf.Lerp(startSettings.scale, currentSettings.scale, realT)
					));
				}

				yield return new WaitForSeconds(0.25f);
			}
			else
			{
				screens.currentScreen.Close(false);
				screens.currentScreen = menuScreen;
				var currentSettings = screens.currentScreen.defaultHeartLocation;
				uiHeart.Reposition(currentSettings);
			}
		}
		else
		{
			screens.currentScreen = menuScreen;
			var currentSettings = screens.currentScreen.defaultHeartLocation;
			uiHeart.Reposition(currentSettings);
		}

		if (withAnimation)
		{
			menuScreen.Open();
			yield return new WaitForSeconds(screens.currentScreen.openDuration);
		}
		else
		{
			menuScreen.Open(false);
		}

		globalCanvasGroup.interactable = true;
		globalCanvasGroup.blocksRaycasts = true;
	}

	public GameObject CurrentUIHit()
	{
		return GetModule().CurrentHitWithLayer(uiLayerMask);
	}

	protected abstract CustomStandaloneInputModule GetModule();

	public void ApplyCancel()
	{
		if (globalCanvasGroup.interactable)
		{
			screens.currentScreen.ApplyCancel();
		}
	}

	public virtual IEnumerator FadeIntoPlay()
	{
		globalCanvasGroup.interactable = false;
		globalCanvasGroup.blocksRaycasts = false;
		yield return screens.currentScreen.Close();
	}

	public static AbstractMainMenu Instance { get; private set; }
}