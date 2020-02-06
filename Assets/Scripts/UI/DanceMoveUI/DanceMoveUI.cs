using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DanceMoveUI : MonoBehaviour
{
	[Serializable]
	public class
		DirectionalArrowDictionary : SerializableDictionary<MovementDirectionUtilities.MovementDirection, Sprite>
	{
	}

	[SerializeField]
	private SymbolHolder mainSymbolHolder;

	[SerializeField]
	private DirectionalArrowDictionary arrows;

	[SerializeField]
	private Sprite waitingForInput;

	[SerializeField]
	private Sprite unknownSymbol;

	[SerializeField]
	private Sprite lockedUnknownSymbol;

	[SerializeField]
	private Transform interactionsHolder;

	[SerializeField]
	private InteractionUI interactionPrefab;

	[SerializeField]
	private Image cutOut;

	[SerializeField]
	private AnimationCurve cutOutCurve;

	private Coroutine showCutOut;
	private Coroutine clearCutOut;

	private readonly List<InteractionUI> interactions = new List<InteractionUI>();

	public void Deactivate(bool force)
	{
		if (showCutOut != null)
		{
			StopCoroutine(showCutOut);
		}

		if (force)
		{
			gameObject.SetActive(false);
		}
		else
		{
			clearCutOut = StartCoroutine(ClearCutOut());
		}
	}

	public void Initialize()
	{
		cutOut.rectTransform.sizeDelta = CutOutMax();
		Deactivate(true);
	}

	public void InitializeInteraction(Unit interactingWith)
	{
		mainSymbolHolder.Initialize(PlayerInput.Instance.maxDanceMoveSymbols, waitingForInput);
		var unitInteractions = interactingWith.interactions;
		for (var i = 0; i < unitInteractions.Count; i++)
		{
			var interaction = unitInteractions[i];
			InteractionUI interactionUI;
			if (interactions.Count == i)
			{
				interactionUI = Instantiate(interactionPrefab, interactionsHolder);
				interactions.Add(interactionUI);
			}
			else
			{
				interactionUI = interactions[i];
				interactionUI.gameObject.SetActive(true);
			}

			InitializeInteractionUI(interactionUI, interaction);
		}

		for (var i = unitInteractions.Count; i < interactions.Count; i++)
		{
			interactions[i].gameObject.SetActive(false);
		}

		gameObject.SetActive(true);
		if (clearCutOut != null)
		{
			StopCoroutine(clearCutOut);
		}

		cutOut.rectTransform.sizeDelta = CutOutMax();
		showCutOut = StartCoroutine(ShowCutOut());
	}

	private void InitializeInteractionUI(InteractionUI interactionUI, Interaction interaction)
	{
		var symbolCount = interaction.danceMovesSetToApply.Count;
		interactionUI.Initialize(interaction.interactionSymbol, interaction.GetDescription(), symbolCount);
		if (!interaction.visibility.visibleOnUI)
		{
			interactionUI.gameObject.SetActive(false);
		}

		for (var i = 0; i < symbolCount; i++)
		{
			var danceMove = interaction.danceMovesSetToApply[i];
			Sprite sprite;
			if (!danceMove.lockedState.locked)
			{
				sprite = arrows[danceMove.danceMove];
			}
			else
			{
				switch (danceMove.lockedState.lockedType)
				{
					case Interaction.LockedType.CanGuess:
						sprite = unknownSymbol;
						break;
					case Interaction.LockedType.OnlyWhenKnown:
						sprite = lockedUnknownSymbol;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			interactionUI.AddSymbol(sprite, i);
		}
	}

	public void AddSymbol(MovementDirectionUtilities.MovementDirection movementDirection, int index)
	{
		mainSymbolHolder.AddSymbol(arrows[movementDirection], index,
			movementDirection == MovementDirectionUtilities.MovementDirection.None);
	}

	public void FillLeftoverSymbols()
	{
		mainSymbolHolder.FillLeftoverSymbols();
	}

	private IEnumerator ShowCutOut()
	{
		yield return ResizeCutOut(CutOutMin());
		showCutOut = null;
	}

	private IEnumerator ClearCutOut()
	{
		yield return ResizeCutOut(CutOutMax());
		clearCutOut = null;
		gameObject.SetActive(false);
	}

	private IEnumerator ResizeCutOut(Vector2 finalSize)
	{
		RepositionOnPlayer();
		var startingCutOut = cutOut.rectTransform.sizeDelta;
		float t = 0;
		var repositionAgain = true;
		while (t < 1)
		{
			t += Time.fixedDeltaTime * 5;
			var realT = cutOutCurve.Evaluate(t);
			if (repositionAgain && t > 0.5f)
			{
				RepositionOnPlayer();
				repositionAgain = false;
			}

			cutOut.rectTransform.sizeDelta = Vector2.Lerp(startingCutOut, finalSize, realT);
			yield return new WaitForFixedUpdate();
		}

		RepositionOnPlayer();
	}

	private void RepositionOnPlayer()
	{
		var targetPosition =
			GameSessionManager.Instance.currentSceneObjects.currentWorld.GetCellCenterWorld(Player.Instance
				.CurrentPosition) + Player.Instance.SpriteOffset;
		Vector2 screenPoint = GameCamera.Instance.camera.WorldToScreenPoint(targetPosition);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(GameUI.Instance.canvasRect, screenPoint,
			null, out var canvasPos);
		cutOut.transform.localPosition = canvasPos;
	}

	private Vector2 CutOutMax()
	{
		return new Vector2(Screen.width, Screen.height) * 2;
	}

	private Vector2 CutOutMin()
	{
		return new Vector2(Screen.width, Screen.height) / 4;
	}
}