using System.Collections.Generic;
using UnityEngine;

public class HeadSetHideAndSeekController : MonoBehaviour
{
	public enum HeadSetState
	{
		InTrash = 0,
		FindSeekPlace = 1,
		Hidden = 2,
		OnPlayer = 3
	}

	[SerializeField]
	private HeadSetTrashPiles headSetTrashPiles;
	[SerializeField]
	private List<HeadSetHidePlace> hidePlaces;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		HeadSetStateChanged();
	}

	public void ChangeHeadSetState(HeadSetState headSetState)
	{
		SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.headSetState = headSetState;
		HeadSetStateChanged();
	}

	private void HeadSetStateChanged()
	{
		var headSetState = SaveSystem.currentGameSave.globalVariables.scavengerAssociationVariables.headSetState;
		headSetTrashPiles.HeadSetStateChanged(headSetState);
		foreach (var hidePlace in hidePlaces)
		{
			hidePlace.HeadSetStateChanged(headSetState);
		}
	}

	public static HeadSetHideAndSeekController Instance { get; private set; }
}