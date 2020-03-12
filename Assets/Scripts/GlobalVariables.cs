using System;
using UnityEngine;

[Serializable]
public class GlobalVariables
{
	[Serializable]
	public class ScavengerAssociationVariables
	{
		public HeadSetPlace.HeadSetState headSetState;
		public bool interceptionGuardHeartAttack;

		public ScavengerAssociationVariables(HeadSetPlace.HeadSetState headSetState, bool interceptionGuardHeartAttack)
		{
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