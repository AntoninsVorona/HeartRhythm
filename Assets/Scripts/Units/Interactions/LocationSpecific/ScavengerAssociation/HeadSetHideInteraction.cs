﻿using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Head Set Hide Interaction",
	fileName = "HeadSetHideInteraction")]
public class HeadSetHideInteraction : StartCutSceneInteraction
{
	public override bool ApplyInteraction()
	{
		((HeadSetHidePlace) owner).Interacted();
		GameLogic.Instance.PlayCutScene(cutSceneToStart);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}