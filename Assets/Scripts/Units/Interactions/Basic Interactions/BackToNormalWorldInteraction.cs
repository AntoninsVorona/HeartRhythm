using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Back To Normal World Interaction", fileName = "BackToNormalWorldInteraction")]
public class BackToNormalWorldInteraction : Interaction
{
	public bool enablePieceMode;
	
	public override bool ApplyInteraction()
	{
		GameLogic.Instance.BackToRealWorld(enablePieceMode);
		return true;
	}
	
	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}