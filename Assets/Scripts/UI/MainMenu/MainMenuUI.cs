using System.Collections;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup globalCanvasGroup;
	[SerializeField]
	private Animator heartBeatAnimator;
	[SerializeField]
	private MainMenuFadeController fadeController;
	
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

		globalCanvasGroup.interactable = false;
	}

	public void Show()
	{
		StartCoroutine(ShowSequence());
	}

	private IEnumerator ShowSequence()
	{
		fadeController.Initialize();
		var stages = fadeController.GetStagesAmount();
		yield return new WaitForSeconds(0.03f);
		for (var i = 1; i < stages; i++)
		{
			yield return new WaitForSeconds(0.97f);
			heartBeatAnimator.SetTrigger(AnimatorUtilities.START_TRIGGER);
			yield return new WaitForSeconds(0.03f);
			fadeController.ChangeStage(i);
		}
		globalCanvasGroup.interactable = true;
	}
	
	public static MainMenuUI Instance { get; private set; }
}