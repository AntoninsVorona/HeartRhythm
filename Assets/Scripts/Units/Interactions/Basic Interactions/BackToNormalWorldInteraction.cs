using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Back To Normal World Interaction", fileName = "BackToNormalWorldInteraction")]
public class BackToNormalWorldInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		GameLogic.Instance.BackToRealWorld();
		return true;
	}
}