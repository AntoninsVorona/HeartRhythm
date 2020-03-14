﻿using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Location Specific/Scavenger Association/Acquire Head Set Interaction",
	fileName = "AcquireHeadSetInteraction")]
public class AcquireHeadSetInteraction : StartCutSceneInteraction
{
	public override bool ApplyInteraction()
	{
		GameLogic.Instance.Save();
		Player.Instance.LoseItem("HeadsetChestKey");
		GameSessionManager.Instance.PlayCutScene(cutSceneToStart);
		return true;
	}

	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}