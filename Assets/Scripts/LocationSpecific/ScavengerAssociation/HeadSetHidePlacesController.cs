using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeadSetHidePlacesController : MonoBehaviour
{
	private List<HeadSetHidePlace> hidePlaces;

	private void Start()
	{
		hidePlaces = GetComponentsInChildren<HeadSetHidePlace>().ToList();
	}

	public void ActivateHidePlaces()
	{
		foreach (var hidePlace in hidePlaces)
		{
			hidePlace.StartBlinking();
		}
	}

	public void HeadSetIsHidden()
	{
		foreach (var hidePlace in hidePlaces)
		{
			hidePlace.HeadSetIsHidden();
		}
	}
}