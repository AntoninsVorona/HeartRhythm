using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Interactions/Basic Interactions/Fight Interaction", fileName = "FightInteraction")]
public class FightInteraction : Interaction
{
	public override bool ApplyInteraction()
	{
		GameLogic.Instance.StartCoroutine(GameLogic.Instance.GoToEnemyRealm((Enemy) owner));
		return true;
	}
}