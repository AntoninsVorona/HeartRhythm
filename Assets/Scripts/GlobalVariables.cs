using System;
using UnityEngine;

[Serializable]
public class GlobalVariables
{
	[Serializable]
	public class ScavengerAssociationVariables
	{
		public bool playerHasToSleepFirst;
		public HeadSetHideAndSeekController.HeadSetState headSetState;
		public bool interceptionGuardHeartAttack;

		public ScavengerAssociationVariables(bool playerHasToSleepFirst, HeadSetHideAndSeekController.HeadSetState headSetState, bool interceptionGuardHeartAttack)
		{
			this.playerHasToSleepFirst = playerHasToSleepFirst;
			this.headSetState = headSetState;
			this.interceptionGuardHeartAttack = interceptionGuardHeartAttack;
		}
	}

	public GlobalVariables(bool wearsHeadset, int maxDanceMoveSymbols, ScavengerAssociationVariables scavengerAssociationVariables)
	{
		this.wearsHeadset = wearsHeadset;
		this.maxDanceMoveSymbols = maxDanceMoveSymbols;
		this.scavengerAssociationVariables = scavengerAssociationVariables;
	}

	public bool wearsHeadset;
	public int maxDanceMoveSymbols;
	public ScavengerAssociationVariables scavengerAssociationVariables;
}