using System;
using System.Collections.Generic;
using UnityEngine;

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

	private readonly List<InteractionUI> interactions = new List<InteractionUI>();

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void Initialize(Unit interactingWith)
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
	}

	public void InitializeInteractionUI(InteractionUI interactionUI, Interaction interaction)
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
		mainSymbolHolder.AddSymbol(arrows[movementDirection], index);
	}
}