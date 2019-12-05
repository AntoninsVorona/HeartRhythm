using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Enter Level Interaction", fileName = "EnterLevelInteraction")]
public class EnterLevelInteraction : Interaction
{
	public LevelData levelToEnter;
	
	public override bool ApplyInteraction()
	{
		GameLogic.Instance.LoadLevel(levelToEnter);
		return true;
	}
	
	protected override object[] GetDescriptionParams()
	{
		return new object[0];
	}
}